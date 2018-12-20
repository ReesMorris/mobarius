using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUIHandler : MonoBehaviour {

    public static GameUIHandler Instance;

    [Header("Abilities")]
    public AbilityIcon abilityPassive;
    public AbilityIcon abilityQ;
    public AbilityIcon abilityW;
    public AbilityIcon abilityE;
    public AbilityIcon abilityR;

    [Header("Stats")]
    public Image healthBar;
    public TMP_Text healthText;
    public TMP_Text healthRegenText;
    public Image manaBar;
    public TMP_Text manaText;
    public TMP_Text manaRegenText;

    float cooldownQ;
    float cooldownQDuration;
    float cooldownW;
    float cooldownWDuration;
    float cooldownE;
    float cooldownEDuration;
    float cooldownR;
    float cooldownRDuration;

    bool ready;

    void Start() {
        Instance = this;
        StartCoroutine("HandleCooldowns");
    }

    public void UpdateAbilities(string championName) {
        Champion champion = ChampionRoster.Instance.GetChampion(championName);

        // First time running this? Let's load the icons
        if (!ready) {
            abilityPassive.SetupIcon(champion.abilityPassive, "");
            abilityQ.SetupIcon(champion.abilityQ, "Q");
            abilityW.SetupIcon(champion.abilityW, "W");
            abilityE.SetupIcon(champion.abilityE, "E");
            abilityR.SetupIcon(champion.abilityR, "R");
            ready = true;
        }
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
        manaText.text = champion.mana.ToString("F1") + " / " + champion.maxMana;
        if (champion.mana < champion.maxMana)
            manaRegenText.text = "+" + champion.manaRegen.ToString("F1");
    }

    public bool CanCastAbility(AbilityHandler.Abilities ability) {
        switch (ability) {
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

}
