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
        PlayerChampion playerChampion = other.GetComponent<PlayerChampion>();
        if(playerChampion != null) {
            PhotonView photonView = playerChampion.GetComponent<PhotonView>();
            if(photonView.owner.GetTeam() != turret.team) {
                turret.EnemyEnterRadius(playerChampion);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        PlayerChampion playerChampion = other.GetComponent<PlayerChampion>();
        if(playerChampion != null) {
            PhotonView photonView = playerChampion.GetComponent<PhotonView>();
            if (photonView.owner.GetTeam() != turret.team) {
                turret.EnemyLeaveRadius(playerChampion);
            }
        }
    }
}