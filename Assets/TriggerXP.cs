using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerXP : MonoBehaviour {

    public int amount;

    void OnTriggerEnter(Collider other) {
        ChampionXP championXP = other.GetComponent<ChampionXP>();
        if (championXP != null) {
            PhotonView photonView = championXP.photonView;
            if (photonView.isMine) {
                photonView.RPC("GiveXP", PhotonTargets.All, amount);
            }
        }
    }
}