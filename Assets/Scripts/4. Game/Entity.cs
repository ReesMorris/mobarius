using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

/*
    The script used to control an entity
*/
/// <summary>
/// The script used to control an entity.
/// </summary>
public class Entity : MonoBehaviour {

    // Delegates
    public delegate void OnEntityDeath(int attackerId);
    public static OnEntityDeath onEntityDeath;

    // Public variables
    public bool useEntityBehaviour;
    public Image healthBarFill;

    [Header("XP")]
    public int XPOnDeath;
    public float XPRadius;

    public PhotonView photonView;
    [HideInInspector] public PunTeams.Team team;
    [HideInInspector] bool IsDead;

    // Private variables
    float health;
    float maxHealth;

    // Set the health bar colour depending on the local player's team, once the entity is spawned.
    void Start() {
        if (useEntityBehaviour) {
            photonView = GetComponent<PhotonView>();

            if (healthBarFill != null) {
                healthBarFill.color = GameUIHandler.Instance.enemyHealthColour;
                if (PhotonNetwork.player.GetTeam() == team)
                    healthBarFill.color = GameUIHandler.Instance.allyHealthColour;
            }
        }
    }

    /// <summary>
    /// Checks to see whether this entity is dead.
    /// </summary>
    /// <returns>
    /// True if the entity is dead; false if not.
    /// </returns>
    public bool GetIsDead() {
        PlayerChampion playerChampion = GetComponent<PlayerChampion>();
        if (playerChampion)
            return playerChampion.IsDead;
        return IsDead;
    }

    /// <summary>
    /// Initiates the entity on the network.
    /// </summary>
    /// <param name="maxHealth">The entitys maximum health value</param>
    public void Init(float maxHealth) {
        photonView = GetComponent<PhotonView>();
        photonView.RPC("EntityInit", PhotonTargets.AllBuffered, maxHealth);
    }

    // Syncs the entity's maximum health value across the network
    [PunRPC]
    void EntityInit(float _maxHealth) {
        health = maxHealth = _maxHealth;
    }

    // Called when an entity takes damage by a source
    [PunRPC]
    void EntityDamage(float amount, int attackerId) {

        // Update the health bar UI and kill the entity if necessary
        if (useEntityBehaviour) {
            health = Mathf.Max(health - amount, 0f);
            if (healthBarFill != null)
                healthBarFill.fillAmount = health / maxHealth;
            if (health <= 0f) {
                IsDead = true;

                // Give XP to all enemy champions who are not in the area
                if(PhotonNetwork.isMasterClient) {
                    Collider[] hitColliders = Physics.OverlapSphere(transform.position, XPRadius);
                    for(int i = 0; i < hitColliders.Length; i++) {
                        PlayerChampion playerChampion = hitColliders[i].GetComponent<PlayerChampion>();
                        if (playerChampion != null) {
                            if(playerChampion.PhotonView.owner.GetTeam() != team) {
                                ChampionXP championXP = playerChampion.GetComponent<ChampionXP>();
                                championXP.photonView.RPC("GiveXP", PhotonTargets.All, XPOnDeath, false);
                            }
                        }
                    }
                }

                // Tell other scripts we're dead
                if (onEntityDeath != null)
                    onEntityDeath(attackerId);

                // Destroy the bullet
                if (PhotonNetwork.isMasterClient)
                    PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// Calculates the entity's movement speed.
    /// </summary>
    /// <returns>
    /// The movement speed of the entity; 0 as a fallback.
    /// </returns>
    public float GetMovementSpeed() {
        PlayerChampion champion = GetComponent<PlayerChampion>();
        Minion minion = GetComponent<Minion>();
        if (champion != null)
            return champion.Champion.movementSpeed;
        if (minion != null)
            return minion.speed;
        return 0;
    }

    // Called to reduce speed of an entity across the network
    [PunRPC]
    public void SetMovementSpeed(float speed) {
        PlayerChampion champion = GetComponent<PlayerChampion>();
        Minion minion = GetComponent<Minion>();
        if (champion != null)
            champion.Champion.movementSpeed = speed;
        if (minion != null)
            minion.speed = speed;
    }
}
