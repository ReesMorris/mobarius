using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*
    Updates the size of a RectTransform UI element based on its text content
*/
/// <summary>
/// Updates the size of a RectTransform UI element based on its text content.
/// </summary>
public class AutoWidthText : MonoBehaviour {

    // Public variables
    public float padding;

    // Private variables
    TMP_Text text;
    RectTransform rectTransform;

    /// <summary>
    /// Assigns private variables and automatically resizes the attached RectTransform.
    /// </summary>
	void Start () {
        rectTransform = GetComponent<RectTransform>();
        text = GetComponent<TMP_Text>();
        UpdateWidth();
	}
	
    /// <summary>
    /// Updates the size of a RectTransform UI element based on its text content, taking padding into consideration.
    /// </summary>
    public void UpdateWidth() {
        rectTransform.sizeDelta = new Vector2(text.preferredWidth + padding, rectTransform.sizeDelta.y);
    }
}
