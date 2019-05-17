using UnityEngine;
using System.Xml;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;

/*
    The main script used to handle localisation
*/
/// <summary>
/// The main script used to handle localisation.
/// </summary>
public class LocalisationManager : MonoBehaviour {

    // Public variables
    public string currentLanguage = "en-gb";
    public static LocalisationManager instance;

    // Private variables
    private Dictionary<string, string> localisations;
    private string defaultIso;

    // Creates an instance of this script once the game begins.
    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    // Assigns the private variables once the game begins.
    void Start () {
        defaultIso = currentLanguage;
        localisations = new Dictionary<string, string>();
        SetLanguage(defaultIso);
    }

    /// <summary>
    /// Finds a localisation key in the dictionary for the current language.
    /// </summary>
    /// <returns>
    /// The associated localisation value if the key exists in the dictionary; the key itself if the key could not be found.
    /// </returns>
    /// <param name="key">The localisation key in the XML file</param>
    /// <param name="formatting">An array of parameters to replace the {x} values in the localisation text</param>
    public string GetValue(string key, params string[] formatting) {
        return localisations.ContainsKey(key) ? string.Format(localisations[key], formatting) : key;
    }

    /// <summary>
    /// Sets the current language.
    /// </summary>
    /// <remarks>
    /// The iso must exist in the StreamingAssets folder as a .xml file.
    /// The parameter must match the name exactly, without the file extension.
    /// If the XML file cannot be found, the defaultIso will be used instead.
    /// </remarks>
    /// <param name="iso">An iso language code</param>
    public void SetLanguage(string iso) {
        string filePath = Path.Combine(Application.streamingAssetsPath, "Languages/" + iso + ".xml");

        // Check that the file exists
        if (File.Exists(filePath)) {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);

            // Update the dictionary to contain all key-value pairs from the XML document
            XmlNodeList nodeList = xmlDoc.DocumentElement.SelectNodes("translation");
            foreach (XmlNode node in nodeList) {
                string key = node.Attributes["key"].Value;
                string val = node.InnerText;
                localisations.Add(key, val);
            }
        } else {
            SetLanguage(defaultIso); // file doesn't exist, let's use the default language instead
        }
    }
}
