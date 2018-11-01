using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {

    public Transform target;
    public Vector3 distance;
    public Vector3 rotation;

    PhotonView photonView;

    void Start() {
        photonView = target.GetComponent<PhotonView>();
    }

	void Update () {
        if (photonView.isMine) {
            transform.localPosition = target.position + distance;
            transform.localEulerAngles = rotation;
        }
	}
}
