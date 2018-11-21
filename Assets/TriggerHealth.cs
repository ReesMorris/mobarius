using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerHealth : MonoBehaviour {

	public float healing;

    void OnTriggerEnter(Collider other) {
        PlayerChampion playerChampion = other.GetComponent<PlayerChampion>();
        if(playerChampion != null) {
            playerChampion.Heal(healing);
        }
    }
}
