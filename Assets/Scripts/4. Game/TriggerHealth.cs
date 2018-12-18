using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerHealth : MonoBehaviour {

	public float healing;

    void OnTriggerEnter(Collider other) {
        PlayerChampion playerChampion = other.GetComponent<PlayerChampion>();
        if(playerChampion != null) {
        PhotonView photonView = playerChampion.GetComponent<PhotonView>();
            if (photonView.isMine) {
                photonView.RPC("Heal", PhotonTargets.All, healing);
            }
        }
    }
}