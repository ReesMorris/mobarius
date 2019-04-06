using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

public class VersionManager : MonoBehaviour {

    public string currentVersion;
    public bool skipVersionChecking;
    public string versionCheckerUrl;
    public string downloadUrl;
    public TMP_Text versionText;
    [HideInInspector] public bool outdated;

    private UIHandler UIHandler;

    void Start() {
        UIHandler = GetComponent<UIHandler>();
        outdated = !skipVersionChecking;
        versionText.text = LocalisationManager.instance.GetValue("auth_version", currentVersion);
        UIHandler.ShowInfobox(LocalisationManager.instance.GetValue("auth_verifying_version"), true);
        StartCoroutine(CompareVersions());
    }

    IEnumerator CompareVersions() {
        if (skipVersionChecking) {
            UIHandler.ShowInfobox(LocalisationManager.instance.GetValue("auth_validation_skip"), false);
            yield return new WaitForSeconds(2f);
            UIHandler.HideInfobox();
        } else {
            UnityWebRequest www = UnityWebRequest.Get(versionCheckerUrl);
            yield return www.SendWebRequest();
            UIHandler.HideInfobox();
            if (www.isNetworkError || www.isHttpError) {
                UIHandler.ShowError(www.error);
            } else {
                if (currentVersion != www.downloadHandler.text) {
                    UIHandler.ShowError(LocalisationManager.instance.GetValue("outdated_version", www.downloadHandler.text), downloadUrl);
                } else {
                    outdated = false;
                }
            }
        }
    }
}