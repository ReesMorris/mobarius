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
            if (entity.team != minion.Minion.Entity.team) {
                minion.EnemyEnterRadius(entity);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        Entity entity = other.GetComponent<Entity>();
        if (entity != null) {
            minion.EnemyLeaveRadius(entity);
        }
    }
}
