using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using TMPro;

public class AuthenticationManager : MonoBehaviour
{

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
        errorContainer.SetActive(true);
        errorText.text = message;
    }

    // Called when the user presses the log in button
    void OnLoginButtonClick() {
        // Set the user to authenticating, and prevent further clicks from being made
        authenticating = true;

        // This is where we will authenticate a user login
        JSONNode user = ValidateLogin(loginUsername.text, loginPassword.text);
        if (user["error"] == null) {
            // We have validation!
        }
        else {
            // Something went wrong
            authenticating = false;
            ShowError(user["error"]);
        }
    }

    // Will return a JSON array of either a failure code, or a user which can be converted to an Account class
    JSONNode ValidateLogin(string username, string password) {
        print("Validating login for " + username + " with password" + password);

        // WWW request to web server to check to see if user exists

        JSONNode parsed = JSON.Parse("{\"error\":\"user_not_found\"}");
        return parsed;
    }
}
