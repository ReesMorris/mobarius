using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChat : MonoBehaviour {

    ChatHandler chatHandler;
    PhotonView photonView;
    PlayerChampion playerChampion;

    void Start() {
        photonView = GetComponent<PhotonView>();
        chatHandler = ChatHandler.Instance;
        playerChampion = GetComponent<PlayerChampion>();
    }

    void Update() {
        if (photonView.isMine) {
            chatHandler.CustomUpdate(photonView.viewID);
        }
    }
}
