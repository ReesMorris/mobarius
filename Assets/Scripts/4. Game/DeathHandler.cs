using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    The script handling player death and respawning
*/
/// <summary>
/// The script handling player death and respawning.
/// </summary>
public class DeathHandler : MonoBehaviour {

    // Public variables
    public static DeathHandler Instance;

    // Allow other scripts to access this when the game starts.
	void Start () {
        Instance = this;
	}

    /// <summary>
    /// Saturates the game screen and begins the death timer.
    /// </summary>
    /// <param name="playerChampion">The champion that died</param>
    /// <remarks>
    /// Called when the local Champion dies.
    /// </remarks>
    public void OnDeath(PlayerChampion playerChampion) {
        Camera.main.GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionCurves>().saturation = 0;
        StartCoroutine(RespawnTimer(playerChampion));
    }

    // The respawn timer
    IEnumerator RespawnTimer(PlayerChampion playerChampion) {
        GameUIHandler.Instance.deathBar.SetActive(true);
        float timeLeft = 6;
        float reductionSpeed = 0.1f;

        // While the player is still dead, update the death countdown timer
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

        // Once the death timer has ended, respawn the player on the network
        playerChampion.PhotonView.RPC("Heal", PhotonTargets.All, playerChampion.Champion.maxHealth);
        playerChampion.PhotonView.RPC("GiveMana", PhotonTargets.All, playerChampion.Champion.maxMana);
        Camera.main.GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionCurves>().saturation = 1;
        playerChampion.Respawn();
        GameUIHandler.Instance.deathBar.SetActive(false);
    }
	
}
