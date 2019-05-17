using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This script handles cases where enemies will enter the radius of a turret
*/
/// <summary>
/// This script handles cases where enemies will enter the radius of a turret.
/// </summary>
public class TurretRange : MonoBehaviour {

    // Public variables
    public Turret turret;
    public bool showRadius;

    // Hide the trigger around the turret
    void Start() {
        if(!showRadius) {
            GetComponent<MeshRenderer>().enabled = false;
        }
    }

    // Whenever an enemy enters the radius (trigger) of the turret, tell the core script
    private void OnTriggerEnter(Collider other) {
        Entity entity = other.GetComponent<Entity>();
        if(entity != null) {
            if (entity.team != turret.team) {
                turret.EnemyEnterRadius(entity);
            }
        }
    }

    // Whenever an enemy leaves the radius (trigger) of the turret, tell the core script
    private void OnTriggerExit(Collider other) {
        Entity entity = other.GetComponent<Entity>();
        if(entity != null) {
            turret.EnemyLeaveRadius(entity);
        }
    }
}