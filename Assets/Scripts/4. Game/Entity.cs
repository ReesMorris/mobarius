﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Entity : MonoBehaviour {

    public bool useEntityBehaviour;
    public Image healthBarFill;

    PhotonView photonView;
    [HideInInspector] public PunTeams.Team team;
    [HideInInspector] bool IsDead;

    float health;
    float maxHealth;

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

    public bool GetIsDead() {
        PlayerChampion playerChampion = GetComponent<PlayerChampion>();
        if (playerChampion)
            return playerChampion.IsDead;
        return IsDead;
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
        if (useEntityBehaviour) {
            health = Mathf.Max(health - amount, 0f);
            if (healthBarFill != null)
                healthBarFill.fillAmount = health / maxHealth;
            if (health <= 0f) {
                IsDead = true;

                // Last hit?
                if(attacker != null) {
                    if(attacker == PhotonNetwork.player) {
                        ScoreHandler.Instance.OnMinionKill();
                    }
                }

                if (PhotonNetwork.isMasterClient) {
                    PhotonNetwork.Destroy(gameObject);
                }
            }
        }
    }

}