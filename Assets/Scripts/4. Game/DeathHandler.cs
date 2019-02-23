using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathHandler : MonoBehaviour {

    public static DeathHandler Instance;

	void Start () {
        Instance = this;
	}

    public void OnDeath(PlayerChampion playerChampion) {
        Camera.main.GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionCurves>().saturation = 0;
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
        playerChampion.PhotonView.RPC("Heal", PhotonTargets.All, playerChampion.Champion.maxHealth);
        playerChampion.PhotonView.RPC("GiveMana", PhotonTargets.All, playerChampion.Champion.maxMana);
        Camera.main.GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionCurves>().saturation = 1;
        playerChampion.Respawn();
        GameUIHandler.Instance.deathBar.SetActive(false);
    }
	
}
