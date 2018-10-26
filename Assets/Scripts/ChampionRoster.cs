using System.Collections.Generic;
using UnityEngine;

public class ChampionRoster : MonoBehaviour {

    public Champion[] champions;

    public Champion[] GetChampions() {
        List<Champion> availableChampions = new List<Champion>();
        List<Champion> unownedChampions = new List<Champion>();

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
}
