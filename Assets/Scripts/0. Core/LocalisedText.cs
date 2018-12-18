using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LocalisedText : MonoBehaviour {

    public string key;
    public string[] parameters;

	void Start () {
        GetComponent<TMP_Text>().text = LocalisationManager.instance.GetValue(key, parameters);
	}
}
