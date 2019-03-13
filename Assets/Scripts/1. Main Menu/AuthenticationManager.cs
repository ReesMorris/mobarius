using System.Collections;
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

    private bool authenticating;
    private UIHandler UIHandler;
    private VersionManager VersionManager;
    private MainMenuManager mainMenuManager;
    public bool LoggedIn { get; private set; }

    private void Start() {
        // References
        UIHandler = GetComponent<UIHandler>();
        VersionManager = GetComponent<VersionManager>();
        mainMenuManager = GetComponent<MainMenuManager>();

        // Labels
        registerLabel.onClick.AddListener(OnRegisterLabelClick);
        loginLabel.onClick.AddListener(OnLoginLabelClick);

        // Buttons
        registerButton.onClick.AddListener(OnRegisterButtonClick);
        loginButton.onClick.AddListener(OnLoginButtonClick);
    }

    void Update() {
        // Login mechanics | will enable/disable inputs depending on what the user is doing
        loginButton.interactable = (!authenticating && loginUsername.text != "" && loginPassword.text != "" && !VersionManager.outdated);
        loginUsername.interactable = loginPassword.interactable = (!authenticating);
        loginButtonText.SetActive(!authenticating);
        loginButtonLoading.SetActive(authenticating);

        // Register mechanics | will enable/disable inputs depending on what the user is doing
        registerLabel.interactable = (!VersionManager.outdated);
        registerButton.interactable = (!authenticating && registerUsername.text != "" && registerEmail.text != "" && registerPassword.text != "" && registerPassword2.text != "" && !VersionManager.outdated);
        registerUsername.interactable = registerEmail.interactable = registerPassword.interactable = registerPassword2.interactable = !authenticating;
        registerButtonText.SetActive(!authenticating);
        registerButtonLoading.SetActive(authenticating);
    }

    void OnRegisterLabelClick() {
        loginModal.SetActive(false);
        registerModal.SetActive(true);
        registerUsername.Select();
    }
    void OnLoginLabelClick() {
        loginModal.SetActive(true);
        registerModal.SetActive(false);
        loginUsername.Select();
    }

    public void ShowError(string message) {
        authenticating = false;
        UIHandler.ShowError(message);
    }

    // Called when the user presses the log in button
    public void OnLoginButtonClick() {
        if(loginButton.interactable) {
            // Set the user to authenticating, and prevent further clicks from being made
            authenticating = true;

            // This is where we will authenticate a user login
            UIHandler.HideError();
            StartCoroutine(LoginRequest(loginUsername.text, loginPassword.text));
            StartCoroutine("LoggingInTimer");
        }
    }

    public void OnRegisterButtonClick() {
        if(registerButton.interactable) {
            // Set the user to authenticating, and prevent further clicks from being made
            authenticating = true;

            // This is where we will authenticate a user login
            UIHandler.HideError();
            StartCoroutine(RegisterRequest(registerUsername.text, registerEmail.text, registerPassword.text, registerPassword2.text));
        }
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
            StopCoroutine("LoggingInTimer");
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
            StopCoroutine("LoggingInTimer");

        } else {
            LoginUser(data["user"]);
        }
    }

    void LoginUser(JSONNode user) {
        StopCoroutine("LoggingInTimer");
        UserManager.Instance.account = new Account(user["_id"], user["username"], user["email"], user["created_at"], int.Parse(user["xp"]), user["lastGameID"], user["sessionToken"]);
        LoggedIn = true;
        mainMenuManager.Prepare();
    }

    IEnumerator LoggingInTimer() {
        yield return new WaitForSeconds(7f);
        ShowError(LocalisationManager.instance.GetValue("login_error_timeout"));
    }
}
