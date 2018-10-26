using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour {

    public TMP_Text titleText;
    public TMP_Text timer;
    public Button cancelButton;

    int timeElapsed;
    string searching;

    public void OnSearchingStart() {
        timer.gameObject.SetActive(true);
        cancelButton.gameObject.SetActive(true);
        GetComponent<ScaleOnHover>().ResetScale();
        GetComponent<ScaleOnHover>().enabled = false;
        searching = LocalisationManager.instance.GetValue("lobby_searching");
        StartCoroutine("RunTimer");
        StartCoroutine("SearchingText");
    }

    public void OnSearchingStop() {
        titleText.text = LocalisationManager.instance.GetValue("lobby_play");
        timer.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
        StopCoroutine("RunTimer");
        StopCoroutine("SearchingText");
        GetComponent<ScaleOnHover>().enabled = true;
        timeElapsed = 0;
    }

    public void OnSearchingPause() {
        StopCoroutine("RunTimer");
        StopCoroutine("SearchingText");
    }

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

    IEnumerator RunTimer() {
        while(true) {
            yield return new WaitForSeconds(1f);
            timeElapsed++;

            TimeSpan time = TimeSpan.FromSeconds(timeElapsed);
            timer.text = time.Minutes.ToString("00") + ":" + time.Seconds.ToString("00");
        }
    }
}
