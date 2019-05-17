using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
    This script handles cases where enemies will enter the radius of Alucard's R
*/
/// <summary>
/// This script handles cases where enemies will enter the radius of Alucard's R.
/// </summary>
public class AlucardR_Trigger : MonoBehaviour {

    // Public variables
    public AlucardR_Effect effect;

    // Whenever an enemy enters the radius (trigger) of the ability, tell the core script
    void OnTriggerEnter(Collider other) {
        if (PhotonNetwork.isMasterClient) {
            Entity entity = other.GetComponent<Entity>();
            if (entity != null)
                effect.TriggerEnter(entity);
        }
    }

    // Whenever an enemy leaves the radius (trigger) of the ability, tell the core script
    void OnTriggerExit(Collider other) {
        if (PhotonNetwork.isMasterClient) {
            Entity entity = other.GetComponent<Entity>();
            if (entity != null)
                effect.TriggerExit(entity, true);
        }
    }
}
