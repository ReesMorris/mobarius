using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerChampion : MonoBehaviour {

    [Header("UI")]
    public TMP_Text usernameText;
    public Image healthBarFill;
    public Image damageIndicator;
    public Transform hundredsContainer;
    public GameObject healthBarLine;
    public Color enemyHealthColour;

    Champion champion;
    PhotonView photonView;
    float oldHealth;
    GameUIHandler gameUIHandler;

    /*  General Code  */

	void Start () {
        photonView = GetComponent<PhotonView>();
        if (PhotonNetwork.player.IsLocal) {
            gameUIHandler = GameObject.Find("GameManager").GetComponent<GameUIHandler>();
            photonView.RPC("Rename", PhotonTargets.All, PhotonNetwork.player.CustomProperties["championName"].ToString());
            usernameText.text = photonView.owner.NickName;
            champion = ScriptableObject.CreateInstance<Champion>();
            champion.Init(ChampionRoster.Instance.GetChampion(gameObject.name), PhotonNetwork.player.NickName);
            champion.health = oldHealth = champion.maxHealth;
            champion.mana = champion.maxMana;
            photonView.RPC("UpdatePlayerHealth", PhotonTargets.All, new object[] { champion.health, champion.maxHealth });
        }
    }

    [PunRPC]
    void Rename(string newName) {
        gameObject.name = newName;
    }

    [PunRPC]
    void Damage(float amount) {
        print(PhotonNetwork.playerName + " is taking damage");
        champion.health = Mathf.Max(champion.health - amount, 0f);
        if (champion.health <= 0f)
            print("I am dead");
        gameUIHandler.UpdateStats(champion);
        photonView.RPC("UpdatePlayerHealth", PhotonTargets.All, new object[] { champion.health, champion.maxHealth });
    }

    [PunRPC]
    void Heal(float amount) {
        champion.health = Mathf.Min(champion.health + amount, champion.maxHealth);
        gameUIHandler.UpdateStats(champion);
        photonView.RPC("UpdatePlayerHealth", PhotonTargets.All, new object[] { champion.health, champion.maxHealth });
    }


    [PunRPC]
    void UpdatePlayerHealth(float health, float maxHealth) {
        float oldFillAmount = healthBarFill.fillAmount;

        healthBarFill.fillAmount = health / maxHealth;

        // Damage taken
        if(oldHealth > health) {
            damageIndicator.fillAmount = oldFillAmount;
        }

        // Health bars
        while(hundredsContainer.childCount < (Mathf.Floor(maxHealth / 100))) {
            Instantiate(healthBarLine, hundredsContainer);
        }

        // Colour
        if (photonView.owner.GetTeam() != PhotonNetwork.player.GetTeam())
            healthBarFill.color = enemyHealthColour;

        oldHealth = health;
    }
}
