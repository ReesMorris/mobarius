using System.Collections.Generic;
using UnityEngine;
using System.IO;

/*
    Handles fetching a list of Champions and individual Champions
*/
/// <summary>
/// Handles fetching a list of Champions and individual Champions.
/// </summary>
public class ChampionRoster : MonoBehaviour {

    // Public variables
    static public ChampionRoster Instance;
    public Champion[] champions;

    /// <summary>
    /// Creates an instance of this script once the game begins.
    /// </summary>
    void Start() {
        Instance = this;
    }

    /// <summary>
    /// Fetches all Champions, sorts them into 'owned' and 'unowned'.
    /// </summary>
    /// <remarks>
    /// Does not include Champions in which 'IsAvailable' is set to false.
    /// </remarks>
    /// <returns>
    /// A Champion array of all Champions, organised by ones which are available for the local player first.
    /// </returns>
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

    /// <summary>
    /// Fetches a Champion based on their name.
    /// </summary>
    /// <returns>
    /// A Champion class if found; null if not.
    /// </returns>
    /// <param name="name">The championName variable of the Champion</param>
    public Champion GetChampion(string name) {
        foreach(Champion champion in champions) {
            if (champion.championName == name)
                return champion;
        }
        return null;
    }
}
