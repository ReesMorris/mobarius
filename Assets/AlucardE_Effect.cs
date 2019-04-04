using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlucardE_Effect : MonoBehaviour {

    PhotonView photonView;

    public void Init(int senderId) {
        photonView = GetComponent<PhotonView>();
        photonView.RPC("SetPosition", PhotonTargets.AllBuffered, senderId);
    }

    [PunRPC]
    void SetPosition(int senderId) {
        transform.SetParent(PhotonView.Find(senderId).transform);
        transform.localPosition = Vector3.zero;
    }
}
