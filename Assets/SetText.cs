using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetText : MonoBehaviour {

    private TMP_Text textObj;

	void Start () {
        textObj = GetComponent<TMP_Text>();
    }

    public void Set(string text) {
        GetComponent<PhotonView>().RPC("ChangeText", PhotonTargets.All, new object[] { text });
    }

    [PunRPC]
    void ChangeText(string text) {
        textObj.text = text;
    }
}
