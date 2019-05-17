using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This script handles spawn areas for both teams
*/
/// <summary>
/// This script handles spawn areas for both teams.
/// </summary>
public class SpawnArea : MonoBehaviour {

    // Public variables
    public PunTeams.Team team;

    // Private variables
    float regenAmount = 100f;

    // Will increase the player's health regen if they are on the team of this side
    void OnTriggerEnter(Collider other) {
        PlayerChampion playerChampion = other.gameObject.GetComponent<PlayerChampion>();
        if (playerChampion) {
            PhotonView photonView = other.gameObject.GetComponent<PhotonView>();
            if(photonView.isMine && photonView.owner.GetTeam() == team) {
                playerChampion.Champion.healthRegen += regenAmount;
                playerChampion.Champion.manaRegen += regenAmount;
            }
        }
    }

    // Will stop the health regen if the player isn't on the team of this side
    void OnTriggerExit(Collider other) {
        PlayerChampion playerChampion = other.gameObject.GetComponent<PlayerChampion>();
        if (playerChampion) {
            PhotonView photonView = other.gameObject.GetComponent<PhotonView>();
            if(photonView.isMine && photonView.owner.GetTeam() == team) {
                playerChampion.Champion.healthRegen -= regenAmount;
                playerChampion.Champion.manaRegen -= regenAmount;
            }
        }
    }
}
