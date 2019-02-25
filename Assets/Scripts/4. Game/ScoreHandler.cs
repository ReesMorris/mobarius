using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreHandler : MonoBehaviour {

	public static ScoreHandler Instance;
    public TMP_Text scoreUI1;
    public TMP_Text scoreUI2;
    public TMP_Text kdaUI;
    public TMP_Text minionKillsUI;
    public string friendlyColour = "#00b1fd";
    public string enemyColour = "#fd002a";

    PhotonView photonView;
    int blueScore;
    int redScore;
    int kills;
    int deaths;
    int assists;
    int minionKills;

    void Start() {
        Instance = this;
        photonView = GetComponent<PhotonView>();
        GameHandler.onGameStart += UpdateUI;
        GameHandler.onGameStart += UpdateKdaUI;
    }

    public void IncreaseScore(PunTeams.Team team) {
        if(PhotonNetwork.isMasterClient) {
            if (team == PunTeams.Team.red)
                redScore++;
            else if (team == PunTeams.Team.blue)
                blueScore++;
            UpdateUI();
        }
    }

    void UpdateUI() {
        if(PhotonNetwork.isMasterClient) {
            photonView.RPC("UpdateScoreUI", PhotonTargets.All, redScore, blueScore);
        }
    }

    [PunRPC]
    void UpdateScoreUI(int _redScore, int _blueScore) {
        redScore = _redScore;
        blueScore = _blueScore;

        // Now we have to depend on the teams here
        // BLUE team always goes first, but the COLOUR of that score actually depends on whether the player is on the same team
        // Therefore, BLUE score shows first but will be coloured RED if the player is on the RED team
        if (PhotonNetwork.player.GetTeam() == PunTeams.Team.blue) {
            scoreUI1.text = "<b><color=" + friendlyColour + ">" + blueScore + "</color></b>";
            scoreUI2.text = "<b><color=" + enemyColour + ">" + redScore + "</color></b>";
        } else {
            scoreUI1.text = "<b><color=" + enemyColour + ">" + blueScore + "</color></b>";
            scoreUI2.text = "<b><color=" + friendlyColour + ">" + redScore + "</color></b>";
        }
    }

    public bool IsFirstBlood() {
        return blueScore == 0 && redScore == 0;
    }

    /* KDA */

    public void OnKill() {
        kills++;
        UpdateKdaUI();
    }
    public void OnDeath() {
        deaths++;
        UpdateKdaUI();
    }
    public void OnAssist() {
        assists++;
        UpdateKdaUI();
    }
    public void UpdateKdaUI() {
        kdaUI.text = kills + "/" + deaths + "/" + assists;
    }
    public void OnMinionKill() {
        minionKills++;
        minionKillsUI.text = minionKills.ToString();
    }
}
