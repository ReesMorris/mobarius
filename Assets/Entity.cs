using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Entity : MonoBehaviour {

    public Image healthBarFill;

    PhotonView photonView;
    [HideInInspector] public PunTeams.Team team;
    [HideInInspector] public bool IsDead;

    float health;
    float maxHealth;

    void Start() {
        photonView = GetComponent<PhotonView>();
       
        if(healthBarFill != null) {
            healthBarFill.color = GameUIHandler.Instance.enemyHealthColour;
            if (PhotonNetwork.player.GetTeam() == team)
                healthBarFill.color = GameUIHandler.Instance.allyHealthColour;
        }
    }

    public void Init(float maxHealth) {
        photonView = GetComponent<PhotonView>();
        photonView.RPC("EntityInit", PhotonTargets.All, maxHealth);
    }

    [PunRPC]
    void EntityInit(float _maxHealth) {
        health = maxHealth = _maxHealth;
    }

    // Called when a champion takes damage by a source
    [PunRPC]
    void EntityDamage(float amount, PhotonPlayer attacker) {
        health = Mathf.Max(health - amount, 0f);
        healthBarFill.fillAmount = health / maxHealth;
        if (health <= 0f) {
            IsDead = true;
            gameObject.SetActive(false);
        }
    }

}
