using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This is a debugging script used to test XP - do not use
*/
/// <summary>
/// This is a debugging script used to test XP - do not use.
/// </summary>
public class TriggerXP : MonoBehaviour {

    // Public variables
    public int amount;

    // Gives XP to player when trigger is entered
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