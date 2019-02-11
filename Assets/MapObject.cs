using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideOnStart : MonoBehaviour {

    void Start() {
        gameObject.SetActive(false);
    }
}
