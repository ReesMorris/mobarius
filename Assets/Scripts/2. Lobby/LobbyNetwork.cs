using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyNetwork : MonoBehaviour {

    public static LobbyNetwork Instance;

    public Button cancelButton;

    private ChampionSelect championSelect;
    public enum LobbyStates { none, searching, championSelect, inGame };
    public LobbyStates lobbyState;
    private MainMenuManager mainMenuManager;
    private UserManager userManager;
    private MapManager mapManager;
    private GameHandler gameHandler;
    private SearchingLabel searchingLabel;

    void Start() {
        Instance = this;
        mainMenuManager = GetComponent<MainMenuManager>();
        championSelect = GetComponent<ChampionSelect>();
        userManager = GetComponent<UserManager>();
        mapManager = GetComponent<MapManager>();
        gameHandler = GetComponent<GameHandler>();
        searchingLabel = SearchingLabel.Instance;
        cancelButton.onClick.AddListener(StopPlay);
    }

    /* Photon Networking */

    // Called in the MainMenuManager Prepare() function once user is authenticated
    public void ConnectToNetwork () {
        mainMenuManager.Preparing();
        print("Connecting to server ...");
        PhotonNetwork.ConnectUsingSettings("0.0.0");

    }
    // Called when connected to the server, allowing us to define player information
    void OnConnectedToMaster() {
        print("Connected to server");
        PhotonNetwork.player.NickName = userManager.account.username;
        PhotonNetwork.JoinLobby();
    }
    // Will tell the MainMenuManager that this process is ready to go
    void OnJoinedLobby() {
        mainMenuManager.Ready();
        print("Joined Lobby");
    }

    // Called by UI buttons; will queue the player based on map name
    public void Play(string mapName) {
        gameHandler.currentMap = mapManager.GetMap(mapName);
        // Check to see if we are on the network and not already in a room
        if (PhotonNetwork.connected && PhotonNetwork.room == null) {
            // Find a room to join
            lobbyState = LobbyStates.searching;
            PhotonNetwork.JoinRoom(mapName);
        }
    }

    // Called if a room could not be found; will create a new one with the searching player as the host
    void OnPhotonJoinRoomFailed() {
        // Create a room named after the map the player wants to play
        PhotonNetwork.CreateRoom(gameHandler.currentMap.name, new RoomOptions {
            MaxPlayers = gameHandler.currentMap.maxPlayers
        }, null);
    }

    // Called by UI to stop searching for a room to join
    public void StopPlay() {
        if (PhotonNetwork.connected && PhotonNetwork.room != null) {
            lobbyState = LobbyStates.none;
            PhotonNetwork.LeaveRoom();
            searchingLabel.OnSearchingStop();
            championSelect.OnStop();
        }
    }

    void OnJoinedRoom() {
        searchingLabel = SearchingLabel.Instance;
        searchingLabel.OnSearchingStart();
    }

    private void Update() {
        if(PhotonNetwork.connected) {
            if(PhotonNetwork.room != null) {

                if (lobbyState == LobbyStates.searching) {
                    // Wait for players
                    if (PhotonNetwork.room.PlayerCount == PhotonNetwork.room.MaxPlayers) {
                        searchingLabel.OnSearchingPause();
                        championSelect.OnStart();
                        lobbyState = LobbyStates.championSelect;
                    }
                }

                if(lobbyState == LobbyStates.championSelect) {
                    // Return to searching if a player has left
                    if(PhotonNetwork.room.PlayerCount < PhotonNetwork.room.MaxPlayers) {
                        lobbyState = LobbyStates.searching;
                        searchingLabel.OnSearchingStart();
                        championSelect.OnStop();
                    }
                }
            }
        }
    }

    public void SetLobbyState(LobbyStates lobbyStates) {
        lobbyState = lobbyStates;
    }
}
