﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DefaultAttack : MonoBehaviour {

    public float cooldownTime;
    public GameObject bulletPrefab;

    PhotonView photonView;
    PlayerChampion playerChampion;
    PlayerMovement playerMovement;
    NavMeshAgent navMeshAgent;
    float lastShotTime;
    bool gameEnded;

    [HideInInspector] public GameObject target;

    void Start() {
        photonView = GetComponent<PhotonView>();
        playerChampion = GetComponent<PlayerChampion>();
        playerMovement = GetComponent<PlayerMovement>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        GameHandler.onGameEnd += OnGameEnd;
    }

    void OnGameEnd() {
        gameEnded = true;
    }

    void Update() {
        if(photonView.isMine && !gameEnded) {
            if (!playerChampion.IsDead) {
                if(!playerChampion.IsStunned()) {
                    if(target != null) {
                        PlayerChampion targetChampion = target.GetComponent<PlayerChampion>();
                        Entity targetEntity = target.GetComponent<Entity>();

                        // Check to make sure the target is still alive
                        if (targetChampion != null)
                            if (targetChampion.IsDead)
                                target = null;
                        if (targetEntity != null)
                            if (targetEntity.GetIsDead())
                                target = null;

                        // Is the target stil alive?
                        if (target == null)
                            playerMovement.StopMovement();
                        else {
                            if (target.GetComponent<PhotonView>() != photonView) {
                                if (Vector3.Distance(target.transform.position, transform.position) < playerChampion.Champion.range) {
                                    if (Time.time >= (lastShotTime + cooldownTime)) {
                                        lastShotTime = Time.time;
                                        photonView.RPC("Shoot", PhotonTargets.All, 100f, PhotonNetwork.player.GetTeam(), playerChampion.Champion.attackDamage, target.transform.position, target.GetComponent<PhotonView>().viewID, photonView.viewID);
                                        navMeshAgent.destination = transform.position;
                                    }
                                } else {
                                    navMeshAgent.destination = target.transform.position;
                                    navMeshAgent.isStopped = false;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    [PunRPC]
    void Shoot(float speed, PunTeams.Team team, float damage, Vector3 position, int photonId, int shooter) {
        transform.LookAt(new Vector3(position.x, transform.position.y, position.z));
        GameObject bullet = Instantiate(bulletPrefab, (transform.position + transform.forward), transform.rotation);
        Bullet b = bullet.GetComponent<Bullet>();
        b.Setup(damage, team, transform.position, photonId, shooter);
    }
}
