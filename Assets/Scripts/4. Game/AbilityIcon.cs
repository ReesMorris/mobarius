using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class AbilityIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public Image icon;
    public Image icon2;
    public GameObject iconBg;
    public TMP_Text hotkey;
    public TMP_Text cooldown;
    public GameObject tooltip;
    public TMP_Text tooltipText;
    public TMP_Text tooltipInfo;

    Ability ability;

    public void SetupIcon(Ability _ability, string keyCode, Champion champion) {
        ability = _ability;
        icon.sprite = ability.icon;
        icon2.sprite = ability.icon;
        tooltip.SetActive(false);
        if (keyCode != "") {
            iconBg.SetActive(false);
            cooldown.text = "";
            hotkey.text = keyCode;
        }

        string cost = ability.cost + " Mana";
        if (ability.cost == 0)
            cost = "No Cost";

        // Replace string texts
        string desc = ability.desc;
        foreach(AbilityDamage ad in ability.damage) {
            string message = "";
            if (ad.damageType == AbilityHandler.DamageTypes.PhysicalDamage)
                message = ad.damage + " <color=#fa8a01>(+" + champion.physicalDamage + ")</color> physical damage";
            else if (ad.damageType == AbilityHandler.DamageTypes.MagicDamage)
                message = ad.damage + " <color=#bd77ff>(+" + champion.magicDamage + ")</color> magic damage";
            else
                message = "undefined";
            desc = ReplaceFirst(desc, "{x}", message);
        }

        tooltipText.text = 
            "<color=#dfcf8f><size=18>" + ability.name + "</size></color>\n" + 
            "<color=#b2b2b3><size=15>" + cost + "</size></color>\n\n" + 
            "<color=#b2b2b3><size=15>" + desc + "</size></color>";
        tooltipInfo.text =
            "<color=#dfcf8f><size=18>[" + ability.abilityKey + "]</size></color>\n" +
            "<color=#b2b2b3><size=15>" + ability.cooldown + "s Cooldown</size></color>";
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if(tooltip != null) {
            tooltip.SetActive(true);
            tooltip.GetComponent<RectTransform>().sizeDelta = new Vector2(570, tooltipText.preferredHeight + 20f); // [src: https://forum.unity.com/threads/modify-the-width-and-height-of-recttransform.270993/]
            tooltip.SetActive(false);
            tooltip.GetComponent<RectTransform>().sizeDelta = new Vector2(570, tooltipText.preferredHeight + 20f); // [src: https://forum.unity.com/threads/modify-the-width-and-height-of-recttransform.270993/]
            tooltip.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if(tooltip != null) {
            tooltip.SetActive(false);
        }
    }

    public void SetCooldown(float cooldownRemaining, float cooldownDuration) {
        if (cooldownRemaining > 0f) {
            iconBg.SetActive(true);
            icon2.fillAmount = 1 - (cooldownRemaining / cooldownDuration);
            if(cooldownRemaining > 1f)
                cooldown.text = Mathf.Ceil(cooldownRemaining).ToString();
            else
                cooldown.text =cooldownRemaining.ToString("F1");

        } else {
            iconBg.SetActive(false);
            cooldown.text = "";
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
