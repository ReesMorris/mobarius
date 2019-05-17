using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/*
    The script responsible for displaying text to the local user regarding the status of matchmaking
*/
/// <summary>
/// The script responsible for displaying text to the local user regarding the status of matchmaking.
/// </summary>
public class SearchingLabel : MonoBehaviour {

    // Public variables
    public static SearchingLabel Instance;

    public TMP_Text titleText;
    public TMP_Text timer;
    public Button cancelButton;

    // Private variables
    int timeElapsed;
    string searching;
    bool isSearching;

    // Allow other scripts to reference this when the game starts
    void Start() {
        Instance = this;
    }

    /// <summary>
    /// Displays the 'searching' text and timer.
    /// </summary>
    /// <remarks>
    /// Called when a player begins searching for a game.
    /// </remarks>
    public void OnSearchingStart() {
        isSearching = true;
        timer.gameObject.SetActive(true);
        cancelButton.gameObject.SetActive(true);
        searching = LocalisationManager.instance.GetValue("lobby_searching");
        StartCoroutine("RunTimer");
        StartCoroutine("SearchingText");
    }

    /// <summary>
    /// Stops displaying the 'searching' text and resets the timer
    /// </summary>
    /// <remarks>
    /// Called when a player stops searching for a game (searching cancelled)
    /// </remarks>
    public void OnSearchingStop() {
        isSearching = false;
        timeElapsed = 0;
        timer.text = "00:00";
        titleText.text = LocalisationManager.instance.GetValue("lobby_play");
        timer.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
        StopCoroutine("RunTimer");
        StopCoroutine("SearchingText");
        ScaleOnHover scaleOnHover = GetComponent<ScaleOnHover>();
        if(scaleOnHover != null)
            GetComponent<ScaleOnHover>().enabled = true;
    }

    /// <summary>
    /// Pauses the searching text and timer; does not reset their values
    /// </summary>
    /// <remarks>
    /// Called to pause searching for a game
    /// </remarks>
    public void OnSearchingPause() {
        isSearching = false;
        StopCoroutine("RunTimer");
        StopCoroutine("SearchingText");
    }

    // Displays searching text with flashing ellipses
    IEnumerator SearchingText() {
        int i = 0;
        while(true) {
            yield return new WaitForSeconds(0.2f);
            i++;
            i %= 4;
            titleText.text = searching;
            for (int j = 0; j < i; j++) {
                titleText.text += ".";
            }
        }
    }

    // Displays the amount of time elapsed since searching started
    IEnumerator RunTimer() {
        while(true) {
            yield return new WaitForSeconds(1f);
            if(isSearching)
                timeElapsed++;

            TimeSpan time = TimeSpan.FromSeconds(timeElapsed);
            timer.text = time.Minutes.ToString("00") + ":" + time.Seconds.ToString("00");
        }
    }
}
