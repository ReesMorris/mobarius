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
        yield return new WaitForSeconds(4f);
        deathOverlay.SetActive(false);
        playerChampion.PhotonView.RPC("Heal", PhotonTargets.All, playerChampion.Champion.maxHealth);
        playerChampion.PhotonView.RPC("GiveMana", PhotonTargets.All, playerChampion.Champion.maxMana);
        playerChampion.Respawn();
    }
	
}
