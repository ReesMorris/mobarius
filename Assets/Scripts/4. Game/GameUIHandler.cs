using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUIHandler : MonoBehaviour {

    [Header("Abilities")]
	public GameObject statsBox;

    [Header("Stats")]
    public Image healthBar;
    public TMP_Text healthText;
    public TMP_Text healthRegenText;
    public Image manaBar;
    public TMP_Text manaText;
    public TMP_Text manaRegenText;

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
}
