using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChampionSelect : MonoBehaviour {

    [Header("General UI")]
    public GameObject champSelectUI;
    public Transform leftColumn;
    public GameObject playerPrefabLeft;
    public Transform rightColumn;
    public GameObject playerPrefabRight;
    public TMP_Text countdownTimer;

    [Header("ScrollView")]
    public Transform scrollViewContainer;
    public GameObject champRowPrefab;
    public GameObject champButtonPrefab;
    public int championsPerRow;

    int timeRemaining;
    bool waitingForTeams;
    ChampionRoster championRoster;

    void Start() {
        championRoster = GetComponent<ChampionRoster>();

        SetupChampions();
    }

    void Update() {
        if(waitingForTeams) {
            if(PhotonNetwork.playerList[PhotonNetwork.playerList.Length - 1].GetTeam() != PunTeams.Team.none) {
                waitingForTeams = false;
                DisplayPlayers();
            }
        }
    }

    void SetupChampions() {
        int i = 0;
        GameObject panel = new GameObject();
        foreach(Champion champion in championRoster.GetChampions()) {
            print(champion.championName);
            if(i % championsPerRow == 0)
                panel = Instantiate(champRowPrefab, scrollViewContainer);

            GameObject champ = Instantiate(champButtonPrefab, panel.transform);
            champ.name = champion.championName;

            Image image = champ.GetComponent<Image>();
            Button button = champ.GetComponent<Button>();
            button.interactable = champion.IsOwned;
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
        waitingForTeams = true;
        if(PhotonNetwork.isMasterClient) {
            AssignTeams();
        }
        champSelectUI.SetActive(true);
        timeRemaining = 60;
    }

    void DisplayPlayers() {
        foreach(PhotonPlayer player in PhotonNetwork.playerList) {
            GameObject container;
            if (player.GetTeam() == PunTeams.Team.blue) {
                container = Instantiate(playerPrefabLeft, leftColumn);
            } else {
                container = Instantiate(playerPrefabRight, rightColumn);
            }
            container.transform.Find("Username").GetComponent<TMP_Text>().text = player.NickName;
        }
    }

    public void OnStop() {
        champSelectUI.SetActive(false);
        StopCoroutine("Countdown");

        // Remove old UIs
        foreach(GameObject panel in leftColumn) {
            Destroy(panel);
        }
        foreach (GameObject panel in rightColumn) {
            Destroy(panel);
        }
    }

    IEnumerator Countdown() {
        while(true) {
            yield return new WaitForSeconds(1f);
            countdownTimer.GetComponent<SetText>().Set((timeRemaining--).ToString());
        }
    }
}
