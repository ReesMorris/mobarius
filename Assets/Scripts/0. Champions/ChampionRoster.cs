﻿using System.Collections.Generic;
using UnityEngine;

public class ChampionRoster : MonoBehaviour {

    static public ChampionRoster Instance;

    int getChampionsAttempts;

    Champion[] champions;

    void Start() {
        Instance = this;
    }

    public Champion[] GetChampions() {
        champions = Resources.FindObjectsOfTypeAll(typeof(Champion)) as Champion[];
        if(champions.Length == 0 && getChampionsAttempts < 10) {
            Debug.LogWarning("No champions found; retrying attempt" + getChampionsAttempts++);
            GetChampions();
        }
        List<Champion> availableChampions = new List<Champion>();
        List<Champion> unownedChampions = new List<Champion>();

        // Find all champions and mark them as either owned (sorted) or unowned (unsorted)
        foreach (Champion champion in champions) {
            if(champion.IsAvailable) {
                if(champion.IsOwned) {
                    availableChampions.Add(champion);
                } else {
                    unownedChampions.Add(champion);
                }
            }
        }

        // Put unowned champions last
        foreach(Champion champion in unownedChampions) {
            availableChampions.Add(champion);
        }

        return availableChampions.ToArray();
    }

    public Champion GetChampion(string name) {
        foreach(Champion champion in champions) {
            if (champion.championName == name)
                return champion;
        }
        return null;
    }
}
