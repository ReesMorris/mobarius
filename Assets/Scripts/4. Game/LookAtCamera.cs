using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This script will constantly the attached GameObject to the camera
*/
/// <summary>
/// This script will constantly the attached GameObject to the camera.
/// </summary>
public class LookAtCamera : MonoBehaviour {

    // Called when the game first starts
    void Start() {
        this.enabled &= PhotonNetwork.player.IsLocal;
    }
	
    // Called every frame
    void Update() {
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }
}
