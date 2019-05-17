﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This script handles the particle systems generated by Alucard's E effect
*/
/// <summary>
/// This script handles the particle systems generated by Alucard's E effect.
/// </summary>
public class AlucardE_Effect : MonoBehaviour {

    // Private variables
    PhotonView photonView;

    /// <summary>
    /// Initialises the particle system on the network
    /// </summary>
    /// <param name="senderId">The ID of the player performing the ability</param>
    public void Init(int senderId) {
        photonView = GetComponent<PhotonView>();
        photonView.RPC("SetPosition", PhotonTargets.AllBuffered, senderId);
    }

    // Sets the position of the particle system in the world, across the network
    [PunRPC]
    void SetPosition(int senderId) {
        transform.SetParent(PhotonView.Find(senderId).transform);
        transform.localPosition = Vector3.zero;
    }
}
