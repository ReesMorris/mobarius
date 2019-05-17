using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This is a debugging script used to test damage - do not use
*/
/// <summary>
/// This is a debugging script used to test damage - do not use.
/// </summary>
public class TriggerDamage : MonoBehaviour {

    // Public variables
    public float damage;

    // Damages the player on collision
    void OnTriggerEnter(Collider other) {
        PlayerChampion playerChampion = other.GetComponent<PlayerChampion>();
        if(playerChampion != null) {
            PhotonView photonView = playerChampion.GetComponent<PhotonView>();
            if (photonView.isMine) {
                photonView.RPC("Damage", PhotonTargets.All, damage, null);
            }
        }
    }
}