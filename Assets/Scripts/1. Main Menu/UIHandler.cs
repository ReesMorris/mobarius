using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHandler : MonoBehaviour {

    public static UIHandler Instance;

    public GameObject lobbyUI;
    public GameObject gameUI;

    [Header("Menu Options")]
    public Button closeButton;
    public Button settingsButton;
    public Button minimizeButton;
    public Button maximizeButton;

    [Header("Infobox")]
    public GameObject infoboxContainer;
    public TMP_Text infoboxText;
    public GameObject infoboxLoading;

    [Header("Error")]
    public GameObject errorContainer;
    public TMP_Text errorText;
    public Button errorButton;
    private string buttonClickUrl;

    [Header("Quit")]
    public GameObject quitContainer;
    public Button quitConfirm;
    public Button quitCancel;

    public bool ErrorShowing {
        get {
            return errorContainer.activeSelf;
        }
    }

    // Allows us to create instances
    void Awake() {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(gameObject);
        }
    }

    private void Start() {
        errorButton.onClick.AddListener(ErrorButtonClick);

        closeButton.onClick.AddListener(ShowQuitContainer);
        quitCancel.onClick.AddListener(HideQuitContainer);
        quitConfirm.onClick.AddListener(QuitGame);
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
    public void ErrorButtonClick() {
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

    /* Quit */

    public void ShowQuitContainer() {
        quitContainer.SetActive(true);
    }
    public void HideQuitContainer() {
        quitContainer.SetActive(false);
    }
    void QuitGame() {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }

    /* Lobby UI */

    public void HideLobbyUI() {
        lobbyUI.SetActive(false);
    }
    public void ShowLobbyUI() {
        lobbyUI.SetActive(true);
    }

}
