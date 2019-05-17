using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    This script contains the logic for the inhibitor building
*/
/// <summary>
/// This script contains the logic for the inhibitor building.
/// </summary>
public class Inhibitor : MonoBehaviour {

    // Delegates
    public delegate void OnInhibitorDestroyed(Inhibitor inhibitor);
    public static OnInhibitorDestroyed onInhibitorDestroyed;
    
    // Public variables
    public float baseHealth;
    public float healthRegen;
    public PunTeams.Team team;
    public List<Turret> prerequisites;
    public GameObject healthbarUI;
    public Image healthImage;

    // Private variables
    bool ready;
    float currentHealth;
    PhotonView photonView;

    // Updates the UI for the local player and assigns private variables when the game starts.
    void Start() {
        GetComponent<Entity>().team = team;
        healthbarUI.SetActive(false);
        healthImage.fillAmount = 1;
        photonView = GetComponent<PhotonView>();

        currentHealth = baseHealth;
    }

    // Waits every frame for the building to be ready
    void Update() {
        if (healthImage != null && !ready)
            OnGameStart();
    }

    // Called by the master client when the game begins
    void OnGameStart() {
        Turret.onTurretDestroyed += OnTurretDestroyed;

        // Set health bar colours
        healthImage.color = GameUIHandler.Instance.enemyHealthColour;
        if (PhotonNetwork.player.GetTeam() == team)
            healthImage.color = GameUIHandler.Instance.allyHealthColour;

        // Health regen
        //if (PhotonNetwork.isMasterClient)
        //StartCoroutine("RegenHealth");
        ready = true;
    }

    // Called when the building is destroyed
    void OnDestroy() {
        Turret.onTurretDestroyed -= OnTurretDestroyed;
        StopAllCoroutines();
    }

    // Health regeneration
    IEnumerator RegenHealth() {
        while(true) {
            yield return new WaitForSeconds(1f);
            photonView.RPC("Heal", PhotonTargets.AllBuffered, 15f);
        }
    }

    // Called when a turret is destroyed
    void OnTurretDestroyed(Turret turret) {
        if (this != null) {
            if (prerequisites.Count > 0) {
                foreach (Turret t in prerequisites) {
                    if (t == turret) {
                        prerequisites.Remove(t);
                        break;
                    }
                }
                if (prerequisites.Count == 0)
                    photonView.RPC("BecomeTargetable", PhotonTargets.AllBuffered);
            }
        }
    }

    // Called to become targetable
    [PunRPC]
    void BecomeTargetable() {
        gameObject.layer = LayerMask.NameToLayer("Targetable");
        healthbarUI.SetActive(true);
    }

    /// <summary>
    /// Applies damage to the building.
    /// </summary>
    /// <param name="amount">The amount of damage to take</param>
    /// <param name="shooterId">The viewID of the attacker</param>
    [PunRPC]
    public void Damage(float amount, int shooterId) {

        // Update the UI
        currentHealth = Mathf.Max(0f, currentHealth - amount);
        healthImage.fillAmount = (currentHealth / baseHealth);

        // Are we dead?
        if (currentHealth == 0f) {
            if (PhotonNetwork.player.GetTeam() == team)
                GameUIHandler.Instance.MessageWithSound("Announcer/AllyInhibitorDestroyed", "Ally inhibitor destroyed!");
            else
                GameUIHandler.Instance.MessageWithSound("Announcer/EnemyInhibitorDestroyed", "Enemy inhibitor destroyed!");

            // Tell other scripts we're dead
            if (onInhibitorDestroyed != null)
                onInhibitorDestroyed(this);
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Applies health to the building.
    /// </summary>
    /// <param name="amount">The amount of health to regenerate</param>
    [PunRPC]
    public void Heal(float amount) {
        currentHealth = Mathf.Min(currentHealth + amount, baseHealth);
        healthImage.fillAmount = (currentHealth / baseHealth);
    }
}
