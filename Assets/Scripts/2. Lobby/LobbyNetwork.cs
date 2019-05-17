using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable; // Source = http://forum.photonengine.com/discussion/3320/hashtable-tutorial ; accessed 3 January 2019
using UnityEngine.Networking;

/*
    The main script used to handle connections to and from the lobby
*/
/// <summary>
/// The main script used to handle connections to and from the lobby.
/// </summary>
public class LobbyNetwork : MonoBehaviour {

    // Public variables
    public static LobbyNetwork Instance;
    public string setGameIdURL;
    public Button cancelButton;

    // Private variables
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

    // Set listeners and assign private variables when the game starts.
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

    /// <summary>
    /// Attempts to connect the user to the Photon Network.
    /// </summary>
    /// <remarks>
    /// Called in the MainMenuManager Prepare() function once user is authenticated.
    /// </remarks>
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
        // AttemptRejoin(); // TODO: FIX; BROKEN!
    }

    // Will attempt to rejoin the server last played on
    // This method is OBSOLETE
    void AttemptRejoin() {
        if (shouldAttemptRejoin) {
            shouldAttemptRejoin = false;
            isRejoined = true;
            PhotonNetwork.ReJoinRoom(UserManager.Instance.account.lastGameID);
        }
    }

    /// <summary>
    /// Called by UI buttons; will queue the player based on the given map name
    /// </summary>
    /// <param name="mapName">The name of the map to put the player in</param>
    public void Play(string mapName) {
        gameHandler.currentMap = mapManager.GetMap(mapName);

        // Check to see if we are on the network and not already in a room
        if (PhotonNetwork.connected && PhotonNetwork.room == null) {

            // Find a room to join
            lobbyState = LobbyStates.searching;

            // Set room properties so that we can be found by other players looking for the same game (equivalent to matchmaking)
            Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() {
                { "m", gameHandler.currentMap.name }
            };
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        }
    }

    // Called if a reconnection attempt failed
    void OnPhotonJoinRoomFailed() {
        isRejoined = false;
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

    /// <summary>
    /// Called by UI to stop searching for a room to join
    /// </summary>
    public void StopPlay() {
        if (PhotonNetwork.connected && PhotonNetwork.room != null) {
            lobbyState = LobbyStates.none;
            PhotonNetwork.LeaveRoom();
            searchingLabel.OnSearchingStop();
            championSelect.OnStop();
        }
    }

    // Called by Photon when the user joins a room
    void OnJoinedRoom() {
        searchingLabel = SearchingLabel.Instance;
        searchingLabel.OnSearchingStart();

        // Obsolete: disconnect and reconnect handling
        if(isRejoined) {
            GameHandler.Instance.currentMap = MapManager.Instance.GetMap((string)PhotonNetwork.room.CustomProperties["m"]);
            isRejoined = false;
        }
    }

    // Called every frame; checks to see whether a room is full
    void Update() {
        if(PhotonNetwork.connected) {
            if(PhotonNetwork.room != null) {
                if (lobbyState == LobbyStates.searching) {
                    
                    // Wait for players
                    if (PhotonNetwork.room.PlayerCount == PhotonNetwork.room.MaxPlayers) {
                        searchingLabel.OnSearchingPause();
                        championSelect.OnStart();
                        lobbyState = LobbyStates.championSelect;
                        StartCoroutine(UpdateGameID());

                        // Host player to instantiate the map for all players in the room
                        if(PhotonNetwork.isMasterClient) {
                            PhotonNetwork.room.IsVisible = false;
                            PhotonNetwork.Instantiate(gameHandler.currentMap.map.name, gameHandler.currentMap.map.transform.position, gameHandler.currentMap.map.transform.rotation, 0);
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

    /// <summary>
    /// Sets the local player's current state in the lobby
    /// </summary>
    /// <param name="lobbyStates">The state to set the value to</param>
    public void SetLobbyState(LobbyStates lobbyStates) {
        lobbyState = lobbyStates;
    }

    // Sends a POST request to the database, updating the last game ID (redundant until match rejoining is fixed)
    IEnumerator UpdateGameID() {
        WWWForm form = new WWWForm();
        form.AddField("token", UserManager.Instance.account.sessionToken);
        form.AddField("id", PhotonNetwork.room.Name);

        UnityWebRequest www = UnityWebRequest.Post(setGameIdURL, form);
        yield return www.SendWebRequest();
    }
}
