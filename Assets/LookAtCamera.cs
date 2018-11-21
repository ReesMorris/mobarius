using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour {

	void Start () {
        this.enabled &= PhotonNetwork.player.IsLocal;
	}
	
	void Update () {
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }
}
