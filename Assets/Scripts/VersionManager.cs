﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

public class VersionManager : MonoBehaviour {

    public string currentVersion;
    public string versionCheckerUrl;
    public string downloadUrl;
    public TMP_Text versionText;
    [HideInInspector] public bool outdated;

    private UIHandler UIHandler;

    void Start() {
        UIHandler = GetComponent<UIHandler>();
        outdated = true;
        versionText.text = LocalisationManager.instance.GetValue("auth_version", currentVersion);
        UIHandler.ShowInfobox(LocalisationManager.instance.GetValue("auth_verifying_version"), true);
        StartCoroutine(CompareVersions());
    }

    IEnumerator CompareVersions() {
        UnityWebRequest www = UnityWebRequest.Get(versionCheckerUrl);
        yield return www.SendWebRequest();
        UIHandler.HideInfobox();
        if (www.isNetworkError || www.isHttpError) {
            UIHandler.ShowError(www.error);
        }
        else {
            if(currentVersion != www.downloadHandler.text) {
                UIHandler.ShowError(LocalisationManager.instance.GetValue("outdated_version", www.downloadHandler.text), downloadUrl);
            } else {
                outdated = false;
            }
        }
    }
}