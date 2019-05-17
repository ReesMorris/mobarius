using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    The script attached to particle effects
*/
/// <summary>
/// The script attached to particle effects.
/// </summary>
public class Effect : MonoBehaviour {

    // Public variables
    public Vector3 initialRot;

    // Private variables
    PhotonView photonView;

    // Assign private variables on game start
    void Start() {
        photonView = GetComponent<PhotonView>();
    }

    /// <summary>
    /// Assigns a particle system to a parent element.
    /// </summary>
    /// <param name="parentId">The PhotonView to attach the particle system to</param>
    public void Init(int parentId) {
        if (photonView == null)
            Start();
        photonView.RPC("InitRPC", PhotonTargets.AllBuffered, parentId);
    }

    /// <summary>
    /// Displays this particle system on the network.
    /// </summary>
    public void Show() {
        photonView.RPC("ShowRPC", PhotonTargets.AllBuffered);
    }

    /// <summary>
    /// Hides this particle system on the network.
    /// </summary>
    public void Hide() {
        photonView.RPC("HideRPC", PhotonTargets.AllBuffered);
    }

    // Initiates the particle system on the network
    [PunRPC]
    void InitRPC(int parentId) {
        GameObject parent = PhotonView.Find(parentId).gameObject;
        transform.eulerAngles = initialRot;
        transform.parent = parent.transform;
        transform.localPosition = Vector3.zero;
        gameObject.SetActive(false);
    }

    // Shows the particle system on the network
    [PunRPC]
    void ShowRPC() {
        gameObject.SetActive(true);
    }

    // Hides the particle system on the network
    [PunRPC]
    void HideRPC() {
        gameObject.SetActive(false);
    }
}
