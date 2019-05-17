using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Spins the RectTransform element attached to this script
*/
/// <summary>
/// Spins the RectTransform element attached to this script.
/// </summary>
public class UISpin : MonoBehaviour {

    // Public variables
    public float speed;

    // Private variables
    private RectTransform rectTransform;

    // Assigns private variables on game start.
    void Start() {
        rectTransform = GetComponent<RectTransform>();
    }
	
    // Rotate the RectTransform element by (speed) every frame
    void Update() {
        rectTransform.eulerAngles += Vector3.forward * speed;
    }
}
