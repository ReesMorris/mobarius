using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.AI;

public class PlayerChampion : MonoBehaviour {

    [Header("UI")]
    public TMP_Text usernameText;
    public Image healthBarFill;
    public Image damageIndicator;
    public Transform hundredsContainer;
    public GameObject healthBarLine;
    public Image manaBarFill;
    public Color enemyHealthColour;

    public Champion Champion { get; protected set; }
    public bool IsDead { get; protected set; }
    public PhotonView PhotonView { get; protected set; }
    float oldHealth;
    GameUIHandler gameUIHandler;

    /*  General Code  */

    void Start () {
        PhotonView = GetComponent<PhotonView>();
        if (PhotonNetwork.player.IsLocal) {
            gameUIHandler = GameObject.Find("GameManager").GetComponent<GameUIHandler>();              // Tell other players to rename this player's character to be the name of the champion             PhotonView.RPC("Rename", PhotonTargets.All, PhotonNetwork.player.CustomProperties["championName"].ToString());             usernameText.text = PhotonView.owner.NickName;              // Create a new champion class for this player so that we can store their details             Champion = ScriptableObject.CreateInstance<Champion>();             Champion.Init(ChampionRoster.Instance.GetChampion(gameObject.name), PhotonNetwork.player.NickName);             Champion.health = oldHealth = Champion.maxHealth;             Champion.mana = Champion.maxMana;

            // Update UI to show full health and mana, etc
            if (PhotonView.isMine) {
                gameUIHandler.UpdateAbilities(Champion);
                gameUIHandler.UpdateStats(Champion);
            }

            // Tell other players to update the health and mana bar of this player
            PhotonView.RPC("UpdatePlayerHealth", PhotonTargets.All, new object[] { Champion.health, Champion.maxHealth });
            PhotonView.RPC("UpdatePlayerMana", PhotonTargets.All, new object[] { Champion.mana, Champion.maxMana }); 
        }
    }

    public void Respawn() {
        PhotonView = GetComponent<PhotonView>();
        PhotonView.RPC("Spawn", PhotonTargets.All);
    }

    [PunRPC]
    void Spawn() {
        gameObject.layer = LayerMask.NameToLayer("Targetable");
        if(PhotonView.isMine) {
            GameHandler gameHandler = GameHandler.Instance;
            Vector3 position = gameHandler.currentMap.blueSpawns[0].transform.position;
            if (PhotonView.owner.GetTeam() == PunTeams.Team.red) {
                position = gameHandler.currentMap.redSpawns[0].transform.position;
            }
            position += (Vector3.up * 3f);
            transform.position = position;
            Camera.main.GetComponent<PlayerCamera>().FocusOnPlayer(true);
            GetComponent<NavMeshAgent>().enabled = true;
            GetComponent<PlayerMovement>().enabled = true;
            IsDead = false;
        }
    }

    [PunRPC]
    void Rename(string newName) {
        gameObject.name = newName;
    }

    // Called when a champion takes damage by a source
    [PunRPC]
    void Damage(float amount) {
        //Champion.damage.Add(new Damage(attacker, amount, gameUIHandler.TimeElapsed));
        Champion.health = Mathf.Max(Champion.health - amount, 0f);

        // If the champion is the local player, update their UI to reflect the damage
        if (PhotonView.isMine) {
            gameUIHandler.UpdateStats(Champion);

            // Does the player have any health left?
            if (Champion.health <= 0f) {
                IsDead = true;
                PhotonView.RPC("OnDeath", PhotonTargets.All);
                DeathHandler.Instance.OnDeath(this);
            }
        }

        // Tell other players that this player has been damaged (to update the health bar)
        PhotonView.RPC("UpdatePlayerHealth", PhotonTargets.All, new object[] { Champion.health, Champion.maxHealth });
    }

    [PunRPC]
    void OnDeath() {

        // Make this player untargetable for abilities, etc.
        gameObject.layer = LayerMask.NameToLayer("Default");

        // Stop player from being able to move
        GetComponent<NavMeshAgent>().enabled = false;
        GetComponent<PlayerMovement>().enabled = false;
    }

    // Called when a champion is healed by a source
    [PunRPC]
    void Heal(float amount) {
        Champion.health = Mathf.Min(Champion.health + amount, Champion.maxHealth);

        // If the champion is the local player, update their UI to reflect the healing
        if (PhotonView.isMine)
            gameUIHandler.UpdateStats(Champion);

        // Tell other players that this player has been healed (to update the health bar)
        PhotonView.RPC("UpdatePlayerHealth", PhotonTargets.All, new object[] { Champion.health, Champion.maxHealth });
    }

    // Called when a champion is being given mana
    [PunRPC]
    void GiveMana(float amount) {
        Champion.mana = Mathf.Min(Champion.mana + amount, Champion.maxMana);
        PhotonView.RPC("UpdatePlayerMana", PhotonTargets.All, new object[] { Champion.mana, Champion.maxMana });

        // If the champion is the local player, update their UI to reflect the mana change
        if (PhotonView.isMine)
            gameUIHandler.UpdateStats(Champion);
    }

    // Called when a champion is using mana
    [PunRPC]
    void TakeMana(float amount) {
        Champion.mana = Mathf.Max(Champion.mana - amount, 0f);
        PhotonView.RPC("UpdatePlayerMana", PhotonTargets.All, new object[] { Champion.mana, Champion.maxMana });

        // If the champion is the local player, update their UI to reflect the mana change
        if (PhotonView.isMine)
            gameUIHandler.UpdateStats(Champion);
    }

    // Called to update the UI bar for player mana
    [PunRPC]
    void UpdatePlayerMana(float mana, float maxMana) {
        manaBarFill.fillAmount = mana / Mathf.Max(1f, maxMana); // can't divide by 0
    }

    // Called to update the UI bar for player health
    [PunRPC]
    void UpdatePlayerHealth(float health, float maxHealth) {
        float oldFillAmount = healthBarFill.fillAmount;

        healthBarFill.fillAmount = health / maxHealth;

        // Damage taken
        if(oldHealth > health) {
            damageIndicator.fillAmount = oldFillAmount;
        }

        // Health bars
        while(hundredsContainer.childCount < (Mathf.Floor(maxHealth / 100))) {
            Instantiate(healthBarLine, hundredsContainer);
        }

        // Colour
        if (PhotonView.owner.GetTeam() != PhotonNetwork.player.GetTeam())
            healthBarFill.color = enemyHealthColour;

        oldHealth = health;
    }
}
