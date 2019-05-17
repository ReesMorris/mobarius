using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*
    Updates a TMP Text object across the Photon Network to contain a new text value
*/
/// <summary>
/// Updates a TMP Text object across the Photon Network to contain a new text value.
/// </summary>
public class SetText : MonoBehaviour {


    // Private variables
    private TMP_Text textObj;

    /// <summary>
    /// Assigns private variables once the game begins.
    /// </summary>
	void Start () {
        textObj = GetComponent<TMP_Text>();
    }

    /// <summary>
    /// Sets the text of the TMP Text object attached to this script and synchronises it across the network.
    /// </summary>
    /// <param name="text">The message to be displayed</param>
    /// <remarks>
    /// Does not localise the message passed in.
    /// </remarks>
    public void Set(string text) {
        GetComponent<PhotonView>().RPC("ChangeText", PhotonTargets.All, new object[] { text });
    }

    // The RPC to synchronise the text change across the network 
    [PunRPC]
    void ChangeText(string text) {
        textObj.text = text;
    }
}
