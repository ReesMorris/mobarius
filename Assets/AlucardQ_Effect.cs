﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlucardQ_Effect : MonoBehaviour {

    PhotonView photonView;
    Ability ability;
    PhotonPlayer attacker;
    int attackerId;

    public void Init(float radius, Ability _ability, int _attackerId) {
        attacker = PhotonView.Find(_attackerId).owner;
        ability = _ability;
        attackerId = _attackerId;
        photonView = GetComponent<PhotonView>();
        photonView.RPC("InitRPC", PhotonTargets.AllBuffered, radius);
    }

    [PunRPC]
    void InitRPC(float radius) {
        radius /= 2f;
        transform.position += Vector3.up * 0.883f;
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren) {
            child.localScale = new Vector3(radius, 1, radius);
        }

        if(PhotonNetwork.isMasterClient) {
            StartCoroutine(Damage(radius));
        }
    }

    IEnumerator Damage(float radius) {
        yield return new WaitForSeconds(0.1f);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        for (int i = 0; i < hitColliders.Length; i++) {
            Entity entity = hitColliders[i].GetComponent<Entity>();
            if(entity != null) {
                print("Entity != null");
                PhotonView targetView = entity.GetComponent<PhotonView>();
                PhotonPlayer target = targetView.owner;
                if (entity.team != attacker.GetTeam()) {
                    if (entity.GetComponent<PlayerChampion>() != null)
                        targetView.RPC("Damage", PhotonTargets.AllBuffered, AbilityHandler.Instance.GetDamageFromAbility(ability, "damage"), attackerId);
                    else if(entity.GetComponent<Minion>() != null)
                        targetView.RPC("EntityDamage", PhotonTargets.AllBuffered, AbilityHandler.Instance.GetDamageFromAbility(ability, "damage"), attackerId);
                }
            }
        }
    }
}