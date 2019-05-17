using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This script handles Alucard's passive effect
*/
/// <summary>
/// This script handles Alucard's passive effect.
/// </summary>
public class AlucardPassive : MonoBehaviour {

    // Private variables
    PhotonView photonView;
    PlayerChampion champion;

    // Set up event listeners and references to other scripts
    void Start() {
        Entity.onEntityDeath += OnEntityDeath;
        photonView = GetComponent<PhotonView>();
        champion = GetComponent<PlayerChampion>();
    }

    // If Alucard kills another player, heal them by a small amount
    void OnEntityDeath(int attackerId) {
        if(photonView.isMine) {
            if(photonView.viewID == attackerId) {
                photonView.RPC("Heal", PhotonTargets.All, champion.Champion.maxHealth * 0.07f);
            }
        }
    }
}
