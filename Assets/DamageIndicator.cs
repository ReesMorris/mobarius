using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : MonoBehaviour {

    Image image;

	void Start () {
        image = GetComponent<Image>();
        if (PhotonNetwork.player.IsLocal)
            StartCoroutine("Resize");
	}

    IEnumerator Resize() {
        while(true) {
            if (image.fillAmount > 0)
                image.fillAmount -= 0.02f;
            yield return new WaitForSeconds(0.01f);
        }
    }
}
