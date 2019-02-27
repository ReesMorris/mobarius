using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class ChatHandler : MonoBehaviour {

    public static ChatHandler Instance;

    public float autoHideInterval;
    public GameObject chatContainer;
    public GameObject chatPanel;
    public GameObject chatMessagePrefab;
    public ScrollRect scrollRect;
    public TMP_InputField inputField;

    float height;
    RectTransform chatPanelRectTransform;
    float lastMessageReceived;
    PhotonView photonView;

    void Awake() {
        Instance = this;
    }

    void Start() {
        photonView = GetComponent<PhotonView>();
        chatPanelRectTransform = chatPanel.GetComponent<RectTransform>();
    }

    public void SendChatMessage(int playerId, string message) {
        inputField.text = "";
        FocusOnInput();
        photonView.RPC("AddChatMessage", PhotonTargets.All, playerId, message);
    }

    // Called once per frame by PlayerChat.cs
    public void CustomUpdate(int playerId) {
        // Show the textarea when pressing enter (if it's hidden)
        if (!inputField.gameObject.activeSelf) {
            if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)) {
                inputField.gameObject.SetActive(true);
                FocusOnInput();
                chatContainer.SetActive(true);
            }
        }

        // If already showing; check for inputs/closing of the chat
        if (inputField.gameObject.activeSelf) {
            lastMessageReceived = Time.time;
            if (Input.GetKeyDown(KeyCode.Escape)) {
                inputField.gameObject.SetActive(false);
                inputField.text = "";
            }
            if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)) {
                if (inputField.text.Trim().Length > 0) {
                    SendChatMessage(playerId, inputField.text);
                }
            }
        }

        // Auto-hide after time
        if (lastMessageReceived + autoHideInterval < Time.time) {
            chatContainer.SetActive(false);
        }
    }

    void FocusOnInput() {
        EventSystem.current.SetSelectedGameObject(inputField.gameObject, null);
        inputField.OnPointerClick(new PointerEventData(EventSystem.current));
    }

    [PunRPC]
    void AddChatMessage(int playerId, string message) {

        // Get player data
        PhotonView player = PhotonView.Find(playerId);

        // Update some variables
        chatContainer.SetActive(true);
        lastMessageReceived = Time.time;

        // Create the chat object
        GameObject chatMessage = Instantiate(chatMessagePrefab, chatPanel.transform);
        RectTransform rectTransform = chatMessage.GetComponent<RectTransform>();
        TMP_Text text = chatMessage.transform.Find("Message").GetComponent<TMP_Text>();

        // Create the message
        string m = "";
        string colour = ScoreHandler.Instance.friendlyColour;
        if(player.owner.GetTeam() != PhotonNetwork.player.GetTeam())
            colour = ScoreHandler.Instance.enemyColour;
        m += "<color="+colour+">"+player.owner.NickName+" ("+player.name+"): ";
        m += "<color=white>" + message;
        text.text = m;

        // Resize the chat container
        height += text.preferredHeight;
        rectTransform.sizeDelta = new Vector2(chatPanelRectTransform.sizeDelta.x, text.preferredHeight);
        chatPanelRectTransform.sizeDelta = new Vector2(chatPanelRectTransform.sizeDelta.x, height);

        // Scroll to the bottom of the chat container
        scrollRect.normalizedPosition = new Vector2(0, 0);
    }
}
