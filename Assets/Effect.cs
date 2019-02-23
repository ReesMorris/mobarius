using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour {

    public Vector3 initialRot;

    PhotonView photonView;

    void Start() {
        photonView = GetComponent<PhotonView>();
    }

    public void Init(int parentId) {
        if (photonView == null)
            Start();
        photonView.RPC("InitRPC", PhotonTargets.AllBuffered, parentId);
    }

    public void Show() {
        photonView.RPC("ShowRPC", PhotonTargets.AllBuffered);
        print("receiving SHOW request");
    }

    public void Hide() {
        photonView.RPC("HideRPC", PhotonTargets.AllBuffered);
    }

    [PunRPC]
    void InitRPC(int parentId) {
        GameObject parent = PhotonView.Find(parentId).gameObject;
        transform.eulerAngles = initialRot;
        transform.parent = parent.transform;
        transform.localPosition = Vector3.zero;
        gameObject.SetActive(false);
    }

    [PunRPC]
    void ShowRPC() {
        gameObject.SetActive(true);
    }

    [PunRPC]
    void HideRPC() {
        gameObject.SetActive(false);
    }
}
