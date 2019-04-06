using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlucardPassive : MonoBehaviour {

    PhotonView photonView;
    PlayerChampion champion;

    void Start() {
        Entity.onEntityDeath += OnEntityDeath;
        photonView = GetComponent<PhotonView>();
        champion = GetComponent<PlayerChampion>();
    }

    void OnEntityDeath(int attackerId) {
        if(photonView.isMine) {
            if(photonView.viewID == attackerId) {
                photonView.RPC("Heal", PhotonTargets.All, champion.Champion.maxHealth * 0.07f);
                Debug.Log("Healed by " + champion.Champion.maxHealth * 0.03f);
            }
        }
    }
}
