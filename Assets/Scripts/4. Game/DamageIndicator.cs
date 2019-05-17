using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    The script handling the red damage indicator displayed when a Champion takes damage
*/
/// <summary>
/// The script handling the red damage indicator displayed when a Champion takes damage.
/// </summary>
public class DamageIndicator : MonoBehaviour {

    // Private variables
    Image image;

    // Constantly run this script from when the game begins.
	void Start () {
        StartResize();
	}

    /// <summary>
    /// Scales down the red damage indicator on a player's health bar.
    /// </summary>
    public void StartResize() {
        image = GetComponent<Image>();
        if (PhotonNetwork.player.IsLocal)
            StartCoroutine("Resize");
    }

    // Forever attempt to scale down the red damage indicator, as long as it actually has a length
    IEnumerator Resize() {
        while (true) {
            if (image.fillAmount > 0)
                image.fillAmount -= 0.02f;
            yield return new WaitForSeconds(0.01f);
        }
    }
}
