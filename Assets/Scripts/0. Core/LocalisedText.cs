using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
    Overrides the text of a TMP Text element to its localised equivalent
*/
/// <summary>
/// Overrides the text of a TMP Text element to its localised equivalent.
/// </summary>
public class LocalisedText : MonoBehaviour {

    // Public variables
    public string key;
    public string[] parameters;

    // Updates the text of the TMP Text element attached to this script when the game begins.
	void Start () {
        GetComponent<TMP_Text>().text = LocalisationManager.instance.GetValue(key, parameters);
	}
}
