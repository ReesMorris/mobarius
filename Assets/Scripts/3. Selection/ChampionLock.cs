using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChampionLock : MonoBehaviour {

    TMP_Text title;
    Image image;
    ChampionSelect championSelect;

    void Start() {
        title = transform.Find("Title").GetComponent<TMP_Text>();
        image = transform.Find("Image").GetComponent<Image>();
        championSelect = GameObject.Find("GameManager").GetComponent<ChampionSelect>();
    }

    public void LockIn(Champion champion) {
        GetComponent<PhotonView>().RPC("Lock", PhotonTargets.All, new object[] { champion.championName });
    }

    public void SetPosition(bool isLeft, string playerName) {
        GetComponent<PhotonView>().RPC("Position", PhotonTargets.All, new object[] { isLeft, playerName });
    }

    [PunRPC]
    void Position(bool isLeft, string playerName) {
        if (championSelect == null)
            Start();

        if (isLeft) {
            gameObject.transform.SetParent(championSelect.leftColumn);
        } else {
            gameObject.transform.SetParent(championSelect.rightColumn);
        }
        gameObject.name = playerName;
        gameObject.GetComponent<RectTransform>().localScale = Vector3.one;
        gameObject.transform.Find("Username").GetComponent<TMP_Text>().text = playerName;
    }

    [PunRPC]
    void Lock(string championName) {
        image.sprite = Resources.Load<Sprite>("Champions/" + championName);
        title.text = championName;
        championSelect.OnPlayerLock();
    }
}
