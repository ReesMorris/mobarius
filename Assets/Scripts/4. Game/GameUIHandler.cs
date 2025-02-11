﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
    The script responsible for handling game UI elements (mainly for abilities)
*/
/// <summary>
/// The script responsible for handling game UI elements (mainly for abilities).
/// </summary>
public class GameUIHandler : MonoBehaviour {

    // Public variables
    public static GameUIHandler Instance;

    public delegate void OnGameTimeUpdate(int timeElapsed);
    public static OnGameTimeUpdate onGameTimeUpdate;
    public delegate void OnUpgradeButtonClicked(AbilityHandler.Abilities abilityKey);
    public static OnUpgradeButtonClicked onUpgradeButtonClicked;

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
    public Button abilityQUpgrade;
    public AbilityIcon abilityW;
    public Button abilityWUpgrade;
    public AbilityIcon abilityE;
    public Button abilityEUpgrade;
    public AbilityIcon abilityR;
    public Button abilityRUpgrade;
    public AbilityIcon abilityF;
    public AbilityIcon abilityG;
    public AbilityIcon abilityB;

    [Header("Level")]
    public Image characterIcon;
    public TMP_Text levelText;
    public Image levelFill;
    public TMP_Text levelUpText;

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

    // Private variables
    float cooldownQ;
    float cooldownQDuration;
    float cooldownW;
    float cooldownWDuration;
    float cooldownE;
    float cooldownEDuration;
    float cooldownR;
    float cooldownRDuration;
    float cooldownF;
    float cooldownFDuration;
    float cooldownG;
    float cooldownGDuration;
    float cooldownB;
    float cooldownBDuration;
    PhotonView photonView;
    AbilityHandler abilityHandler;

    bool gameEnded;

    // Allows the script to be accessed by other players, called at the start of the game.
    void Awake() {
        Instance = this;
    }

    // Set up listeners and references at the start of the game.
    void Start() {
        GameHandler.onGameStart += OnGameStart;
        GameHandler.onGameEnd += OnGameEnd;
        ChampionXP.onChampionLevelUp += OnChampionLevelUp;
        ChampionXP.onChampionReceiveXP += OnChampionReceiveXP;
        ChampionXP.onChampionUpgradeAbility += OnChampionUpgradeAbility;
        abilityHandler = GetComponent<AbilityHandler>();
        SetupUpgradeListeners();
        photonView = GetComponent<PhotonView>();
        StartCoroutine("HandleCooldowns");
    }

    // Set up listeners to listen for ability upgrade buttons being clicked
    void SetupUpgradeListeners() {
        abilityQUpgrade.onClick.AddListener(delegate { UpgradeButtonClicked(AbilityHandler.Abilities.Q); });
        abilityWUpgrade.onClick.AddListener(delegate { UpgradeButtonClicked(AbilityHandler.Abilities.W); });
        abilityEUpgrade.onClick.AddListener(delegate { UpgradeButtonClicked(AbilityHandler.Abilities.E); });
        abilityRUpgrade.onClick.AddListener(delegate { UpgradeButtonClicked(AbilityHandler.Abilities.R); });
    }

    /// <summary>
    /// Called when a champion's abilities are upgraded to update the tooltip UI.
    /// </summary>
    /// <param name="champion">The Champion of the player.</param>
    public void UpdateAbilities(Champion champion) {
        abilityPassive.SetupIcon(abilityHandler.GetChampionAbility(champion, AbilityHandler.Abilities.Passive), "", champion);
        abilityQ.SetupIcon(abilityHandler.GetChampionAbility(champion, AbilityHandler.Abilities.Q), "Q", champion);
        abilityW.SetupIcon(abilityHandler.GetChampionAbility(champion, AbilityHandler.Abilities.W), "W", champion);
        abilityE.SetupIcon(abilityHandler.GetChampionAbility(champion, AbilityHandler.Abilities.E), "E", champion);
        abilityR.SetupIcon(abilityHandler.GetChampionAbility(champion, AbilityHandler.Abilities.R), "R", champion);
        abilityF.SetupIcon(abilityHandler.GetChampionAbility(champion, AbilityHandler.Abilities.F), "F", champion);
        abilityG.SetupIcon(abilityHandler.GetChampionAbility(champion, AbilityHandler.Abilities.G), "G", champion);
        abilityB.SetupIcon(abilityHandler.GetChampionAbility(champion, AbilityHandler.Abilities.B), "B", champion);
    }

    // Called when the upgrade button for an ability is clicked
    void UpgradeButtonClicked(AbilityHandler.Abilities abilityKey) {
        if (onUpgradeButtonClicked != null)
            onUpgradeButtonClicked(abilityKey);
    }

    /// <summary>
    /// Updates the health bar and mana bar UI elements above the character.
    /// </summary>
    /// <param name="champion">The Champion of the player.</param>
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

    /// <summary>
    /// Called when a champion's abilities are upgraded to update the tooltip UI.
    /// </summary>
    /// <param name="hotkey">The keycode for the ability (Q,W,E..)</param>
    /// <param name="ability">The Ability attempting to be cast</param>
    /// <param name="champion">The Champion attempting to cast the ability</param>
    /// <returns>
    /// True if the user can cast a specific ability; false if not.
    /// </returns>
    public bool CanCastAbility(AbilityHandler.Abilities hotkey, Ability ability, Champion champion) {

        // Conditions in which the return is instantly false
        if (champion.movementSpeed == 0)
            return false;
        if (gameEnded)
            return false;
        if (abilityHandler.GetAbilityLevel(champion, ability) == 0)
            return false;
        if (champion.mana < ability.cost)
            return false;
        if (ChatHandler.Instance.inputField.gameObject.activeSelf)
            return false;

        // Return whether the cooldown for the ability has ended
        switch (hotkey) {
            case AbilityHandler.Abilities.Q:
                return cooldownQ == 0f;
            case AbilityHandler.Abilities.W:
                return cooldownW == 0f;
            case AbilityHandler.Abilities.E:
                return cooldownE == 0f;
            case AbilityHandler.Abilities.R:
                return cooldownR == 0f;
            case AbilityHandler.Abilities.F:
                return cooldownF == 0f;
            case AbilityHandler.Abilities.G:
                return cooldownG == 0f;
            case AbilityHandler.Abilities.B:
                return cooldownB == 0f;
        }

        // Something went wrong, don't cast whatever it is
        return false;
    }

    /// <summary>
    /// Called when an ability has just been cast by the local player's Champion.
    /// </summary>
    /// <param name="ability">The Ability that was just cast</param>
    /// <param name="cooldown">The cooldown of the ability</param>
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
            case AbilityHandler.Abilities.F:
                cooldownF = cooldownFDuration = cooldown;
                break;
            case AbilityHandler.Abilities.G:
                cooldownG = cooldownGDuration = cooldown;
                break;
            case AbilityHandler.Abilities.B:
                cooldownB = cooldownBDuration = cooldown;
                break;
        }
    }

    // Runs every 0.1 seconds to update cooldown timer text
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
            if (cooldownF > 0f) {
                cooldownF = Mathf.Max(0f, cooldownF - speed);
                abilityF.SetCooldown(cooldownF, cooldownFDuration);
            }
            if (cooldownG > 0f) {
                cooldownG = Mathf.Max(0f, cooldownG - speed);
                abilityG.SetCooldown(cooldownG, cooldownGDuration);
            }
            if (cooldownB > 0f) {
                cooldownB = Mathf.Max(0f, cooldownB - speed);
                abilityB.SetCooldown(cooldownB, cooldownBDuration);
            }
            yield return new WaitForSeconds(speed);
        }
    }

    /* Game Timer */

    /// <summary>
    /// The master client starts the countdown timer to be synced across the network.
    /// </summary>
    public void StartGameTimer() {
        if (PhotonNetwork.isMasterClient)
            StartCoroutine("Timer");
    }

    // The master client updating the game timer every 1 second
    IEnumerator Timer() {
        while(true) {
            yield return new WaitForSeconds(1f);
            TimeElapsed++;
            photonView.RPC("UpdateGameTimer", PhotonTargets.All, TimeElapsed);
        }
    }

    // The network call to update the game timer for all clients in sync
    [PunRPC]
    public void UpdateGameTimer(int timeElapsed) {
        float mins = Mathf.Floor(timeElapsed / 60f);
        float secs = timeElapsed % 60;
        gameTimer.text = mins.ToString("00") + ":" + secs.ToString("00");

        // Play a sound effect after 15 seconds
        if (mins == 0f && secs == 15f) {
            SoundManager.Instance.PlaySound("Announcer/WelcomeToSummonersRift");
            ShowPlayerText("Welcome to Summoners Rift");
        }
        onGameTimeUpdate(timeElapsed);
    }

    /* Display Text */

    /// <summary>
    /// Displays a message to the local player for 3 seconds.
    /// </summary>
    /// <param name="message">The message shown to the player</param>
    /// <remarks>
    /// The message is not localised.
    /// </remarks>
    public void ShowPlayerText(string message) {
        StartCoroutine(ShowText(message));
    }

    // The timer to show the message to the player
    IEnumerator ShowText(string message) {
        displayText.text = message;
        yield return new WaitForSeconds(3f);
        displayText.text = "";
    }

    /* Text & Sound & Kill Message */

    /// <summary>
    /// Displays a kill message to the local player and plays a sound effect.
    /// </summary>
    /// <param name="sound">The sound played globally</param>
    /// <param name="displayText">The message shown to the player</param>
    /// <remarks>
    /// The message is not localised.
    /// </remarks>
    public void KillMessageWithSound(string sound, string displayText) {
        SoundManager.Instance.PlaySound(sound);
        ShowPlayerText(displayText);
    }

    /// <summary>
    /// Displays a message to the local player and plays a sound effect.
    /// </summary>
    /// <param name="sound">The sound played globally</param>
    /// <param name="displayText">The message shown to the player</param>
    /// <remarks>
    /// The message is not localised.
    /// </remarks>
    public void MessageWithSound(string sound, string displayText) {
        SoundManager.Instance.PlaySound(sound);
        ShowPlayerText(displayText);
    }

    /* Game Start and End */
    void OnGameStart() {
        gameEnded = false;
        TimeElapsed = 0;
        gameTimer.text = "00:00";
    }
    void OnGameEnd() {
        StopCoroutine("Timer");
        gameEnded = true;
    }

    /* Set Character Icon */

    /// <summary>
    /// Sets the character icon sprite of the Champion.
    /// </summary>
    /// <param name="champion">The Champion the sprite will be obtained from</param>
    public void SetCharacterIcon(Champion champion) {
        characterIcon.sprite = champion.icon;
    }

    /* XP Systems */

    // Listener for when a champion receives XP
    void OnChampionReceiveXP(PhotonPlayer player, float progress) {
        if (PhotonNetwork.player == player) {
            levelFill.fillAmount = progress;
        }
    }

    // Listener for when a champion upgrades an ability
    void OnChampionUpgradeAbility(PhotonPlayer player, Champion champion, int unclaimedUpgrades) {
        if (PhotonNetwork.player == player) {
            SetLevelUpText(unclaimedUpgrades);
            UpdateAbilities(champion);
        }
    }

    // Listener for when a champion levels up
    void OnChampionLevelUp(Champion champion, PhotonPlayer player, int level, int unclaimedUpgrades) {
        if (PhotonNetwork.player == player) {
            levelText.text = level.ToString();
            SetLevelUpText(unclaimedUpgrades);
            UpdateAbilities(champion);

            if (unclaimedUpgrades > 0) {
                abilityQUpgrade.gameObject.SetActive(true);
                abilityWUpgrade.gameObject.SetActive(true);
                abilityEUpgrade.gameObject.SetActive(true);
                abilityRUpgrade.gameObject.SetActive(true);
            }
        }
    }

    // Updates the 'level up' text on the local player's XP bar
    void SetLevelUpText(int unclaimedUpgrades) {
        if (unclaimedUpgrades > 0) {
            levelUpText.text = "LEVEL UP! +" + unclaimedUpgrades;
            levelUpText.gameObject.SetActive(true);
        } else {
            levelUpText.gameObject.SetActive(false);
            abilityQUpgrade.gameObject.SetActive(false);
            abilityWUpgrade.gameObject.SetActive(false);
            abilityEUpgrade.gameObject.SetActive(false);
            abilityRUpgrade.gameObject.SetActive(false);
        }
    }
}
