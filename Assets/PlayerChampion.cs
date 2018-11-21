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

    /*  General Code  */

	void Start () {
        photonView = GetComponent<PhotonView>();
        if(PhotonNetwork.player.IsLocal) {
            photonView.RPC("Rename", PhotonTargets.All, PhotonNetwork.player.CustomProperties["championName"].ToString());
            usernameText.text = photonView.owner.NickName;
        }

        champion = ChampionRoster.Instance.GetChampion(gameObject.name);
        champion.health = oldHealth = champion.maxHealth;
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

        // Healing done
        else {

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
