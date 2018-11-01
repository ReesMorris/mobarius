using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameHandler : MonoBehaviour {

    public Map currentMap {get; set;}
    Vector2 spawnIndexes;
    PhotonView photonView;

    void Start() {
        photonView = GetComponent<PhotonView>();
    }

    public void SpawnPlayers() {
        photonView.RPC("SpawnPlayer", PhotonTargets.All);
    }

    [PunRPC]
    void SpawnPlayer() {
        Vector3 position = currentMap.blueSpawns[0].transform.position;
        if(PhotonNetwork.player.GetTeam() == PunTeams.Team.red) {
            position = currentMap.redSpawns[0].transform.position;
        }
        GameObject player = PhotonNetwork.Instantiate(PhotonNetwork.player.CustomProperties["championName"].ToString(), position, Quaternion.identity, 0);
        PlayerCamera playerCamera = Camera.main.GetComponent<PlayerCamera>();
        playerCamera.target = player.transform;
        playerCamera.enabled = true;
        player.GetComponent<NavMeshAgent>().enabled = true;
        player.GetComponent<PlayerMovement>().enabled = true;
    }
}
