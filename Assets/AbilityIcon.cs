using System.Collections;
using System.Collections.Generic;
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

    Ability ability;

    public void SetupIcon(Ability _ability, string keyCode) {
        ability = _ability;
        icon.sprite = ability.icon;
        icon2.sprite = ability.icon;
        tooltip.SetActive(false);
        string passive = "";
        if (keyCode != "") {
            iconBg.SetActive(false);
            cooldown.text = "";
            hotkey.text = keyCode;
        } else {
            passive = "(Passive)";
        }

        tooltipText.text = "<b>" + ability.name + "</b> " + passive + "\n\n" + ability.desc;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if(tooltip != null) {
            tooltip.SetActive(true);
            tooltip.GetComponent<RectTransform>().sizeDelta = new Vector2(900, tooltipText.preferredHeight + 50f); // [src: https://forum.unity.com/threads/modify-the-width-and-height-of-recttransform.270993/]
            tooltip.SetActive(false);
            tooltip.GetComponent<RectTransform>().sizeDelta = new Vector2(900, tooltipText.preferredHeight + 50f); // [src: https://forum.unity.com/threads/modify-the-width-and-height-of-recttransform.270993/]
            tooltip.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if(tooltip != null) {
            tooltip.SetActive(false);
        }
    }


}
