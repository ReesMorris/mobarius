using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    A class containing the properties a Map has
*/
/// <summary>
/// A class containing the properties a Map has.
/// </summary>
[System.Serializable]
public class MapProperties {

    [Header("Champion")]
    public int startingLevel = 1;
    public bool XPEnabled;

    [Header("Minions")]
    public int minionSpawnTime;
    public int minionSpawnDelay;

    [Header("Towers")]
    public float nexusDamagePerSec;

    [Header("Display")]
    public PlayerCamera.CameraDisplays display;
}
