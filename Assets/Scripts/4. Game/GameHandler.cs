using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
    The script used to control an entity
*/
/// <summary>
/// The script used to control an entity.
/// </summary>
public class GameHandler : MonoBehaviour {

    // Public variables
    public static GameHandler Instance;

    public delegate void OnGameStart();
    public static OnGameStart onGameStart;
    public delegate void OnGameEnd();
    public static OnGameEnd onGameEnd;
    public Map currentMap {get; set;}

    // Private variables
    Vector2 spawnIndexes;
    PhotonView photonView;
    bool victory;

    // Set up references and allow other scripts to access this when the game starts.
    void Start() {
        Instance = this;
        photonView = GetComponent<PhotonView>();
    }

    /// <summary>
    /// Called by the master client to spawn players and start the game across the network.
    /// </summary>
    public void StartGame() {
        photonView.RPC("InstantiatePlayers", PhotonTargets.All);
        GameUIHandler.Instance.StartGameTimer();
        photonView.RPC("Begin", PhotonTargets.All);
    }

    // Called when a player connects to the network
    void OnPhotonPlayerConnected(PhotonPlayer other) {
        if(GameUIHandler.Instance.TimeElapsed > 10f)
            SoundManager.Instance.PlaySound("Announcer/SummonerReconnected");
    }

    // Called when a player disconnects from the network
    void OnPhotonPlayerDisconnected(PhotonPlayer other) {
        if (GameUIHandler.Instance.TimeElapsed > 10f)
            SoundManager.Instance.PlaySound("Announcer/SummonerDisconnected");
    }

    // The network call to commence the game across all clients
    [PunRPC]
    void Begin() {
        victory = false;
        if(onGameStart != null)
            onGameStart();
    }

    // The network call to spawn each player on the network
    [PunRPC]
    void InstantiatePlayers() {
        GameObject player = PhotonNetwork.Instantiate(PhotonNetwork.player.CustomProperties["championName"].ToString(), Vector3.zero, Quaternion.identity, 0);
        player.GetComponent<PlayerChampion>().Respawn();
    }

    /// <summary>
    /// Called by the master client once one team's nexus has been destroyed.
    /// </summary>
    /// <param name="winningTeam">The team who destroyed the nexus</param>
    public void Victory(PunTeams.Team winningTeam) {
        photonView.RPC("OnVictory", PhotonTargets.AllBuffered, winningTeam);
    }
    
    // The network call to end the game once one team's nexus has been destroyed.
    [PunRPC]
    void OnVictory(PunTeams.Team winningTeam) {
        if (!victory) {
            victory = true;
            GameObject losingNexus;
            if (winningTeam == PunTeams.Team.blue)
                losingNexus = GameObject.Find("Red Nexus");
            else
                losingNexus = GameObject.Find("Blue Nexus");

            // Prevent errors in trying to move the camera to something that doesn't exist
            if(losingNexus != null) {
                Vector3 pos = new Vector3(losingNexus.transform.position.x, 15f, losingNexus.transform.position.z - 15f);
                Camera.main.GetComponent<PlayerCamera>().SetEndOfGameTarget(pos, 1f);
            }

            // End the game
            StartCoroutine(VictoryEnum(winningTeam));
            if (onGameEnd != null)
                onGameEnd();
        }
    }

    // Plays a victory/defeat sound to players and then returns them to the lobby screen
    IEnumerator VictoryEnum(PunTeams.Team winningTeam) {
        yield return new WaitForSeconds(1f);
        if (PhotonNetwork.player.GetTeam() == winningTeam)
            SoundManager.Instance.PlaySound("Announcer/Victory");
        else
            SoundManager.Instance.PlaySound("Announcer/Defeat");

        yield return new WaitForSeconds(4f);
        Camera.main.GetComponent<PlayerCamera>().ClearEndOfGameTarget();
        Camera.main.transform.parent = null;
        UIHandler.Instance.ShowLobbyUI();
        LobbyNetwork.Instance.StopPlay();
    }

}
