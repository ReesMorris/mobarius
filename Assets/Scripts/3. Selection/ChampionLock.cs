using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
    The script responsible for networking the 'lock in' button across the network
*/
/// <summary>
/// The script responsible for networking the 'lock in' button across the network.
/// </summary>
public class ChampionLock : MonoBehaviour {

    // Private variables
    private TMP_Text title;
    private Image image;
    private ChampionSelect championSelect;

    // Set references to private variables when the game starts
    void Start() {
        title = transform.Find("Title").GetComponent<TMP_Text>();
        image = transform.Find("Image").GetComponent<Image>();
        championSelect = GameObject.Find("GameManager").GetComponent<ChampionSelect>();
    }

    /// <summary>
    /// Sends an RPC call to all networked users, updating the UI and text to the player's choice.
    /// </summary>
    /// <param name="champion">The Champion being locked in by the local player</param>
    public void LockIn(Champion champion) {
        GetComponent<PhotonView>().RPC("Lock", PhotonTargets.All, new object[] { champion.championName });
    }

    /// <summary>
    /// Sends an RPC call to add the local player's UI onto the left/right sidebar across the network.
    /// </summary>
    /// <param name="isLeft">True if the player should be displayed on the left side of the UI</param>
    /// <param name="playerName">The username of the player being displayed</param>
    public void SetPosition(bool isLeft, string playerName) {
        GetComponent<PhotonView>().RPC("Position", PhotonTargets.All, new object[] { isLeft, playerName });
    }

    // The RPC call to instantiate the local player's UI onto the left/right sidebar
    [PunRPC]
    void Position(bool isLeft, string playerName) {
        if (championSelect == null)
            Start();

        // Choose which side to display
        if (isLeft) {
            gameObject.transform.SetParent(championSelect.leftColumn);
        } else {
            gameObject.transform.SetParent(championSelect.rightColumn);
        }

        // Update the variables across the network
        gameObject.name = playerName;
        gameObject.GetComponent<RectTransform>().localScale = Vector3.one;
        gameObject.transform.Find("Username").GetComponent<TMP_Text>().text = playerName;
    }

    // The RPC call to lock the player in, updating their chosen character UI across the network
    [PunRPC]
    void Lock(string championName) {
        image.sprite = Resources.Load<Sprite>("Champions/Icons/" + championName);
        title.text = championName;
        championSelect.OnPlayerLock();
    }
}
