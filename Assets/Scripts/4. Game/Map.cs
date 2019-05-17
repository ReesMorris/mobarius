using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This class contains variables for the Map
*/
/// <summary>
/// This class contains variables for the Map.
/// </summary>
[System.Serializable]
public class Map {

    // Public variables
    public string name;
    public byte maxPlayers;
    public GameObject map;
    public GameObject[] redSpawns;
    public GameObject[] blueSpawns;
    public MinionWaypoints[] redMinions;
    public MinionWaypoints[] blueMinions;
    public MapProperties properties;
}
