using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlucardR_Trigger : MonoBehaviour {

    public AlucardR_Effect effect;

    void OnTriggerEnter(Collider other) {
        if (PhotonNetwork.isMasterClient) {
            Entity entity = other.GetComponent<Entity>();
            if (entity != null)
                effect.TriggerEnter(entity);
        }
    }

    void OnTriggerExit(Collider other) {
        if (PhotonNetwork.isMasterClient) {
            Entity entity = other.GetComponent<Entity>();
            if (entity != null)
                effect.TriggerExit(entity, true);
        }
    }
}
