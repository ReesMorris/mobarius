using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDamage : MonoBehaviour {

    public float damage;

    void OnTriggerEnter(Collider other) {
        PlayerChampion playerChampion = other.GetComponent<PlayerChampion>();
        if(playerChampion != null) {
            PhotonView photonView = playerChampion.GetComponent<PhotonView>();
            if (photonView.isMine) {
                photonView.RPC("Damage", PhotonTargets.All, damage);
            }
        }
    }
}