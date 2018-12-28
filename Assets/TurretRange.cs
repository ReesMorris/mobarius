using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretRange : MonoBehaviour {

    public Turret turret;
    public bool showRadius;

    void Start() {
        if(!showRadius) {
            GetComponent<MeshRenderer>().enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other) {
        Entity entity = other.GetComponent<Entity>();
        if(entity != null) {
            PhotonView photonView = entity.GetComponent<PhotonView>();
            if (entity.team != turret.team) {
                turret.EnemyEnterRadius(entity);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        Entity entity = other.GetComponent<Entity>();
        if(entity != null) {
            PhotonView photonView = entity.GetComponent<PhotonView>();
            turret.EnemyLeaveRadius(entity);
        }
    }
}