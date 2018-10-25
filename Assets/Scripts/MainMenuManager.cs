using System.Collections;
using UnityEngine;
using UnityEngine.UI;
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

    void Start() {
        userManager = GetComponent<UserManager>();

        socialButton.onClick.AddListener(OnSocialButtonClick);

        ProgressToLevel(648);
    }

    void OnSocialButtonClick() {
        socialFeed.gameObject.SetActive(!socialFeed.gameObject.activeSelf);
        newsSection2.SetActive(!newsSection2.activeSelf);
    }

    // Called once a user has authenticated logging in
    public void Prepare() {
        StartCoroutine("FetchNews");
        SetupUser(userManager.account);
    }

    void Preparing() {
        thingsBeforeReady++;
    }

    void Ready() {
        thingsBeforeReady--;
        if (thingsBeforeReady == 0) {
            mainMenuUI.SetActive(true);
        }
    }

    IEnumerator FetchNews() {
        Preparing();
        WWWForm form = new WWWForm();
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

    void SetupUser(Account user) {
        Preparing();
        username.text = user.username;
        level.text = XPToLevel(user.xp).ToString();
        xp.fillAmount = ProgressToLevel(user.xp) - 0.07f; // to factor in that a 0.93 is the filled amount
        Ready();
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
        int level = 1;
        while (LevelToXP(level) < xp)
            level++;

        float lastLevel = LevelToXP(level - 1);
        float nextLevel = LevelToXP(level);
        return (xp - lastLevel) / (nextLevel - lastLevel);
    }
}