using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
    The class containing information about an individual ability UI element
*/
/// <summary>
/// The class containing information about an individual ability UI element.
/// </summary>
[System.Serializable]
public class AbilityUI {

    // Public variables
    public TMP_Text keybind;
    public Image icon;
    public Image cooldownIcon;
    public GameObject cooldownBg;
    public TMP_Text cooldownText;
}
