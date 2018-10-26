using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChampionSelect : MonoBehaviour {

    public GameObject champSelectUI;
    public Transform leftColumn;
    public GameObject playerPrefabLeft;
    public Transform rightColumn;
    public GameObject playerPrefabRight;
    public TMP_Text countdownTimer;

    int timeRemaining;
    bool waitingForTeams;

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

    void Update() {
        if(waitingForTeams) {
            if(PhotonNetwork.playerList[PhotonNetwork.playerList.Length - 1].GetTeam() != PunTeams.Team.none) {
                waitingForTeams = false;
                DisplayPlayers();
            }
        }
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
