using System.Collections.Generic;
using UnityEngine;

public class ChampionRoster : MonoBehaviour {

    static public ChampionRoster Instance;
    public Champion[] champions;

    void Start() {
        Instance = this;
    }

    public Champion[] GetChampions() {
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
