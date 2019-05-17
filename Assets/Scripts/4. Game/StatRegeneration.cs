using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This script focuses on handling player stat regeneration
*/
/// <summary>
/// This script focuses on handling player stat regeneration.
/// </summary>
public class StatRegeneration : MonoBehaviour {

    // Private variables
    Champion champion;
    PlayerChampion playerChampion;
    PhotonView photonView;

    // Start the heal coroutines if the player is local
    void Start() {
        photonView = GetComponent<PhotonView>();

        // Only heal the local player
        if(photonView.isMine) {
            playerChampion = GetComponent<PlayerChampion>();
            champion = playerChampion.Champion;
            StartCoroutine("RegenHealth");
            StartCoroutine("RegenMana");
        }
    }

    // Will regen the player's health every 0.5 seconds, equivalent to their healthRegen variable
    IEnumerator RegenHealth() {
        while(true) {
            if(!playerChampion.IsDead) {
                photonView.RPC("Heal", PhotonTargets.All, champion.healthRegen / 5f);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    // Will regfen the player's mana every 0.5 seconds, equivalent to their manaRegen variable
    IEnumerator RegenMana() {
        while(true) {
            if(!playerChampion.IsDead) {
                photonView.RPC("GiveMana", PhotonTargets.All, champion.manaRegen / 5f);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}
