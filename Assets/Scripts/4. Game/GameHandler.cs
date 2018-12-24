using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameHandler : MonoBehaviour {

    public static GameHandler Instance;

    public Map currentMap {get; set;}
    Vector2 spawnIndexes;
    PhotonView photonView;

    void Start() {
        Instance = this;
        photonView = GetComponent<PhotonView>();
    }

    public void StartGame() {
        SpawnAll();
        GameUIHandler.Instance.StartGameTimer();
    }

    public void SpawnAll() {
        photonView.RPC("InstantiatePlayers", PhotonTargets.All);
    }

    [PunRPC]
    void InstantiatePlayers() {
        GameObject player = PhotonNetwork.Instantiate(PhotonNetwork.player.CustomProperties["championName"].ToString(), Vector3.zero, Quaternion.identity, 0);
        player.name = PhotonNetwork.player.CustomProperties["championName"].ToString();
        PlayerCamera playerCamera = Camera.main.GetComponent<PlayerCamera>();
        playerCamera.target = player.transform;
        playerCamera.enabled = true;
        player.GetComponent<PlayerChampion>().Respawn();
    }

}
