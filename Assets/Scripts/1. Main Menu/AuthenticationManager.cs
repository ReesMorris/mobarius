using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using TMPro;
using UnityEngine.Networking;

/*
    Handles user login and registration
*/
/// <summary>
/// Handles user login and registration.
/// </summary>
public class AuthenticationManager : MonoBehaviour {

    // Public variables
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

    // Private variables
    private bool authenticating;
    private UIHandler UIHandler;
    private VersionManager VersionManager;
    private MainMenuManager mainMenuManager;
    public bool LoggedIn { get; private set; }

    // Called when the game starts; add listeners and get references to other scripts
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

    // Called every frame; toggles the auth button on/off depending on the contents of the input fields
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

    // Called when the user clicks on the "Don't have an account?" button
    void OnRegisterLabelClick() {
        loginModal.SetActive(false);
        registerModal.SetActive(true);
        registerUsername.Select();
    }

    // Called when the user clicks on the "Already have an account?" button
    void OnLoginLabelClick() {
        loginModal.SetActive(true);
        registerModal.SetActive(false);
        loginUsername.Select();
    }

    /// <summary>
    /// Displays an error modal in the center of the screen.
    /// </summary>
    /// <param name="message">The error message to display on the modal</param>
    /// <remarks>
    /// Does not localise the message passed in.
    /// </remarks>
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

    // Called when the user presses on the register button
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

        // Send the web request
        UnityWebRequest www = UnityWebRequest.Post(loginUrl, form);
        yield return www.SendWebRequest();

        // Display the error if the request could not complete successfully
        if (www.isNetworkError || www.isHttpError) {
            ShowError(www.error);
            StopCoroutine("LoggingInTimer");
        }

        // Pass validation on to verify the request
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

        // Send the web request
        UnityWebRequest www = UnityWebRequest.Post(registerUrl, form);
        yield return www.SendWebRequest();

        // Display the error if the request could not complete successfully
        if (www.isNetworkError || www.isHttpError) {
            ShowError(www.error);
        }
        
        // Pass validation on to verify the request
        else {
            ValidateAuthentication(www.downloadHandler.text);
        }
    }

    // Validates whether a user is logged in or not
    void ValidateAuthentication(string response) {
        JSONNode data = JSON.Parse(response);

        // If the response was not successful
        if (!bool.Parse(data["success"])) {

            // Some errors are returned with additional information to be parsed:  error_code|3,4,5
            string tmp = data["error"].ToString().Remove(0, 1).Remove(data["error"].ToString().Length - 2, 1);
            string[] error = tmp.Split('|');
            string[] parameters = {};
            string message = error[0];
            if (error.Length > 1) {
                parameters = error[1].Split(',');
            }

            // Show the error to the user
            ShowError(LocalisationManager.instance.GetValue(message, parameters));
            StopCoroutine("LoggingInTimer");
        }
        
        // The response was successful; we can expect a 'user' array
        else {
            LoginUser(data["user"]);
        }
    }

    // Logs the user in by storing their details in an Account class
    void LoginUser(JSONNode user) {
        StopCoroutine("LoggingInTimer");
        UserManager.Instance.account = new Account(user["_id"], user["username"], user["email"], user["created_at"], int.Parse(user["xp"]), user["lastGameID"], user["sessionToken"]);
        LoggedIn = true;
        mainMenuManager.Prepare();
    }

    // A fallback to time the user out if a web request is not completed after 7 seconds
    IEnumerator LoggingInTimer() {
        yield return new WaitForSeconds(7f);
        ShowError(LocalisationManager.instance.GetValue("login_error_timeout"));
    }
}
