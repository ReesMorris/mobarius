using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This is a debugging script used to test victory - do not use
*/
/// <summary>
/// This is a debugging script used to test victory - do not use.
/// </summary>
public class TriggerVictory : MonoBehaviour {
    
    // Wins the game when trigger is entered
    void OnTriggerEnter(Collider collider) {
        if(collider.GetComponent<PlayerChampion>() != null)
            GameHandler.Instance.Victory(PunTeams.Team.red);
    }
}
