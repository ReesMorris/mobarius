using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
    This script handles all upper-right UI elements
*/
/// <summary>
/// This script handles all upper-right UI elements.
/// </summary>
public class ScoreHandler : MonoBehaviour {

    // Public variables
	public static ScoreHandler Instance;
    public TMP_Text scoreUI1;
    public TMP_Text scoreUI2;
    public TMP_Text kdaUI;
    public TMP_Text minionKillsUI;
    public Image pingImage;
    public Sprite[] pingStates;
    public TMP_Text pingText;
    public TMP_Text fpsText;
    public string friendlyColour = "#00b1fd";
    public string enemyColour = "#fd002a";

    // Private variables
    PhotonView photonView;
    int blueScore;
    int redScore;
    int kills;
    int deaths;
    int assists;
    int minionKills;
    bool started;

    float updateInterval = 0.5F;
    float accum = 0;
    int frames = 0;
    float timeleft;

    // Assign event listeners when the game starts.
    void Start() {
        Instance = this;
        photonView = GetComponent<PhotonView>();
        GameHandler.onGameStart += UpdateUI;
        GameHandler.onGameStart += UpdateKdaUI;
        GameHandler.onGameStart += OnGameStart;
        GameHandler.onGameEnd += OnGameEnd;
    }

    // When the game begins, reset all variables (which may be from previous games)
    void OnGameStart() {
        started = true;
        redScore = 0;
        blueScore = 0;
        kills = 0;
        deaths = 0;
        assists = 0;
        minionKills = 0;
        UpdateUI();
        UpdateKdaUI();
        minionKillsUI.text = "0";
        StartCoroutine(PingEnumerator());
    }

    // When the game ends, clear some variables
    void OnGameEnd() {
        started = false;
        accum = 0;
        frames = 0;
        timeleft = 0;
    }

    // Update frame rate whilst the game is running
    void Update() {
        if (started)
            UpdateFrames();
    }

    // Update the player ping measurement every 2 seconds
    IEnumerator PingEnumerator() {
        while (started) {
            UpdatePing();
            yield return new WaitForSeconds(2f);
        }
    }

    // Updates the player's ping UI
    void UpdatePing() {
        float ping = PhotonNetwork.networkingPeer.RoundTripTime;
        pingText.text = ping + " ms";
        if (ping < 60)
            pingImage.sprite = pingStates[0];
        else if (ping < 120)
            pingImage.sprite = pingStates[1];
        else if (ping < 180)
            pingImage.sprite = pingStates[2];
        else
            pingImage.sprite = pingStates[3];
    }

    // SRC: https://wiki.unity3d.com/index.php/FramesPerSecond [Accessed: 13 March 2019]
    // Updates the frame rate UI
    void UpdateFrames() {
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;
        if (timeleft <= 0.0) {
            float fps = accum / frames;
            fpsText.text = "FPS: " + Mathf.Floor(fps);
            timeleft = updateInterval;
            accum = 0.0F;
            frames = 0;
        }
    }

    /// <summary>
    /// Increases the score for a specified team.
    /// </summary>
    /// <param name="team">The team to increase the score for</param>
    public void IncreaseScore(PunTeams.Team team) {
        if(PhotonNetwork.isMasterClient) {
            if (team == PunTeams.Team.red)
                redScore++;
            else if (team == PunTeams.Team.blue)
                blueScore++;
            UpdateUI();
        }
    }

    // Updates the UI across the network
    void UpdateUI() {
        if(PhotonNetwork.isMasterClient) {
            photonView.RPC("UpdateScoreUI", PhotonTargets.All, redScore, blueScore);
        }
    }

    // Updates the UI across the network
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

    /// <summary>
    /// Returns true if no kills have happened yet, false if not.
    /// </summary>
    public bool IsFirstBlood() {
        return blueScore == 0 && redScore == 0;
    }

    /* KDA */

    /// <summary>
    /// Increases amount of local player kills by 1.
    /// </summary>
    public void OnKill() {
        kills++;
        UpdateKdaUI();
    }

    /// <summary>
    /// Increases amount of local player deaths by 1.
    /// </summary>
    public void OnDeath() {
        deaths++;
        UpdateKdaUI();
    }

    /// <summary>
    /// Increases amount of local player assists by 1.
    /// </summary>
    public void OnAssist() {
        assists++;
        UpdateKdaUI();
    }

    /// <summary>
    /// Updates local player KDA UI.
    /// </summary>
    public void UpdateKdaUI() {
        kdaUI.text = kills + "/" + deaths + "/" + assists;
    }

    /// <summary>
    /// Increases amount of local player minion kills by 1.
    /// </summary>
    public void OnMinionKill() {
        minionKills++;
        minionKillsUI.text = minionKills.ToString();
    }
}
