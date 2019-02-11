using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameHandler : MonoBehaviour {

    public static GameHandler Instance;

    public delegate void OnGameStart();
    public static OnGameStart onGameStart;
    public delegate void OnGameEnd();
    public static OnGameEnd onGameEnd;

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

    public void Victory(PunTeams.Team winningTeam) {
        photonView.RPC("OnVictory", PhotonTargets.AllBuffered, winningTeam);
    }
    
    [PunRPC]
    void OnVictory(PunTeams.Team winningTeam) {
        Transform losingNexus;
        if(winningTeam == PunTeams.Team.blue)
            losingNexus = GameObject.Find("Red Nexus").transform;
        else
            losingNexus = GameObject.Find("Blue Nexus").transform;
        Vector3 pos = new Vector3(losingNexus.position.x, 15f, losingNexus.position.z - 15f);
        Camera.main.GetComponent<PlayerCamera>().SetEndOfGameTarget(pos, 1f);

        StartCoroutine(VictoryEnum(winningTeam));
        if (onGameEnd != null)
            onGameEnd();
    }

    IEnumerator VictoryEnum(PunTeams.Team winningTeam) {
        yield return new WaitForSeconds(1f);
        if (PhotonNetwork.player.GetTeam() == winningTeam)
            SoundManager.Instance.PlaySound("Announcer/Victory");
        else
            SoundManager.Instance.PlaySound("Announcer/Defeat");

        yield return new WaitForSeconds(4f);
        UIHandler.Instance.ShowLobbyUI();
        LobbyNetwork.Instance.StopPlay();
    }

}
