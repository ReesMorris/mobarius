using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using TMPro;
using UnityEngine.Networking;

public class AuthenticationManager : MonoBehaviour {

    [Header("Server")]
    public string loginUrl;

    [Header("Login")]
    public TMP_InputField loginUsername;
    public TMP_InputField loginPassword;
    public Button loginButton;
    public GameObject loginButtonText;
    public GameObject loginButtonLoading;

    [Header("Error")]
    public GameObject errorContainer;
    public TMP_Text errorText;
    public Button errorButton;

    private bool authenticating;

    private void Start() {
        loginButton.onClick.AddListener(OnLoginButtonClick);
        errorButton.onClick.AddListener(OnErrorButtonClick);
    }

    void Update() {
        // Login mechanics | will enable/disable inputs depending on what the user is doing
        loginButton.interactable = (!authenticating && loginUsername.text != "" && loginPassword.text != "");
        loginUsername.interactable = !authenticating;
        loginPassword.interactable = !authenticating;
        loginButtonText.SetActive(!authenticating);
        loginButtonLoading.SetActive(authenticating);
    }

    void OnErrorButtonClick() {
        errorContainer.SetActive(false);
    }

    void ShowError(string message) {
        authenticating = false;
        errorContainer.SetActive(true);

        // Error codes (will be moved in future)
        message.Replace("login_error_unknown", "An unknown error occured. Please try again.");
        message.Replace("login_error_user_not_found", "A user could not be found with that username. If you have not yet created an account, please register first.");
        message.Replace("login_error_invalid_password", "The password entered is incorrect. Please try again.");
        message.Replace("invalid_params", "Please complete both fields before continuing.");

        errorText.text = message;
    }

    // Called when the user presses the log in button
    void OnLoginButtonClick() {
        // Set the user to authenticating, and prevent further clicks from being made
        authenticating = true;

        // This is where we will authenticate a user login
        StartCoroutine(LoginRequest(loginUsername.text, loginPassword.text));
    }

    // Will send a login request to the server string specified
    IEnumerator LoginRequest(string username, string password) {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        UnityWebRequest www = UnityWebRequest.Post(loginUrl, form);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            ShowError(www.error);
        }
        else {
            ValidateLogin(www.downloadHandler.text);
        }
    }

    // Validates whether a user is logged in or not
    void ValidateLogin(string response) {
        JSONNode data = JSON.Parse(response);
        if (!bool.Parse(data["success"])) {
            ShowError(data["error"]);
        } else {
            // We need to construct an Account class from this data
            print(data["user"]);
        }
    }
}
