using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger_Victory : MonoBehaviour {
    
    void OnTriggerEnter(Collider collider) {
        if(collider.GetComponent<PlayerChampion>() != null)
            GameHandler.Instance.Victory(PunTeams.Team.red);
    }
}
