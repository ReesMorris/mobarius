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
    public string registerUrl;

    [Header("Login Modal")]
    public GameObject loginModal;
    public TMP_InputField loginUsername;
    public TMP_InputField loginPassword;
    public Button loginButton;
    public GameObject loginButtonText;
    public GameObject loginButtonLoading;
    public Button registerLabel;

    [Header("Register Modal")]
    public GameObject registerModal;
    public TMP_InputField registerUsername;
    public TMP_InputField registerEmail;
    public TMP_InputField registerPassword;
    public TMP_InputField registerPassword2;
    public Button registerButton;
    public GameObject registerButtonText;
    public GameObject registerButtonLoading;
    public Button loginLabel;

    [Header("Error")]
    public GameObject errorContainer;
    public TMP_Text errorText;
    public Button errorButton;

    private bool authenticating;

    private void Start() {
        // Labels
        registerLabel.onClick.AddListener(OnRegisterLabelClick);
        loginLabel.onClick.AddListener(OnLoginLabelClick);

        // Buttons
        registerButton.onClick.AddListener(OnRegisterButtonClick);
        loginButton.onClick.AddListener(OnLoginButtonClick);
        errorButton.onClick.AddListener(HideErrorContainer);
    }

    void Update() {
        // Login mechanics | will enable/disable inputs depending on what the user is doing
        loginButton.interactable = (!authenticating && loginUsername.text != "" && loginPassword.text != "");
        loginUsername.interactable = loginPassword.interactable = !authenticating;
        loginButtonText.SetActive(!authenticating);
        loginButtonLoading.SetActive(authenticating);

        // Register mechanics | will enable/disable inputs depending on what the user is doing
        registerButton.interactable = (!authenticating && registerUsername.text != "" && registerEmail.text != "" && registerPassword.text != "" && registerPassword2.text != "");
        registerUsername.interactable = registerEmail.interactable = registerPassword.interactable = registerPassword2.interactable = !authenticating;
        registerButtonText.SetActive(!authenticating);
        registerButtonLoading.SetActive(authenticating);
    }

    void OnRegisterLabelClick() {
        loginModal.SetActive(false);
        registerModal.SetActive(true);
    }
    void OnLoginLabelClick() {
        loginModal.SetActive(true);
        registerModal.SetActive(false);
    }
    void HideErrorContainer() {
        errorContainer.SetActive(false);
    }

    void ShowError(string message) {
        authenticating = false;
        errorContainer.SetActive(true);
        errorText.text = message;
    }

    // Called when the user presses the log in button
    void OnLoginButtonClick() {
        // Set the user to authenticating, and prevent further clicks from being made
        authenticating = true;

        // This is where we will authenticate a user login
        HideErrorContainer();
        StartCoroutine(LoginRequest(loginUsername.text, loginPassword.text));
    }

    void OnRegisterButtonClick() {
        // Set the user to authenticating, and prevent further clicks from being made
        authenticating = true;

        // This is where we will authenticate a user login
        HideErrorContainer();
        StartCoroutine(RegisterRequest(registerUsername.text, registerEmail.text, registerPassword.text, registerPassword2.text));
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
            ValidateAuthentication(www.downloadHandler.text);
        }
    }

    // Will send a register request to the server string specified
    IEnumerator RegisterRequest(string username, string email, string password, string password2) {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("email", email);
        form.AddField("password", password);
        form.AddField("password2", password2);

        UnityWebRequest www = UnityWebRequest.Post(registerUrl, form);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            ShowError(www.error);
        }
        else {
            ValidateAuthentication(www.downloadHandler.text);
        }
    }

    // Validates whether a user is logged in or not
    void ValidateAuthentication(string response) {
        JSONNode data = JSON.Parse(response);
        if (!bool.Parse(data["success"])) {

            // Some errors are returned with additional information to be parsed:  error_code|3,4,5
            string tmp = data["error"].ToString().Remove(0, 1).Remove(data["error"].ToString().Length - 2, 1);
            string[] error = tmp.Split('|');
            string[] parameters = {};
            string message = error[0];
            if (error.Length > 1) {
                parameters = error[1].Split(',');
            }

            ShowError(LocalisationManager.instance.GetValue(message, parameters));

        } else {
            LoginUser(data["user"]);
        }
    }

    void LoginUser(JSONNode user) {
        Account account = new Account(user["username"], user["email"], user["created_at"]);
        ShowError("(debug) Successfully logged in as, " + account.username);

        // To be sent to & used in a network manager script ...
    }
}
