using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerChampion : MonoBehaviour {

    [Header("UI")]
    public TMP_Text healthText;

    Champion champion;
    PhotonView photonView;

    /*  General Code  */

	void Start () {
        photonView = GetComponent<PhotonView>();
        if(PhotonNetwork.player.IsLocal)
            photonView.RPC("Rename", PhotonTargets.All, PhotonNetwork.player.CustomProperties["championName"].ToString());

        champion = ChampionRoster.Instance.GetChampion(gameObject.name);
        champion.health = champion.maxHealth;
        champion.mana = champion.maxMana;
        TakeDamage(0f);
    }

    [PunRPC]
    void Rename(string newName) {
        gameObject.name = newName;
    }

    /*  General Health Handling  */

    void OnHealthChange() {
        champion.health = Mathf.Clamp(champion.health, 0, champion.maxHealth);
        if(champion.health == 0f) {
            Die();
        }
        photonView.RPC("UpdatePlayerHealth", PhotonTargets.All, champion.health);
    }

    [PunRPC]
    void UpdatePlayerHealth(float health) {
        healthText.text = health.ToString();
    }

    /*  Healing Handling  */

    public void Heal(float amount) {
        if(PhotonNetwork.isMasterClient) {
            champion.health += amount;
            OnHealthChange();
        }
    }

    /*  Damage & Death Handling  */

    public void TakeDamage(float amount) {
        if(PhotonNetwork.isMasterClient) {
            champion.health -= amount;
            OnHealthChange();
        }
    }

    void Die() {
        print("i am dead");
    }
}
