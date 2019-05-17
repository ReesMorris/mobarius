using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This script contains variables essential to minion waypoint traversal
*/
/// <summary>
/// This script contains variables essential to minion waypoint traversal.
/// </summary>
[System.Serializable]
public class MinionWaypoints {

    // Public variables
    public Transform spawnPosition;
    public Transform[] destinations;
}
