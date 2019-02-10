using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Nexus : MonoBehaviour {

    public float baseHealth;
    public float healthRegen;
    public PunTeams.Team team;
    public List<Inhibitor> prerequisites;
    public GameObject healthbarUI;
    public Image healthImage;

    float currentHealth;
    bool canReheal = true;
    PhotonView photonView;

    void Start() {
        GameHandler.onGameStart += OnGameStart;
        GetComponent<Entity>().team = team;
        healthbarUI.SetActive(false);
        healthImage.fillAmount = 1;
        photonView = GetComponent<PhotonView>();

        currentHealth = baseHealth;
    }

    void OnGameStart() {
        Inhibitor.onInhibitorDestroyed += OnInhibitorDestroyed;

        // Set health bar colours
        healthImage.color = GameUIHandler.Instance.enemyHealthColour;
        if (PhotonNetwork.player.GetTeam() == team)
            healthImage.color = GameUIHandler.Instance.allyHealthColour;

        // Health regen
        if (PhotonNetwork.isMasterClient)
            StartCoroutine(RegenHealth());
    }

    // Health regeneration
    IEnumerator RegenHealth() {
        while (canReheal) {
            yield return new WaitForSeconds(1f);
            photonView.RPC("Heal", PhotonTargets.AllBuffered, 15f);
        }
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

    [PunRPC]
    public void Damage(float amount, PhotonPlayer shooter) {
        currentHealth = Mathf.Max(0f, currentHealth - amount);
        healthImage.fillAmount = (currentHealth / baseHealth);
        if (currentHealth == 0f) {
            canReheal = false;
            healthbarUI.SetActive(false);
            if(PhotonNetwork.isMasterClient)
                GameHandler.Instance.Victory(GetComponent<Targetable>().allowTargetingBy);
        }
    }

    [PunRPC]
    public void Heal(float amount) {
        currentHealth = Mathf.Min(currentHealth + amount, baseHealth);
        healthImage.fillAmount = (currentHealth / baseHealth);
    }
}
