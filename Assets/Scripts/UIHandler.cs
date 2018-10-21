using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHandler : MonoBehaviour {

    public static UIHandler instance;

    [Header("Infobox")]
    public GameObject infoboxContainer;
    public TMP_Text infoboxText;
    public GameObject infoboxLoading;

    [Header("Error")]
    public GameObject errorContainer;
    public TMP_Text errorText;
    public Button errorButton;
    private string buttonClickUrl;

    // Allows us to create instances
    void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }
    }

    private void Start() {
        errorButton.onClick.AddListener(ErrorButtonClick);
    }


    /*    Error Handling    */

    public void ShowError(string message) {
        buttonClickUrl = "";
        errorText.text = message;
        errorContainer.SetActive(true);
    }
    public void ShowError(string message, string url) {
        ShowError(message);
        buttonClickUrl = url;
    }
    void ErrorButtonClick() {
        if (buttonClickUrl == "") {
            errorContainer.SetActive(false);
        } else {
            Application.OpenURL(buttonClickUrl);
        }
    }
    public void HideError() {
        errorContainer.SetActive(false);
    }

    /* Infobox */

    public void ShowInfobox(string message, bool loading) {
        infoboxText.text = message;
        infoboxContainer.SetActive(true);
        infoboxLoading.SetActive(loading);
    }
    public void HideInfobox() {
        infoboxContainer.SetActive(false);
    }

}
