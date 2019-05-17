using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This script will hide a GameObject when the game begins
*/
/// <summary>
/// This script will hide a GameObject when the game begins.
/// </summary>
public class HideOnStart : MonoBehaviour {

    // Hides the attached GameObject when the game begins
    void Start() {
        gameObject.SetActive(false);
    }
}
