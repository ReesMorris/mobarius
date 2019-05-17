using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This script handles cases where enemies will enter the radius of Alucard's W
*/
/// <summary>
/// This script handles cases where enemies will enter the radius of Alucard's W.
/// </summary>
public class AlucardW_Trigger : MonoBehaviour {

    // Public variables
    public AlucardW_Effect effect;

    // Private variables
    List<Entity> damaged;

    // Assign variables when the game starts.
    void Start() {
        damaged = new List<Entity>();
    }

    // Whenever an enemy enters the radius (trigger) of the ability, tell the core script
    void OnTriggerEnter(Collider other) {
        if (PhotonNetwork.isMasterClient) {
            Entity e = other.GetComponent<Entity>();
            if (e != null) {
                if (!damaged.Contains(e)) {
                    effect.OnTrigger(e);
                    damaged.Add(e);
                }
            }
        }
    }
}
