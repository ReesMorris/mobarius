﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This script handles the particle systems generated by Alucard's R effect
*/
/// <summary>
/// This script handles the particle systems generated by Alucard's R effect.
/// </summary>
public class AlucardR_Effect : MonoBehaviour {

    // Private variables
    PhotonView photonView;
    float damage;
    float duration;
    int attackerId;
    Dictionary<Entity, float> trapped;
    PhotonPlayer attacker;
    bool allowCollisions;

    /// <summary>
    /// Initialises the particle system on the network and deals damage.
    /// </summary>
    /// <param name="damage">The damage dealt by the ability</param>
    /// <param name="duration">The duration of the particle system</param>
    /// <param name="attackerId">The ID of the player performing the ability</param>
    public void Init(float damage, float duration, int attackerId) {
        photonView = GetComponent<PhotonView>();
        photonView.RPC("InitRPC", PhotonTargets.AllBuffered, damage, duration, attackerId);
    }

    // Initialise the effect on the network
    [PunRPC]
    void InitRPC(float _damage, float _duration, int _attackerId) {
        trapped = new Dictionary<Entity, float>();
        damage = _damage;
        duration = _duration;
        attackerId = _attackerId;
        attacker = PhotonView.Find(attackerId).owner;
        allowCollisions = true;
        transform.position += Vector3.up * 0.3f;

        if (PhotonNetwork.isMasterClient)
            StartCoroutine("Handler");
    }

    // Handled by AlucardR_Trigger.cs; slow the entity down
    public void TriggerEnter(Entity entity) {
        if (allowCollisions) {
            if (entity.team != attacker.GetTeam()) {
                if (trapped != null) {
                    if (!trapped.ContainsKey(entity)) {
                        float speed = entity.GetMovementSpeed();
                        trapped.Add(entity, speed);
                        entity.SetMovementSpeed(speed * 0.4f);
                    }
                }
            }
        }
    }

    // Handled by AlucardR_Trigger.cs; restore normal movement speed to the entity
    public void TriggerExit(Entity entity, bool remove) {
        if (trapped != null) {
            if (trapped.ContainsKey(entity)) {
                entity.SetMovementSpeed(trapped[entity]);
                if(remove)
                    trapped.Remove(entity);
            }
        }
    }

    // Handles the particle systems
    IEnumerator Handler() {
        yield return new WaitForSeconds(duration);
        allowCollisions = false;

        // Stun and damage
        foreach (KeyValuePair<Entity, float> dict in trapped) {
            if (dict.Key == null)
                continue;
            PhotonView targetView = dict.Key.GetComponent<PhotonView>();

            // Stun for 2s
            dict.Key.SetMovementSpeed(0);

            // Take damage
            if (dict.Key.GetComponent<PlayerChampion>() != null)
                targetView.RPC("Damage", PhotonTargets.AllBuffered, damage, attackerId);
            else if (dict.Key.GetComponent<Minion>() != null)
                targetView.RPC("EntityDamage", PhotonTargets.AllBuffered, damage, attackerId);
        }

        // Wait 2s
        yield return new WaitForSeconds(2.5f);

        // Cancel stun
        foreach (KeyValuePair<Entity, float> dict in trapped) {
            if (dict.Key == null)
                continue;
            TriggerExit(dict.Key, false);
        }

        PhotonNetwork.Destroy(gameObject);
    }
}