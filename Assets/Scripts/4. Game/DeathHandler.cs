using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathHandler : MonoBehaviour {

    public static DeathHandler Instance;
    public GameObject deathOverlay;

    PhotonView photonView;

	void Start () {
        Instance = this;
        photonView = GetComponent<PhotonView>();
	}

    public void OnDeath(PlayerChampion playerChampion) {
        deathOverlay.SetActive(true);
        StartCoroutine(RespawnTimer(playerChampion));
    }

    IEnumerator RespawnTimer(PlayerChampion playerChampion) {
        GameUIHandler.Instance.deathBar.SetActive(true);
        float timeLeft = 6;
        float reductionSpeed = 0.1f;
        while (timeLeft > 0) {
            timeLeft -= reductionSpeed;
            int timeLeftInt = (int)Mathf.Ceil(timeLeft);
            if (timeLeftInt > 0) {
                GameUIHandler.Instance.deathBarText.text = "Respawning in " + timeLeftInt + " seconds";
                if (timeLeftInt == 1)
                    GameUIHandler.Instance.deathBarText.text = "Respawning in " + timeLeftInt + " second";
            }
            yield return new WaitForSeconds(reductionSpeed);

        }
        deathOverlay.SetActive(false);
        playerChampion.PhotonView.RPC("Heal", PhotonTargets.All, playerChampion.Champion.maxHealth);
        playerChampion.PhotonView.RPC("GiveMana", PhotonTargets.All, playerChampion.Champion.maxMana);
        playerChampion.Respawn();
        GameUIHandler.Instance.deathBar.SetActive(false);
    }
	
}
