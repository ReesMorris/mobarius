using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChampionSelect : MonoBehaviour {

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

    int timeRemaining;
    bool waitingForTeams;
    ChampionRoster championRoster;
    Champion selectedChampion;
    LobbyNetwork lobbyNetwork;
    PhotonView photonView;
    GameHandler gameHandler;
    UIHandler uiHandler;
    bool lockedIn;
    int playersReady;

    enum PlayerStates { picking, lockedIn, starting, started };
    PlayerStates playerState;

    void Start() {
        gameHandler = GetComponent<GameHandler>();
        championRoster = GetComponent<ChampionRoster>();
        lobbyNetwork = GetComponent<LobbyNetwork>();
        uiHandler = GetComponent<UIHandler>();
        photonView = GetComponent<PhotonView>();
        lockinButton.onClick.AddListener(LockChampion);

        SetupChampions();
    }

    void Update() {
        if(waitingForTeams) {
            if(PhotonNetwork.playerList[PhotonNetwork.playerList.Length - 1].GetTeam() != PunTeams.Team.none) {
                waitingForTeams = false;
            }
        }
    }

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

    void SetupChampions() {
        int i = 0;
        GameObject panel = new GameObject();
        foreach(Champion champion in championRoster.GetChampions()) {
            if(i % championsPerRow == 0)
                panel = Instantiate(champRowPrefab, scrollViewContainer);

            GameObject champ = Instantiate(champButtonPrefab, panel.transform);
            champ.name = champion.championName;

            Image image = champ.GetComponent<Image>();
            Button button = champ.GetComponent<Button>();
            button.interactable = champion.IsOwned;
            button.onClick.AddListener(delegate{SelectChampion(champion);});
            image.sprite = champion.icon;

            if(!champion.IsOwned) {
                image.color = new Color(1, 1, 1, 0.5f);
            }

            i++;
        }
    }

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

    public void OnStart() {
        playerState = PlayerStates.picking;
        playersReady = 0;
        waitingForTeams = true;
        if(PhotonNetwork.isMasterClient) {
            AssignTeams();
            DisplayPlayers();
        }
        champSelectUI.SetActive(true);
        SetTimeRemaining(timeToPick);
    }

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

    

    IEnumerator Countdown() {
        while(true) {
            yield return new WaitForSeconds(1f);
            countdownTimer.GetComponent<SetText>().Set((--timeRemaining).ToString());
            if (timeRemaining == -1) {
                countdownTimer.GetComponent<SetText>().Set("0");

                // Game is beginning
                if(playerState == PlayerStates.starting) {
                    gameHandler.SpawnAll();
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

    void LockChampion() {
        playerState = PlayerStates.lockedIn;
        titleText.text = LocalisationManager.instance.GetValue("champ_select_title_waiting");
        lockinButton.interactable = false;
        lockedIn = true;
        Transform container = GetLocalPlayerContainer();
        container.GetComponent<ChampionLock>().LockIn(selectedChampion);
    }

    Transform GetLocalPlayerContainer() {
        if(PhotonNetwork.player.GetTeam() == PunTeams.Team.blue)
            return leftColumn.Find(PhotonNetwork.playerName);
        return rightColumn.Find(PhotonNetwork.playerName);
    }

    void SetTimeRemaining(int amount) {
        timeRemaining = amount;
        countdownTimer.text = timeRemaining.ToString();
    }

    [PunRPC]
    void OnGameStart() {
        uiHandler.HideLobbyUI();
        playerState = PlayerStates.started;
    }
}
