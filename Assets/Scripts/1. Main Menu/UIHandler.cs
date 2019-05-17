using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
    Handles the main menu and lobby UI elements
*/
/// <summary>
/// Handles the main menu and lobby UI elements.
/// </summary>
public class UIHandler : MonoBehaviour {

    // Public variables
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

    // Returns true if an error modal is currently active
    public bool ErrorShowing {
        get {
            return errorContainer.activeSelf;
        }
    }

    // Create an instance of this script when the game starts
    void Awake() {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(gameObject);
        }
    }

    // Add listeners when the game starts
    void Start() {
        errorButton.onClick.AddListener(ErrorButtonClick);

        closeButton.onClick.AddListener(ShowQuitContainer);
        quitCancel.onClick.AddListener(HideQuitContainer);
        quitConfirm.onClick.AddListener(QuitGame);
    }


    /*    Error Handling    */

    /// <summary>
    /// Displays an error message on a pop-up modal.
    /// </summary>
    /// <param name="message">The message to be displayed</param>
    /// <remarks>
    /// Does not localise the message passed in.
    /// </remarks>
    public void ShowError(string message) {
        buttonClickUrl = "";
        errorText.text = message;
        errorContainer.SetActive(true);
    }

    /// <summary>
    /// Displays an error message on a pop-up modal.
    /// Users will be redirected to the URL when clicking the confirmation button.
    /// </summary>
    /// <param name="message">The message to be displayed</param>
    /// <param name="url">The URL to redirect the user to when closing the modal</param>
    /// <remarks>
    /// Does not localise the message passed in.
    /// </remarks>
    public void ShowError(string message, string url) {
        ShowError(message);
        buttonClickUrl = url;
    }

    /// <summary>
    /// Called when the confirmation button is clicked on the error dialogue.
    /// If a URL was set, users will be redirected to a web page.
    /// </summary>
    public void ErrorButtonClick() {
        if (buttonClickUrl == "") {
            errorContainer.SetActive(false);
        } else {
            Application.OpenURL(buttonClickUrl);
        }
    }

    /// <summary>
    /// Closes the error modal.
    /// </summary>
    public void HideError() {
        errorContainer.SetActive(false);
    }

    /* Infobox */

    /// <summary>
    /// Displays the infobox in the upper-right corner of the screen.
    /// </summary>
    /// <param name="message">The message to be displayed</param>
    /// <param name="loading">Whether to display a loading spinner next to the message</param>
    /// <remarks>
    /// Does not localise the message passed in.
    /// </remarks>
    public void ShowInfobox(string message, bool loading) {
        infoboxText.text = message;
        infoboxContainer.SetActive(true);
        infoboxLoading.SetActive(loading);
    }

    /// <summary>
    /// Closes the infobox modal.
    /// </summary>
    public void HideInfobox() {
        infoboxContainer.SetActive(false);
    }

    /* Quit */

    /// <summary>
    /// Displays the "are you sure you would like to quit?" container.
    /// </summary>
    public void ShowQuitContainer() {
        quitContainer.SetActive(true);
    }

    /// <summary>
    /// Closes the "are you sure you would like to quit?" container.
    /// </summary>
    public void HideQuitContainer() {
        quitContainer.SetActive(false);
    }

    // Called when the confirmation button is pressed from the quit container.
    void QuitGame() {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }

    /* Lobby UI */

    /// <summary>
    /// Displays the UI for the lobby.
    /// </summary>
    public void ShowLobbyUI() {
        lobbyUI.SetActive(true);
    }

    /// <summary>
    /// Closes the UI for the lobby.
    /// </summary>
    public void HideLobbyUI() {
        lobbyUI.SetActive(false);
    }

}
