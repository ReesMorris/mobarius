using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AutoWidthText : MonoBehaviour {

    public float padding;

    TMP_Text text;
    RectTransform rectTransform;

	void Start () {
        rectTransform = GetComponent<RectTransform>();
        text = GetComponent<TMP_Text>();
        UpdateWidth();
	}
	
    public void UpdateWidth() {
        rectTransform.sizeDelta = new Vector2(text.preferredWidth + padding, rectTransform.sizeDelta.y);
    }
}
