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
    public Image manaBar;
    public TMP_Text manaText;

    public void UpdateStats(Champion champion) {
        healthBar.fillAmount = champion.health / champion.maxHealth;
        healthText.text = champion.health + " / " + champion.maxHealth;
        manaBar.fillAmount = champion.mana / champion.maxMana;
        manaText.text = champion.mana + " / " + champion.maxMana;
    }


}
