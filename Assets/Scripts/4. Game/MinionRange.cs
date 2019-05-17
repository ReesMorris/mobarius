using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This script handles cases where enemies will enter the radius of a minion
*/
/// <summary>
/// This script handles cases where enemies will enter the radius of a minion.
/// </summary>
public class MinionRange : MonoBehaviour {

    // Public variables
    public MinionBehaviour minion;
    public bool showRadius;

    // Hide the trigger around the minion
    void Start() {
        if (!showRadius) {
            GetComponent<MeshRenderer>().enabled = false;
        }
    }

    // Whenever an enemy enters the radius (trigger) of the minion, tell the core script
    private void OnTriggerEnter(Collider other) {
        Entity entity = other.GetComponent<Entity>();
        if (entity != null) {
            if (entity.team != minion.Minion.Entity.team) {
                minion.EnemyEnterRadius(entity);
            }
        }
    }

    // Whenever an enemy leaves the radius (trigger) of the minion, tell the core script
    private void OnTriggerExit(Collider other) {
        Entity entity = other.GetComponent<Entity>();
        if (entity != null) {
            minion.EnemyLeaveRadius(entity);
        }
    }
}
