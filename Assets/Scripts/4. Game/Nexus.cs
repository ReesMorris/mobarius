using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    This script contains the logic for the nexus building
*/
/// <summary>
/// This script contains the logic for the nexus building.
/// </summary>
public class Nexus : MonoBehaviour {

    // Public variables
    public float baseHealth;
    public float healthRegen;
    public PunTeams.Team team;
    public List<Inhibitor> prerequisites;
    public GameObject healthbarUI;
    public Image healthImage;

    // Private variables
    float currentHealth;
    bool destroyed;
    bool canReheal = true;
    PhotonView photonView;
    bool ready;
    MapProperties mapProperties;

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
        Inhibitor.onInhibitorDestroyed += OnInhibitorDestroyed;

        // Set health bar colours
        healthImage.color = GameUIHandler.Instance.enemyHealthColour;
        if (PhotonNetwork.player.GetTeam() == team)
            healthImage.color = GameUIHandler.Instance.allyHealthColour;

        // Health regen
        if (PhotonNetwork.isMasterClient)
            StartCoroutine(RegenHealth());

        // Map Properties
        mapProperties = MapManager.Instance.GetMapProperties();

        // Damage over time
        if (mapProperties.nexusDamagePerSec > 0)
            if (PhotonNetwork.isMasterClient)
                StartCoroutine(DamageOverTime());

        ready = true;
    }

    // Health regeneration
    IEnumerator RegenHealth() {
        while (canReheal) {
            yield return new WaitForSeconds(1f);
            photonView.RPC("Heal", PhotonTargets.AllBuffered, 15f);
        }
    }

    // Damage over time
    IEnumerator DamageOverTime() {
        while (true) {
            photonView.RPC("Damage", PhotonTargets.All, mapProperties.nexusDamagePerSec, null);
            yield return new WaitForSeconds(1f);
        }
    }

    // Called when the building is destroyed
    void OnDestroy() {
        Inhibitor.onInhibitorDestroyed -= OnInhibitorDestroyed;
        StopAllCoroutines();
    }

    // Called when a turret is destroyed
    void OnInhibitorDestroyed(Inhibitor inhibitor) {
        if (prerequisites.Count > 0) {
            foreach (Inhibitor i in prerequisites) {
                if (i == inhibitor) {
                    prerequisites.Remove(i);
                    break;
                }
            }
            if (prerequisites.Count == 0)
                photonView.RPC("BecomeTargetable", PhotonTargets.AllBuffered);
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
        currentHealth = Mathf.Max(0f, currentHealth - amount);
        healthImage.fillAmount = (currentHealth / baseHealth);

        // Are we destroyed?
        if (currentHealth == 0f && !destroyed) {
            canReheal = false;
            destroyed = true;
            healthbarUI.SetActive(false);
            if(PhotonNetwork.isMasterClient)
                GameHandler.Instance.Victory(GetComponent<Targetable>().allowTargetingBy);
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
