using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
    PhotonView photonView;
    float oldHealth;
    GameUIHandler gameUIHandler;

    /*  General Code  */

	void Start () {
        photonView = GetComponent<PhotonView>();
        if (PhotonNetwork.player.IsLocal) {
            gameUIHandler = GameObject.Find("GameManager").GetComponent<GameUIHandler>();              // Tell other players to rename this player's character to be the name of the champion             photonView.RPC("Rename", PhotonTargets.All, PhotonNetwork.player.CustomProperties["championName"].ToString());             usernameText.text = photonView.owner.NickName;              // Create a new champion class for this player so that we can store their details             Champion = ScriptableObject.CreateInstance<Champion>();             Champion.Init(ChampionRoster.Instance.GetChampion(gameObject.name), PhotonNetwork.player.NickName);             Champion.health = oldHealth = Champion.maxHealth;             Champion.mana = Champion.maxMana;

            // Update UI to show full health and mana, etc
            if (photonView.isMine) {
                gameUIHandler.UpdateAbilities(Champion.championName);
                gameUIHandler.UpdateStats(Champion);
            }              // Tell other players to update the health and mana bar of this player             photonView.RPC("UpdatePlayerHealth", PhotonTargets.All, new object[] { Champion.health, Champion.maxHealth });
            photonView.RPC("UpdatePlayerMana", PhotonTargets.All, new object[] { Champion.mana, Champion.maxMana }); 
        }
    }

    [PunRPC]
    void Rename(string newName) {
        gameObject.name = newName;
    }


    // Called when a champion takes damage by a source
    [PunRPC]
    void Damage(float amount) {
        Champion.health = Mathf.Max(Champion.health - amount, 0f);

        // Does the player have any health left?
        if (Champion.health <= 0f)
            print("Champion is dead");

        // If the champion is the local player, update their UI to reflect the damage
        if (photonView.isMine)
            gameUIHandler.UpdateStats(Champion);

        // Tell other players that this player has been damaged (to update the health bar)
        photonView.RPC("UpdatePlayerHealth", PhotonTargets.All, new object[] { Champion.health, Champion.maxHealth });
    }

    // Called when a champion is healed by a source
    [PunRPC]
    void Heal(float amount) {
        Champion.health = Mathf.Min(Champion.health + amount, Champion.maxHealth);

        // If the champion is the local player, update their UI to reflect the healing
        if (photonView.isMine)
            gameUIHandler.UpdateStats(Champion);

        // Tell other players that this player has been healed (to update the health bar)
        photonView.RPC("UpdatePlayerHealth", PhotonTargets.All, new object[] { Champion.health, Champion.maxHealth });
    }

    // Called when a champion is being given mana
    [PunRPC]
    void GiveMana(float amount) {
        Champion.mana = Mathf.Min(Champion.mana + amount, Champion.maxMana);
        photonView.RPC("UpdatePlayerMana", PhotonTargets.All, new object[] { Champion.mana, Champion.maxMana });

        // If the champion is the local player, update their UI to reflect the mana change
        if (photonView.isMine)
            gameUIHandler.UpdateStats(Champion);
    }

    // Called when a champion is using mana
    [PunRPC]
    void TakeMana(float amount) {
        Champion.mana = Mathf.Max(Champion.mana - amount, 0f);
        photonView.RPC("UpdatePlayerMana", PhotonTargets.All, new object[] { Champion.mana, Champion.maxMana });

        // If the champion is the local player, update their UI to reflect the mana change
        if (photonView.isMine)
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
        if (photonView.owner.GetTeam() != PhotonNetwork.player.GetTeam())
            healthBarFill.color = enemyHealthColour;

        oldHealth = health;
    }
}
