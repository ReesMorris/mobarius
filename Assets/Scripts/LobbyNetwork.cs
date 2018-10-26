using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyNetwork : MonoBehaviour {

    public PlayButton playButton;
    public Button cancelButton;

    private ChampionSelect championSelect;
    private enum LobbyStates { none, searching, championSelect, inGame };
    private LobbyStates lobbyState;
    private MainMenuManager mainMenuManager;
    private UserManager userManager;

    void Start() {
        mainMenuManager = GetComponent<MainMenuManager>();
        championSelect = GetComponent<ChampionSelect>();
        userManager = GetComponent<UserManager>();
        playButton.GetComponent<Button>().onClick.AddListener(Play);
        cancelButton.onClick.AddListener(StopPlay);
    }

    public void ConnectToNetwork () {
        mainMenuManager.Preparing();
        print("Connecting to server ...");
        PhotonNetwork.ConnectUsingSettings("0.0.0");

    }

    void OnConnectedToMaster() {
        print("Connected to server");
        PhotonNetwork.player.NickName = userManager.account.username;
        PhotonNetwork.JoinLobby();
    }

    void OnJoinedLobby() {
        mainMenuManager.Ready();
        print("Joined Lobby");
    }

    void Play() {
        if (PhotonNetwork.connected && PhotonNetwork.room == null) {
            lobbyState = LobbyStates.searching;
            PhotonNetwork.JoinRandomRoom();
        }
    }

    void StopPlay() {
        if (PhotonNetwork.connected && PhotonNetwork.room != null) {
            lobbyState = LobbyStates.none;
            PhotonNetwork.LeaveRoom();
            playButton.OnSearchingStop();
        }
    }

    void OnPhotonRandomJoinFailed() {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 }, null);
    }

    void OnJoinedRoom() {
        playButton.OnSearchingStart();
    }

    private void Update() {
        if(PhotonNetwork.connected) {
            if(PhotonNetwork.room != null) {

                if (lobbyState == LobbyStates.searching) {
                    // Wait for players
                    if (PhotonNetwork.room.PlayerCount == PhotonNetwork.room.MaxPlayers) {
                        playButton.OnSearchingPause();
                        championSelect.OnStart();
                        lobbyState = LobbyStates.championSelect;
                    }
                }

                if(lobbyState == LobbyStates.championSelect) {
                    // Return to searching if a player has left
                    if(PhotonNetwork.room.PlayerCount < PhotonNetwork.room.MaxPlayers) {
                        lobbyState = LobbyStates.searching;
                        playButton.OnSearchingStart();
                        championSelect.OnStop();
                    }
                }
            }
        }
    }
}
