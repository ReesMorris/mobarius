using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionRange : MonoBehaviour {

    public MinionBehaviour minion;
    public bool showRadius;

    void Start() {
        if (!showRadius) {
            GetComponent<MeshRenderer>().enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other) {
        Entity entity = other.GetComponent<Entity>();
        if (entity != null) {
            PhotonView photonView = entity.GetComponent<PhotonView>();
            if (entity.team != minion.Minion.Entity.team) {
                minion.EnemyEnterRadius(entity);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        Entity entity = other.GetComponent<Entity>();
        if (entity != null) {
            PhotonView photonView = entity.GetComponent<PhotonView>();
            minion.EnemyLeaveRadius(entity);
        }
    }
}
