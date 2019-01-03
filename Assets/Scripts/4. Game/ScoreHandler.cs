using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreHandler : MonoBehaviour {

	public static ScoreHandler Instance;
    public TMP_Text scoreUI;
    public TMP_Text kdaUI;
    public TMP_Text minionKillsUI;

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
        scoreUI.text = "<b><color=#00b1fd>" + blueScore + "</color></b>vs<b><color=#fd002a>" + redScore + "</color></b>";
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
