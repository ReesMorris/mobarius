using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable; // Source = http://forum.photonengine.com/discussion/3320/hashtable-tutorial ; accessed 3 January 2019
using UnityEngine.Networking;

public class LobbyNetwork : MonoBehaviour {

    public static LobbyNetwork Instance;
    public string setGameIdURL;
    public Button cancelButton;

    private ChampionSelect championSelect;
    public enum LobbyStates { none, searching, championSelect, inGame };
    public LobbyStates lobbyState;
    private MainMenuManager mainMenuManager;
    private UserManager userManager;
    private MapManager mapManager;
    private GameHandler gameHandler;
    private SearchingLabel searchingLabel;
    private bool shouldAttemptRejoin;
    private bool isRejoined;

    void Start() {
        shouldAttemptRejoin = true;
        Instance = this;
        mainMenuManager = GetComponent<MainMenuManager>();
        championSelect = GetComponent<ChampionSelect>();
        mapManager = GetComponent<MapManager>();
        gameHandler = GetComponent<GameHandler>();
        searchingLabel = SearchingLabel.Instance;
        cancelButton.onClick.AddListener(StopPlay);
    }

    /* Photon Networking */

    // Called in the MainMenuManager Prepare() function once user is authenticated
    public void ConnectToNetwork () {
        PhotonNetwork.AuthValues = new AuthenticationValues(UserManager.Instance.account.id); // https://forum.photonengine.com/discussion/11264/user-id-not-set ; accessed 3 January 2019
        mainMenuManager.Preparing();
        print("Connecting to server ...");
        PhotonNetwork.ConnectUsingSettings("0.0.0");

    }
    // Called when connected to the server, allowing us to define player information
    void OnConnectedToMaster() {
        print("Connected to server");
        PhotonNetwork.player.NickName = UserManager.Instance.account.username;
        PhotonNetwork.JoinLobby();
    }
    // Will tell the MainMenuManager that this process is ready to go
    void OnJoinedLobby() {
        mainMenuManager.Ready();
        print("Joined Lobby");
        // AttemptRejoin(); // BROKEN!
    }

    // Will attempt to rejoin the server last played on
    void AttemptRejoin() {
        if (shouldAttemptRejoin) {
            shouldAttemptRejoin = false;
            isRejoined = true;
            print("attempting join");
            PhotonNetwork.ReJoinRoom(UserManager.Instance.account.lastGameID);
        }
    }

    // Called by UI buttons; will queue the player based on map name
    public void Play(string mapName) {
        gameHandler.currentMap = mapManager.GetMap(mapName);
        // Check to see if we are on the network and not already in a room
        if (PhotonNetwork.connected && PhotonNetwork.room == null) {
            // Find a room to join
            lobbyState = LobbyStates.searching;

            Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() {
                { "m", gameHandler.currentMap.name }
            };
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        }
    }

    // Called if a reconnection attempt failed
    void OnPhotonJoinRoomFailed() {
        isRejoined = false;
        print("OnPhotonJoinRoomFailed");
    }

    // Called if a room could not be found; will create a new one with the searching player as the host
    void OnPhotonRandomJoinFailed() {
        // Create a room named after the map the player wants to play
        // Timestamp source = https://forum.unity.com/threads/time-stamp-in-unity.396205/ ; accessed 3 January 2019
        // RoomOptions source = https://forum.photonengine.com/discussion/5575/pun-4-0-0-6-how-to-use-the-new-room-options ; accessed 3 January 2019
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = gameHandler.currentMap.maxPlayers;
        roomOptions.PlayerTtl = 0;
        roomOptions.EmptyRoomTtl = 0;
        roomOptions.CustomRoomPropertiesForLobby = new string[1] { "m" };
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() {
            { "m", gameHandler.currentMap.name }
        };
        //PhotonNetwork.autoCleanUpPlayerObjects = false;
        PhotonNetwork.CreateRoom(PhotonNetwork.player.NickName + System.DateTime.Now, roomOptions, null);
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

        if(isRejoined) {
            GameHandler.Instance.currentMap = MapManager.Instance.GetMap((string)PhotonNetwork.room.CustomProperties["m"]);
            isRejoined = false;
        }
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
                        StartCoroutine(UpdateGameID());
                        if(PhotonNetwork.isMasterClient) {
                            PhotonNetwork.room.IsVisible = false;
                        }
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

    IEnumerator UpdateGameID() {
        WWWForm form = new WWWForm();
        form.AddField("token", UserManager.Instance.account.sessionToken);
        form.AddField("id", PhotonNetwork.room.Name);

        UnityWebRequest www = UnityWebRequest.Post(setGameIdURL, form);
        yield return www.SendWebRequest();
    }
}
