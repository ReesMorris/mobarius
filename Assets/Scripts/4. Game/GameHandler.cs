﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameHandler : MonoBehaviour {

    public static GameHandler Instance;

    public delegate void OnGameStart();
    public static OnGameStart onGameStart;

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
        photonView.RPC("Begin", PhotonTargets.All);
    }

    public void SpawnAll() {
        photonView.RPC("InstantiatePlayers", PhotonTargets.All);
    }

    void OnPhotonPlayerConnected(PhotonPlayer other) {
        if(GameUIHandler.Instance.TimeElapsed > 10f)
            SoundManager.Instance.PlaySound("Announcer/SummonerReconnected");
    }

    void OnPhotonPlayerDisconnected(PhotonPlayer other) {
        if (GameUIHandler.Instance.TimeElapsed > 10f)
            SoundManager.Instance.PlaySound("Announcer/SummonerDisconnected");
    }

    [PunRPC]
    void Begin() {
        onGameStart();
    }

    [PunRPC]
    void InstantiatePlayers() {
        GameObject player = PhotonNetwork.Instantiate(PhotonNetwork.player.CustomProperties["championName"].ToString(), Vector3.zero, Quaternion.identity, 0);
        player.GetComponent<PlayerChampion>().Respawn();
    }

}
