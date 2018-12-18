using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SimpleJSON;
using UnityEngine.Networking;
using TMPro;

public class MainMenuManager : MonoBehaviour {

    [Header("UI")]
    public GameObject mainMenuUI;

    [Header("User")]
    public TMP_Text username;
    public TMP_Text level;
    public Image xp;
    public TMP_Text status;

    [Header("Social")]
    public Button socialButton;
    public GameObject socialFeed;

    [Header("News")]
    public string newsUrl;
    public Article[] articles;
    public GameObject newsSection2;

    private int imagesLeftToLoad;
    private int thingsBeforeReady;
    private UserManager userManager;
    private LobbyNetwork lobbyNetwork;
    private AuthenticationManager authenticationManager;
    private EventSystem system;
    private UIHandler UIHandler;

    void Start() {
        UserManager.onXPChanged += SetUserXP;
        system = EventSystem.current;
        userManager = GetComponent<UserManager>();
        lobbyNetwork = GetComponent<LobbyNetwork>();
        authenticationManager = GetComponent<AuthenticationManager>();
        socialButton.onClick.AddListener(OnSocialButtonClick);
        UIHandler = GetComponent<UIHandler>();
        authenticationManager.loginUsername.Select();
    }

    void Update() {
        // Allow tab cycling between fields (src: https://forum.unity.com/threads/tab-between-input-fields.263779/#post-2234066)
        if (Input.GetKeyDown(KeyCode.Tab)) {
            Selectable next = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ?
            system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp() :
            system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
            if (next != null) {
                InputField inputfield = next.GetComponent<InputField>();
                if (inputfield != null)
                    inputfield.OnPointerClick(new PointerEventData(system));
                system.SetSelectedGameObject(next.gameObject);
            } else {
                next = Selectable.allSelectables[0];
                system.SetSelectedGameObject(next.gameObject);
            }
        }

        // Allow ENTER key to submit the form (enter for Win; return for Mac)
        if(Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)) {
            if(!authenticationManager.LoggedIn) {
                if(UIHandler.ErrorShowing) {
                    UIHandler.ErrorButtonClick();
                } else {
                    if (authenticationManager.loginModal.activeSelf)
                        authenticationManager.OnLoginButtonClick();
                    else
                        authenticationManager.OnRegisterButtonClick();
                }
            }
        }
    }


    // Called when the social section is clicked
    void OnSocialButtonClick() {
        socialFeed.gameObject.SetActive(!socialFeed.gameObject.activeSelf);
        newsSection2.SetActive(!newsSection2.activeSelf);
    }

    // Called once a user has authenticated logging in
    public void Prepare() {
        StartCoroutine("FetchNews");
        lobbyNetwork.ConnectToNetwork();
        SetupUser(userManager.account);
    }

    // Called by all processes that need to do something before the user can move to the lobby
    public void Preparing() {
        thingsBeforeReady++;
    }

    // Called when a process is ready; preparation will be complete when all processes are ready
    public void Ready() {
        thingsBeforeReady--;
        if (thingsBeforeReady == 0) {
            mainMenuUI.SetActive(true);
        }
    }

    // Called to fetch the news articles for the main menu
    IEnumerator FetchNews() {
        Preparing();
        UnityWebRequest www = UnityWebRequest.Get(newsUrl);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            print(www.error);
        }
        else {
            JSONNode data = JSON.Parse(www.downloadHandler.text);
            for (int i = 0; i < Mathf.Min(data.Count, articles.Length); i++) {
                articles[i].title.text = data[i]["title"];
                StartCoroutine(SetIcon(i, data[i]["image"]));
                imagesLeftToLoad++;
            }
        }
    }

    // Called to grab the news icon from the specified URL; will call ready() once all are loaded
    IEnumerator SetIcon(int index, string url) {
        WWW www = new WWW(url);
        yield return www;
        articles[index].image.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
        articles[index].image.color = Color.white;
        imagesLeftToLoad--;

        if (imagesLeftToLoad == 0) {
            Ready();
        }
    }

    // Called to set the user information up; will call ready() once all complete
    void SetupUser(Account user) {
        Preparing();
        username.text = user.username;
        SetUserXP(user.xp);
        Ready();
    }

    /* XP and Level Systems */

    void SetUserXP(int newXP) {
        level.text = XPToLevel(newXP).ToString();
        xp.fillAmount = ProgressToLevel(newXP) - 0.07f; // to factor in that a 0.93 is the filled amount
    }
    int Equate(float xp) {
        return (int)Mathf.Floor(xp + 300 * Mathf.Pow(2, xp / 7));
    }
    int LevelToXP(int level) {
        float xp = 0;
        for (int i = 1; i < level; i++)
            xp += Equate(i);
        return (int)Mathf.Floor(xp / 4f);
    }
    int XPToLevel(int xp) {
        int level = 1;
        while (LevelToXP(level) < xp)
            level++;
        return level - 1;
    }
    float ProgressToLevel(float xp) {
        if (xp == 0)
            return 0;
        int level = 1;
        while (LevelToXP(level) < xp)
            level++;

        float lastLevel = LevelToXP(level - 1);
        float nextLevel = LevelToXP(level);
        return (xp - lastLevel) / (nextLevel - lastLevel);
    }
}