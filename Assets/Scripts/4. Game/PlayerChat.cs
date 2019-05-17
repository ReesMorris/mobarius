using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This script focuses on handling the player chat interface
*/
/// <summary>
/// This script focuses on handling the player chat interface.
/// </summary>
public class PlayerChat : MonoBehaviour {

    // Private variables
    ChatHandler chatHandler;
    PhotonView photonView;
    PlayerChampion playerChampion;

    // Assign variables when the game starts.
    void Start() {
        photonView = GetComponent<PhotonView>();
        chatHandler = ChatHandler.Instance;
        playerChampion = GetComponent<PlayerChampion>();
    }

    // Every frame, update the chat interface for the local player
    void Update() {
        if (photonView.isMine) {
            chatHandler.CustomUpdate(photonView.viewID);
        }
    }
}
