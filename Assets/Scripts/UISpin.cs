using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISpin : MonoBehaviour {

    public float speed;

    private RectTransform rectTransform;

    void Start() {
        rectTransform = GetComponent<RectTransform>();
    }
	
    void Update() {
        rectTransform.eulerAngles += Vector3.forward * speed;
    }
}
