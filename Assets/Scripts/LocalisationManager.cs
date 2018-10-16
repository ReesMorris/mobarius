using UnityEngine;
using System.Xml;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class LocalisationManager : MonoBehaviour {

    public string currentLanguage = "en-gb";
    public static LocalisationManager instance;

    private Dictionary<string, string> localisations;
    private string defaultIso;
    private bool isReady;

    // Allows us to create instances
    void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }
    }

    void Start () {
        defaultIso = currentLanguage;
        localisations = new Dictionary<string, string>();
        SetLanguage(defaultIso);
    }

    // Will either return the associated value, or the key itself if not found
    public string GetValue(string key, params string[] formatting) {
        return localisations.ContainsKey(key) ? string.Format(localisations[key], formatting) : key;
    }

    // Sets the language to be the iso specified
    void SetLanguage(string iso) {
        string filePath = Path.Combine(Application.streamingAssetsPath, "Languages/" + iso + ".xml");
        if (File.Exists(filePath)) {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);

            XmlNodeList nodeList = xmlDoc.DocumentElement.SelectNodes("translation");
            foreach (XmlNode node in nodeList) {
                string key = node.Attributes["key"].Value;
                string val = node.InnerText;
                localisations.Add(key, val);
            }
            isReady = true;
        } else {
            SetLanguage(defaultIso);
        }
    }
}
