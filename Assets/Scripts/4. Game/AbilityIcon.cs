﻿using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

/*
    The script responsible for handling the ability icon UI interface
*/
/// <summary>
/// The script responsible for handling the ability icon UI interface.
/// </summary>
public class AbilityIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    // Public variables
    public Image icon;
    public Image icon2;
    public GameObject iconBg;
    public TMP_Text hotkey;
    public TMP_Text cooldown;
    public Button upgradeButton;

    // Private variables
    Champion champion;
    Ability ability;
    RectTransform rectTransform;
    AbilityHandler abilityHandler;

    string cost;
    string desc;
    int level;

    // Fetch instances on game start.
    void Start() {
        abilityHandler = AbilityHandler.Instance;
    }

    /// <summary>
    /// Initialises the class.
    /// </summary>
    /// <param name="_ability">The ability this UI represents</param>
    /// <param name="keyCode">The key code this ability is activated with</param>
    /// <param name="_champion">The champion who uses this ability</param>
    /// <remarks>
    /// Abilities that can not be upgraded should be pass in 'null' for the first parameter.
    /// The passive ability should pass in '' for the keyCode parameter.
    /// </remarks>
    public void SetupIcon(Ability _ability, string keyCode, Champion _champion) {
        level = AbilityHandler.Instance.GetAbilityLevel(_champion, _ability);
        if (ability == null)
            SetCooldown(0, 0);
        if (level == 1 && iconBg != null)
            iconBg.SetActive(false);
        ability = _ability;
        champion = _champion;
        icon.sprite = ability.icon;
        icon2.sprite = ability.icon;
        abilityHandler.tooltip.SetActive(false);
        if (keyCode != "") {
            cooldown.text = "";
            hotkey.text = keyCode;
        }

        // Enable the upgrade button if not maxed out, and if is not special case (R ability)
        if (upgradeButton != null) {
            upgradeButton.interactable = true;
            if (level == ability.maxLevel)
                upgradeButton.interactable = false;
            if (keyCode == "R")
                if (champion.currentLevel < 6 || (champion.currentLevel < 11 && level == 1) || (champion.currentLevel < 16 && level == 2))
                    upgradeButton.interactable = false;
        }

        UpdateTooltipText();
    }

    // Called when a champion levels up; so that it doesn't need to be done every single time
    void UpdateTooltipText() {
        if (champion != null) {
            cost = ability.cost + " Mana";
            if (ability.cost == 0)
                cost = "No Cost";

            // Replace string texts
            desc = ability.desc;
            desc = desc.Replace("{duration}", ability.duration.ToString());
            foreach (AbilityDamage ad in ability.damage) {
                string message = "";
                string type = "";
                string colour = "#eee";
                float extraDamage = 0;
                for (int i = 1; i <= ability.maxLevel; i++) {
                    float damage = ability.GetDamage(ad.key, i);

                    // Set the colour for the ability
                    if (ad.damageType == AbilityHandler.DamageTypes.PhysicalDamage) {
                        colour = "#fa8a01";
                        type = "physical damage";
                        extraDamage = champion.physicalDamage;
                    }
                    if (ad.damageType == AbilityHandler.DamageTypes.MagicDamage) {
                        colour = "#bd77ff";
                        type = "magic damage";
                        extraDamage = champion.magicDamage;
                    }
                    if (ad.damageType == AbilityHandler.DamageTypes.Health) {
                        colour = "#90ee90";
                        type = "health";
                    }

                    // Show the text (1/2/3/4/5)
                    if (i == level)
                        message += "<color=" + colour + ">" + damage + "</color>";
                    else
                        message += "<color=#727272>" + damage + "</color>";
                    if (i != ability.maxLevel)
                        message += "/";
                }

                // Finish the message
                message += "<color=" + colour + "> (+" + extraDamage + ")</color> " + type;
                desc = ReplaceFirst(desc, "{x}", message);
            }
        }
    }

    // Called when the tooltip is set to show for this ability
    void SetTooltipText() {
        string key = ability.abilityKey.ToString();
        if (ability.abilityKey != AbilityHandler.Abilities.Passive)
            key = "[" + ability.abilityKey + "]";
        string cooldown = ability.cooldown + "s Cooldown";
        if (ability.cooldown == 0)
            cooldown = "No Cooldown";

        abilityHandler.tooltipText.text =
            "<color=#dfcf8f><size=25>" + ability.name + "</size></color>\n" +
            "<size=5> </size>\n" +
            "<color=#b2b2b3><size=23>" + cost + "</size></color>\n\n" +
            "<size=20> </size>\n" +
            "<color=#b2b2b3><size=23>" + desc + "</size></color>";
        abilityHandler.tooltipInfo.text =
            "<color=#dfcf8f><size=25>" + key + "</size></color>\n" +
            "<size=5> </size>\n" +
            "<color=#b2b2b3><size=23>" + cooldown + "</size></color>";
    }

    /// <summary>
    /// Called when the mouse enters the UI element this script is attached to. Displays the tooltip relating to the ability this icon represents.
    /// </summary>
    /// <remarks>
    /// The tooltip is automatically sized based on its content.
    /// </remarks>
    public void OnPointerEnter(PointerEventData eventData) {
        if(rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        if (abilityHandler.tooltip != null) {
            SetTooltipText();
            RectTransform rt = abilityHandler.tooltip.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, abilityHandler.tooltipText.preferredHeight + 35f); // [src: https://forum.unity.com/threads/modify-the-width-and-height-of-recttransform.270993/]
            abilityHandler.tooltip.SetActive(false);
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, abilityHandler.tooltipText.preferredHeight + 35f); // [src: https://forum.unity.com/threads/modify-the-width-and-height-of-recttransform.270993/]
            abilityHandler.tooltip.SetActive(true);
        }
    }

    /// <summary>
    /// Called when the mouse leaves the UI element this script is attached to. Hides any tooltips.
    /// </summary>
    public void OnPointerExit(PointerEventData eventData) {
        if(abilityHandler.tooltip != null) {
            abilityHandler.tooltip.SetActive(false);
        }
    }

    /// <summary>
    /// Sets the cooldown text to display for this ability.
    /// Usually called every 0.1 seconds from another function after an ability has been casted.
    /// </summary>
    /// <param name="cooldownRemaining">The number of seconds remaining on the cooldown</param>
    /// <param name="cooldownDuration">The total number of seconds the cooldown will last</param>
    /// <remarks>
    /// Does not control whether the user can activate the ability or not; this is a purely cosmetic function.
    /// </remarks>
    public void SetCooldown(float cooldownRemaining, float cooldownDuration) {

        // If the cooldown is ongoing
        if (cooldownRemaining > 0f) {

            // Set the overlay image on the cooldown icon to slowly fill over time
            iconBg.SetActive(true);
            icon2.fillAmount = 1 - (cooldownRemaining / cooldownDuration);
            if(cooldownRemaining > 1f)
                cooldown.text = Mathf.Ceil(cooldownRemaining).ToString();
            else
                cooldown.text = cooldownRemaining.ToString("F1");
        }
        
        // If the cooldown has ended, make the icon as clear as possible
        else {
            if (iconBg != null) {
                if (level == 0)
                    icon2.fillAmount = 0;
                iconBg.SetActive(level == 0);
                cooldown.text = "";
            }
        }
    }

    // src https://stackoverflow.com/questions/141045/how-do-i-replace-the-first-instance-of-a-string-in-net
    string ReplaceFirst(string text, string search, string replace) {
        int pos = text.IndexOf(search);
        if (pos < 0) {
            return text;
        }
        return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
    }


}
