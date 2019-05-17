using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


/*
    The script responsible for the champion selection UI
*/
/// <summary>
/// The script responsible for the champion selection UI.
/// </summary>
public class ChampionSelect : MonoBehaviour {

    // Public variables
    public int timeToPick;
    public int timeBeforeStart;

    [Header("General UI")]
    public GameObject champSelectUI;
    public Transform leftColumn;
    public GameObject playerPrefabLeft;
    public Transform rightColumn;
    public GameObject playerPrefabRight;
    public TMP_Text countdownTimer;
    public Button lockinButton;
    public TMP_Text titleText;

    [Header("ScrollView")]
    public Transform scrollViewContainer;
    public GameObject champRowPrefab;
    public GameObject champButtonPrefab;
    public int championsPerRow;

    // Private variables
    int timeRemaining;
    bool waitingForTeams;
    ChampionRoster championRoster;
    Champion selectedChampion;
    LobbyNetwork lobbyNetwork;
    PhotonView photonView;
    GameHandler gameHandler;
    bool lockedIn;
    int playersReady;

    enum PlayerStates { picking, lockedIn, starting, started };
    PlayerStates playerState;

    // Set references for private variables when the game starts
    void Start() {
        gameHandler = GetComponent<GameHandler>();
        championRoster = GetComponent<ChampionRoster>();
        lobbyNetwork = GetComponent<LobbyNetwork>();
        photonView = GetComponent<PhotonView>();
        lockinButton.onClick.AddListener(LockChampion);
        SetupChampions();
    }

    // Check every frame to see if all teams have been assigned (to display the UI)
    void Update() {
        if(waitingForTeams) {
            if(PhotonNetwork.playerList[PhotonNetwork.playerList.Length - 1].GetTeam() != PunTeams.Team.none) {
                waitingForTeams = false;
            }
        }
    }

    /// <summary>
    /// Called when a player locks in their character choice.
    /// Commences the countdown timer if all players have locked in.
    /// </summary>
    public void OnPlayerLock() {
        playersReady++;

        if(playersReady == PhotonNetwork.room.MaxPlayers) {
            playerState = PlayerStates.starting;
            titleText.text = LocalisationManager.instance.GetValue("champ_select_title_starting");
            SetTimeRemaining(timeBeforeStart);
            if (PhotonNetwork.isMasterClient) {
                StopCoroutine("Countdown");
                StartCoroutine("Countdown");
            }
        }
    }

    // Displays all available champions to be selected
    void SetupChampions() {
        int i = 0;
        GameObject panel = new GameObject();

        // Loop through every champion available
        foreach(Champion champion in championRoster.GetChampions()) {
            if(i % championsPerRow == 0)
                panel = Instantiate(champRowPrefab, scrollViewContainer);

            GameObject champ = Instantiate(champButtonPrefab, panel.transform);
            champ.name = champion.championName;

            // Update the UI and add event listeners
            Image image = champ.GetComponent<Image>();
            Button button = champ.GetComponent<Button>();
            button.interactable = champion.IsOwned;
            button.onClick.AddListener(delegate{SelectChampion(champion);});
            image.sprite = champion.icon;

            // Make unowned champions transparent
            if(!champion.IsOwned) {
                image.color = new Color(1, 1, 1, 0.5f);
            }

            i++;
        }
    }

    // The master client will assign teams to users
    void AssignTeams() {
        for (int i = 0; i < PhotonNetwork.playerList.Length; i++) {
            if(i % 2 == 0) {
                PhotonNetwork.playerList[i].SetTeam(PunTeams.Team.red);
            } else {
                PhotonNetwork.playerList[i].SetTeam(PunTeams.Team.blue);
            }
        }
        StartCoroutine("Countdown");
    }

    /// <summary>
    /// Resets variables and requests the master client to begin assigning teams.
    /// </summary>
    /// <remarks>
    /// Called when all players are moved to the champion select screen.
    /// </remarks>
    public void OnStart() {
        lockedIn = false;
        playerState = PlayerStates.picking;
        playersReady = 0;
        waitingForTeams = true;
        if (PhotonNetwork.isMasterClient) {
            AssignTeams();
            DisplayPlayers();
        }
        champSelectUI.SetActive(true);
        SetTimeRemaining(timeToPick);
    }

    // Displays players on either the left or the right sidebar
    void DisplayPlayers() {
        foreach(PhotonPlayer player in PhotonNetwork.playerList) {
            GameObject container;
            if (player.GetTeam() == PunTeams.Team.blue) {
                container = PhotonNetwork.Instantiate(playerPrefabLeft.name, Vector3.zero, Quaternion.identity, 0) as GameObject;
                container.GetComponent<ChampionLock>().SetPosition(true, player.NickName);
            } else {
                container = PhotonNetwork.Instantiate(playerPrefabRight.name, Vector3.zero, Quaternion.identity, 0) as GameObject;
                container.GetComponent<ChampionLock>().SetPosition(false, player.NickName);
            }
        }
    }

    /// <summary>
    /// Hides the champion selection screen and resets the UI.
    /// </summary>
    /// <remarks>
    /// Called when a user disconnects from the room.
    /// </remarks>
    public void OnStop() {
        champSelectUI.SetActive(false);
        StopCoroutine("Countdown");

        // Remove old UIs
        foreach(Transform panel in leftColumn.transform) {
            Destroy(panel.gameObject);
        }
        foreach (Transform panel in rightColumn.transform) {
            Destroy(panel.gameObject);
        }
    }

    
    // The countdown timer displayed for picking champions and waiting for a game to begin
    IEnumerator Countdown() {
        while(true) {
            yield return new WaitForSeconds(1f);
            countdownTimer.GetComponent<SetText>().Set((--timeRemaining).ToString());
            if (timeRemaining == -1) {
                countdownTimer.GetComponent<SetText>().Set("0");

                // Game is beginning
                if(playerState == PlayerStates.starting) {
                    gameHandler.StartGame();
                    photonView.RPC("OnGameStart", PhotonTargets.All);
                }

                // At least one player has not picked and everyone needs to go back to the lobby
                else {
                    if (playerState == PlayerStates.picking) {
                        // Player did not pick; do not put them back into searching queue
                        lobbyNetwork.StopPlay();
                    }
                    else {
                        // Player picked; put them back into searching queue
                        lobbyNetwork.SetLobbyState(LobbyNetwork.LobbyStates.searching);
                    }
                }
                break;
            } else {

            }
        }
    }

    // Called when the local player clicks on the button UI of a champion
    void SelectChampion(Champion champion) {
        if(!lockedIn) {
            Transform container = GetLocalPlayerContainer();
            container.Find("Image").GetComponent<Image>().sprite = champion.icon;

            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
            props.Add("championName", champion.championName);
            PhotonNetwork.player.SetCustomProperties(props);

            lockinButton.interactable = true;
            selectedChampion = champion;
        }
    }

    // Called when the local player clicks on the 'lock in' button
    void LockChampion() {
        playerState = PlayerStates.lockedIn;
        titleText.text = LocalisationManager.instance.GetValue("champ_select_title_waiting");
        lockinButton.interactable = false;
        lockedIn = true;
        Transform container = GetLocalPlayerContainer();
        container.GetComponent<ChampionLock>().LockIn(selectedChampion);
    }

    // Returns the UI element transform of the container for the local player
    Transform GetLocalPlayerContainer() {
        if(PhotonNetwork.player.GetTeam() == PunTeams.Team.blue)
            return leftColumn.Find(PhotonNetwork.playerName);
        return rightColumn.Find(PhotonNetwork.playerName);
    }

    // Updates the timer to the time remaining, set by the master client
    void SetTimeRemaining(int amount) {
        timeRemaining = amount;
        countdownTimer.text = timeRemaining.ToString();
    }

    // Called to all clients by the master client when the game starts
    [PunRPC]
    void OnGameStart() {
        UIHandler.Instance.HideLobbyUI();
        playerState = PlayerStates.started;
        champSelectUI.SetActive(false);
    }
}
