using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlucardW_Trigger : MonoBehaviour {

    public AlucardW_Effect effect;
    List<Entity> damaged;

    void Start() {
        damaged = new List<Entity>();
    }

    private void OnTriggerEnter(Collider other) {
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
