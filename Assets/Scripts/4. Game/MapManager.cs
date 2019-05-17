using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This script contains functions to handle Maps
*/
/// <summary>
/// This script contains functions to handle Maps.
/// </summary>
public class MapManager : MonoBehaviour {

    // Public variables
    public static MapManager Instance;

    public Map[] maps;

    // Allow other scripts to access this when the game starts.
    void Start() {
        Instance = this;
    }

    /// <summary>
    /// Finds and returns a Map.
    /// </summary>
    /// <param name="name">The name of the Map to be found</param>
    /// <returns>
    /// A Map object if one can be found with the same name; null otherwise.
    /// </returns>
    public Map GetMap(string name) {
        foreach(Map map in maps) {
            if (map.name == name)
                return map;
        }
        return null;
    }

    /// <summary>
    /// Returns the properties of the current map.
    /// </summary>
    public MapProperties GetMapProperties() {
        return GameHandler.Instance.currentMap.properties;
    }
}