using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

/*
    Handles version control management to check for outdated game versions
*/
/// <summary>
/// Handles version control management to check for outdated game versions.
/// </summary>
public class VersionManager : MonoBehaviour {

    // Public variables
    public string currentVersion;
    public bool skipVersionChecking;
    public string versionCheckerUrl;
    public string downloadUrl;
    public TMP_Text versionText;
    [HideInInspector] public bool outdated;

    // Private variables
    private UIHandler UIHandler;

    // Assign private variables on game start.
    void Start() {
        UIHandler = GetComponent<UIHandler>();
        outdated = !skipVersionChecking;
        versionText.text = LocalisationManager.instance.GetValue("auth_version", currentVersion);
        UIHandler.ShowInfobox(LocalisationManager.instance.GetValue("auth_verifying_version"), true);

        // Once the game starts, check to see if the hard-coded version matches the external database
        StartCoroutine(CompareVersions());
    }

    // Checks to see whether the hard-coded version matches the external database
    IEnumerator CompareVersions() {

        // If version checking is disabled in this build
        // Shows a message for two seconds before disappearing
        if (skipVersionChecking) {
            UIHandler.ShowInfobox(LocalisationManager.instance.GetValue("auth_validation_skip"), false);
            yield return new WaitForSeconds(2f);
            UIHandler.HideInfobox();
        }
        
        // If version checking is enabled in this build
        else {
            // Attempt to pull the latest version string from the web API
            UnityWebRequest www = UnityWebRequest.Get(versionCheckerUrl);
            yield return www.SendWebRequest();
            UIHandler.HideInfobox();

            // If an error occurs during this process, show it to the user
            if (www.isNetworkError || www.isHttpError) {
                UIHandler.ShowError(www.error);
            }
            
            // If no errors occur during this process
            else {

                // If the two versions are outdated, show a warning to the user
                if (currentVersion != www.downloadHandler.text) {
                    UIHandler.ShowError(LocalisationManager.instance.GetValue("outdated_version", www.downloadHandler.text), downloadUrl);
                }
                
                // If the two versions match, the game is not outdated
                else {
                    outdated = false;
                }
            }
        }
    }
}