using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.AI;

/*
    This script contains functions related to the player champion
*/
/// <summary>
/// This script contains functions related to the player champion.
/// </summary>
public class PlayerChampion : MonoBehaviour {

    // Public variables
    public delegate void OnPlayerDamaged();
    public static OnPlayerDamaged onPlayerDamaged;

    [Header("UI")]
    public GameObject canvas;
    public TMP_Text usernameText;
    public Image healthBarFill;
    public Image damageIndicator;
    public Transform hundredsContainer;
    public GameObject healthBarLine;
    public Image manaBarFill;

    public Champion Champion { get; protected set; }
    public bool IsDead { get; protected set; }
    public PhotonView PhotonView { get; protected set; }

    // Private variables
    float oldHealth;
    GameUIHandler gameUIHandler;
    MapProperties mapProperties;

    /*  General Code  */

    void Start () {
        PhotonView = GetComponent<PhotonView>();
        mapProperties = MapManager.Instance.GetMapProperties();
        if (PhotonNetwork.player.IsLocal) {
            GetComponent<Entity>().team = PhotonView.owner.GetTeam();
            gameUIHandler = GameObject.Find("GameManager").GetComponent<GameUIHandler>();

            // Tell other players to rename this player's character to be the name of the champion
            gameObject.name = PhotonNetwork.player.CustomProperties["championName"].ToString();
            PhotonView.RPC("Rename", PhotonTargets.AllBuffered, PhotonNetwork.player.CustomProperties["championName"].ToString());
            usernameText.text = PhotonView.owner.NickName;

            // Create a new champion class for this player so that we can store their details
            Champion = ScriptableObject.CreateInstance<Champion>();
            Champion.Init(ChampionRoster.Instance.GetChampion(gameObject.name), PhotonNetwork.player.NickName);
            Champion.health = oldHealth = Champion.maxHealth;
            Champion.mana = Champion.maxMana;

            // Update UI to show full health and mana, etc
            if (PhotonView.isMine) {

                // Hide username/healthbar if in third person
                if (mapProperties.display == PlayerCamera.CameraDisplays.ThirdPerson)
                    canvas.SetActive(false);

                Entity.onEntityDeath += OnEntityDeath;

                // Set up ability indicators and particle effects
                AbilityHandler.Instance.SetupAbilityIndicators(gameObject);
                EffectsHandler.Instance.SetupEffects(gameObject, PhotonView);

                gameUIHandler.UpdateAbilities(Champion);
                gameUIHandler.UpdateStats(Champion);
                gameUIHandler.SetCharacterIcon(Champion);
                Respawn();
                UIHandler.Instance.HideLobbyUI();
            }

            // Tell other players to update the health and mana bar of this player
            PhotonView.RPC("UpdatePlayerHealth", PhotonTargets.AllBuffered, new object[] { Champion.health, Champion.maxHealth });
            PhotonView.RPC("UpdatePlayerMana", PhotonTargets.AllBuffered, new object[] { Champion.mana, Champion.maxMana });

        }
    }

    /// <summary>
    /// Respawns the player character on the network.
    /// </summary>
    public void Respawn() {
        PhotonView = GetComponent<PhotonView>();
        PhotonView.RPC("Spawn", PhotonTargets.All);
    }

    [PunRPC]
    void Spawn() {
        // Show the canvas for username/health/etc if in topdown view.
        canvas.SetActive(true);
        damageIndicator.GetComponent<DamageIndicator>().StartResize();

        PhotonView = GetComponent<PhotonView>();
        gameObject.layer = LayerMask.NameToLayer("Targetable");

        if (PhotonView.isMine) {
            mapProperties = MapManager.Instance.GetMapProperties();
            if (mapProperties.display == PlayerCamera.CameraDisplays.ThirdPerson)
                canvas.SetActive(false);

            // Set the spawn position depending on team
            GameHandler gameHandler = GameHandler.Instance;
            Vector3 position = gameHandler.currentMap.blueSpawns[0].transform.position;
            transform.rotation = gameHandler.currentMap.blueSpawns[0].transform.rotation;
            if (PhotonView.owner.GetTeam() == PunTeams.Team.red) {
                position = gameHandler.currentMap.redSpawns[0].transform.position;
                transform.rotation = gameHandler.currentMap.redSpawns[0].transform.rotation;
            }

            // Re-set the camera and move the player to the spawn point
            position += (Vector3.up * 3f);
            transform.position = position;
            PlayerCamera playerCamera = Camera.main.GetComponent<PlayerCamera>();
            playerCamera.target = gameObject.transform;
            playerCamera.enabled = true;
            playerCamera.FocusOnPlayer(true);
            GetComponent<NavMeshAgent>().enabled = true;
            GetComponent<PlayerMovement>().enabled = true;
        }

        // Update some variables
        IsDead = false;
    }

    // Rename the local character's GameObject across the network
    [PunRPC]
    void Rename(string newName) {
        gameObject.name = newName;
    }

    // Called when a champion takes damage by a source
    [PunRPC]
    void Damage(float amount, int attackerId) {
        if(!Champion.invincible && !IsDead) {
            Champion.health = Mathf.Max(Champion.health - amount, 0f);

            // If the champion is the local player, update their UI to reflect the damage
            if (PhotonView.isMine) {

                // Get the attacker and store their damage in the champion
                PhotonView attacker = PhotonView.Find(attackerId);
                Champion.damage.Insert(0, new Damage(attacker, amount, gameUIHandler.TimeElapsed));

                // Call the delegate for when the player is damaged
                if(onPlayerDamaged != null)
                    onPlayerDamaged();

                // Update the local health bar UI
                gameUIHandler.UpdateStats(Champion);

                // Does the player have any health left?
                if (Champion.health <= 0f) {
                    GetComponent<PlayerMovement>().StopMovement();
                    GetComponent<PlayerAnimator>().PlayAnimation("Death");
                    PhotonView.RPC("OnDeath", PhotonTargets.All, Champion.GetKiller());
                    Champion.ResetDamage();
                    DeathHandler.Instance.OnDeath(this);
                }

                // Tell other players to update the UI which shows over the character's head
                PhotonView.RPC("UpdatePlayerHealth", PhotonTargets.All, Champion.health, Champion.maxHealth);
            }
        }
    }

    // Called when a player dies
    [PunRPC]
    void OnDeath(int killerId) {

        // Update some variables
        IsDead = true;

        // Get the killer
        PhotonView killer = PhotonView.Find(killerId);

        // Hide the canvas for username/health/etc.
        canvas.SetActive(false);

        // Kill messages
        if (killer == null) {
            // Executed
            GameUIHandler.Instance.MessageWithSound("Announcer/Executed", "Executed!");
        } else {
            if(ScoreHandler.Instance.IsFirstBlood()) {
                GameUIHandler.Instance.MessageWithSound("Announcer/FirstBlood", "First blood!");
            } else if (PhotonNetwork.player.GetTeam() == PhotonView.owner.GetTeam()) {
                // Death was on ally side
                if(PhotonView.isMine) {
                    GameUIHandler.Instance.MessageWithSound("Announcer/SelfSlain", "You have been slain!");
                } else {
                    // Ally player death
                    GameUIHandler.Instance.MessageWithSound("Announcer/AllySlain", killer.owner.NickName + " has slain " + PhotonView.owner.NickName);
                }
            } else {
                // Death was on enemy side
                if(PhotonNetwork.player == killer.owner) {
                    GameUIHandler.Instance.MessageWithSound("Announcer/SelfKill", "You have slain " + PhotonView.owner.NickName);
                } else {
                    // I didn't kill them
                    GameUIHandler.Instance.MessageWithSound("Announcer/EnemySlain", killer.owner.NickName + " has slain " + PhotonView.owner.NickName);
                }
            }
            ScoreHandler.Instance.IncreaseScore(killer.owner.GetTeam());

            // Give XP to the killer
            if (PhotonNetwork.isMasterClient) {
                killer.photonView.RPC("GiveXP", PhotonTargets.AllBuffered, 50, false);
            }

            // KDA scores
            if (PhotonView.isMine)
                ScoreHandler.Instance.OnDeath();
            if (PhotonNetwork.player == killer.owner)
                ScoreHandler.Instance.OnKill();
        }

        // Make this player untargetable for abilities, etc.
        gameObject.layer = LayerMask.NameToLayer("Default");

        // Stop player from being able to move
        GetComponent<NavMeshAgent>().enabled = false;
        GetComponent<PlayerMovement>().enabled = false;
    }

    // Called when a champion is healed by a source
    [PunRPC]
    void Heal(float amount) {
        if (Champion != null) {
            // If the champion is the local player, update their UI to reflect the healing
            Champion.health = Mathf.Min(Champion.health + amount, Champion.maxHealth);

            if (PhotonView.isMine) {

                // Update the local health bar UI
                gameUIHandler.UpdateStats(Champion);

                // Tell other players to update the UI which shows over the character's head
                PhotonView.RPC("UpdatePlayerHealth", PhotonTargets.AllBuffered, Champion.health, Champion.maxHealth);
            }
        }
    }

    // Called when a champion is being given mana
    [PunRPC]
    void GiveMana(float amount) {
        if (Champion != null) {
            Champion.mana = Mathf.Min(Champion.mana + amount, Champion.maxMana);

            // If the champion is the local player, update their UI to reflect the mana change
            if (PhotonView.isMine) {

                // Update the local mana bar UI
                gameUIHandler.UpdateStats(Champion);

                // Tell other players to update the UI which shows over the character's head
                PhotonView.RPC("UpdatePlayerMana", PhotonTargets.AllBuffered, Champion.mana, Champion.maxMana);
            }
        }
    }

    // Called when a champion is using mana
    [PunRPC]
    void TakeMana(float amount) {
        if (Champion != null) {
            Champion.mana = Mathf.Max(Champion.mana - amount, 0f);

            // If the champion is the local player, update their UI to reflect the mana change
            if (PhotonView.isMine) {

                // Update the local mana bar UI
                gameUIHandler.UpdateStats(Champion);

                // Tell other players to update the UI which shows over the character's head
                PhotonView.RPC("UpdatePlayerMana", PhotonTargets.AllBuffered, Champion.mana, Champion.maxMana);

            }
        }
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
            healthBarFill.color = GameUIHandler.Instance.enemyHealthColour;

        oldHealth = health;
    }
    
    // Called when an entity dies
    void OnEntityDeath(int attackerId) {
        if(attackerId == PhotonView.viewID)
            ScoreHandler.Instance.OnMinionKill();
    }

    /// <summary>
    /// Returns true if the local champion is stunned (and cannot move).
    /// </summary>
    public bool IsStunned() {
        return Champion.movementSpeed == 0f;
    }
}
