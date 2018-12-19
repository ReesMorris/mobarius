﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatRegeneration : MonoBehaviour {

    Champion champion;
    PhotonView photonView;

    void Start() {
        photonView = GetComponent<PhotonView>();

        // Only heal the local player
        if(photonView.isMine) {
            champion = GetComponent<PlayerChampion>().Champion;
            StartCoroutine("RegenHealth");
            StartCoroutine("RegenMana");
        }
    }

    // Will regen the player's health every 0.5 seconds, equivalent to their healthRegen variable
    IEnumerator RegenHealth() {
        while(true) {
            photonView.RPC("Heal", PhotonTargets.All, champion.healthRegen / 5f);
            yield return new WaitForSeconds(0.5f);
        }
    }

    // Will regfen the player's mana every 0.5 seconds, equivalent to their manaRegen variable
    IEnumerator RegenMana() {
        while(true) {
            photonView.RPC("GiveMana", PhotonTargets.All, champion.manaRegen / 5f);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
