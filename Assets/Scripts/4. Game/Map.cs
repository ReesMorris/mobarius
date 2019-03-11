using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Map {

    public string name;
    public byte maxPlayers;
    public GameObject map;
    public GameObject[] redSpawns;
    public GameObject[] blueSpawns;
    public MinionWaypoints[] redMinions;
    public MinionWaypoints[] blueMinions;
    public MapProperties properties;
}
