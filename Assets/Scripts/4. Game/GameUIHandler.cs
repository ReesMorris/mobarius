using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUIHandler : MonoBehaviour {

    public static GameUIHandler Instance;

    public delegate void OnGameTimeUpdate(int timeElapsed);
    public static OnGameTimeUpdate onGameTimeUpdate;

    [Header("Generic")]
    public Color allyHealthColour;
    public Color enemyHealthColour;

    [Header("Top Bar")]
    public TMP_Text gameTimer;
    public TMP_Text minionKills;
    public TMP_Text kda;
    public TMP_Text score;

    [Header("Abilities")]
    public AbilityIcon abilityPassive;
    public AbilityIcon abilityQ;
    public AbilityIcon abilityW;
    public AbilityIcon abilityE;
    public AbilityIcon abilityR;

    [Header("Display Text")]
    public TMP_Text displayText;

    [Header("Stats")]
    public GameObject deathBar;
    public TMP_Text deathBarText;
    public Image healthBar;
    public TMP_Text healthText;
    public TMP_Text healthRegenText;
    public Image manaBar;
    public TMP_Text manaText;
    public TMP_Text manaRegenText;

    public int TimeElapsed { get; protected set; }

    float cooldownQ;
    float cooldownQDuration;
    float cooldownW;
    float cooldownWDuration;
    float cooldownE;
    float cooldownEDuration;
    float cooldownR;
    float cooldownRDuration;
    PhotonView photonView;

    bool gameEnded;

    void Start() {
        Instance = this;
        GameHandler.onGameStart += OnGameStart;
        GameHandler.onGameEnd += OnGameEnd;
        photonView = GetComponent<PhotonView>();
        StartCoroutine("HandleCooldowns");
    }

    public void UpdateAbilities(Champion champion) {
        Champion template = ChampionRoster.Instance.GetChampion(champion.championName);
        abilityPassive.SetupIcon(AbilityHandler.Instance.GetChampionAbilities(champion.championName, AbilityHandler.Abilities.Passive), "", champion);
        abilityQ.SetupIcon(AbilityHandler.Instance.GetChampionAbilities(champion.championName, AbilityHandler.Abilities.Q), "Q", champion);
        abilityW.SetupIcon(AbilityHandler.Instance.GetChampionAbilities(champion.championName, AbilityHandler.Abilities.W), "W", champion);
        abilityE.SetupIcon(AbilityHandler.Instance.GetChampionAbilities(champion.championName, AbilityHandler.Abilities.E), "E", champion);
        abilityR.SetupIcon(AbilityHandler.Instance.GetChampionAbilities(champion.championName, AbilityHandler.Abilities.R), "R", champion);
    }

    public void UpdateStats(Champion champion) {
        // Health
        healthRegenText.text = "";
        healthBar.fillAmount = champion.health / champion.maxHealth;
        healthText.text = champion.health.ToString("F1") + " / " + champion.maxHealth;
        if (champion.health < champion.maxHealth)
            healthRegenText.text = "+" + champion.healthRegen.ToString("F1");

        // Mana
        manaRegenText.text = "";
        manaBar.fillAmount = champion.mana / champion.maxMana;
        manaText.text = champion.mana.ToString("F1") + " / " + champion.maxMana.ToString("F1");
        if (champion.mana < champion.maxMana)
            manaRegenText.text = "+" + champion.manaRegen.ToString("F1");
    }

    public bool CanCastAbility(AbilityHandler.Abilities hotkey, Ability ability, Champion champion) {
        if (gameEnded)
            return false;
        if (champion.mana < ability.cost)
            return false;
        switch (hotkey) {
            case AbilityHandler.Abilities.Q:
                return cooldownQ == 0f;
            case AbilityHandler.Abilities.W:
                return cooldownW == 0f;
            case AbilityHandler.Abilities.E:
                return cooldownE == 0f;
            case AbilityHandler.Abilities.R:
                return cooldownR == 0f;
        }
        return false;
    }

    public void OnAbilityCasted(AbilityHandler.Abilities ability, float cooldown) {
        switch (ability) {
            case AbilityHandler.Abilities.Q:
                cooldownQ = cooldownQDuration = cooldown;
                break;
            case AbilityHandler.Abilities.W:
                cooldownW = cooldownWDuration = cooldown;
                break;
            case AbilityHandler.Abilities.E:
                cooldownE = cooldownEDuration = cooldown;
                break;
            case AbilityHandler.Abilities.R:
                cooldownR = cooldownRDuration = cooldown;
                break;
        }
    }

    IEnumerator HandleCooldowns() {
        float speed = 0.1f;
        while(true) {
            if(cooldownQ > 0f) {
                cooldownQ = Mathf.Max(0f, cooldownQ - speed);
                abilityQ.SetCooldown(cooldownQ, cooldownQDuration);
            }
            if (cooldownW > 0f) {
                cooldownW = Mathf.Max(0f, cooldownW - speed);
                abilityW.SetCooldown(cooldownW, cooldownWDuration);
            }
            if (cooldownE > 0f) {
                cooldownE = Mathf.Max(0f, cooldownE - speed);
                abilityE.SetCooldown(cooldownE, cooldownEDuration);
            }
            if (cooldownR > 0f) {
                cooldownR = Mathf.Max(0f, cooldownR - speed);
                abilityR.SetCooldown(cooldownR, cooldownRDuration);
            }
            yield return new WaitForSeconds(speed);
        }
    }

    /* Game Timer */

    public void StartGameTimer() {
        if (PhotonNetwork.isMasterClient)
            StartCoroutine("Timer");
    }
    IEnumerator Timer() {
        while(true) {
            yield return new WaitForSeconds(1f);
            TimeElapsed++;
            photonView.RPC("UpdateGameTimer", PhotonTargets.All, TimeElapsed);
        }
    }
    [PunRPC]
    public void UpdateGameTimer(int timeElapsed) {
        float mins = Mathf.Floor(timeElapsed / 60f);
        float secs = timeElapsed % 60;
        gameTimer.text = mins.ToString("00") + ":" + secs.ToString("00");

        if (mins == 0f && secs == 15f) {
            SoundManager.Instance.PlaySound("Announcer/WelcomeToSummonersRift");
            ShowPlayerText("Welcome to Summoners Rift");
        }
        onGameTimeUpdate(timeElapsed);
    }

    /* Display Text */

    public void ShowPlayerText(string message) {
        StartCoroutine(ShowText(message));
    }

    IEnumerator ShowText(string message) {
        displayText.text = message;
        yield return new WaitForSeconds(3f);
        displayText.text = "";
    }

    /* Text & Sound & Kill Message */
    public void KillMessageWithSound(string sound, string displayText) {
        SoundManager.Instance.PlaySound(sound);
        ShowPlayerText(displayText);
    }
    public void MessageWithSound(string sound, string displayText) {
        SoundManager.Instance.PlaySound(sound);
        ShowPlayerText(displayText);
    }

    /* Game Start and End */
    void OnGameStart() {
        gameEnded = false;
    }
    void OnGameEnd() {
        StopCoroutine("Timer");
        gameEnded = true;
    }
}
