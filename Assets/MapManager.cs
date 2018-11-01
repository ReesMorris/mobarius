using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {

    public Map[] maps;

    public Map GetMap(string name) {
        foreach(Map map in maps) {
            if (map.name == name)
                return map;
        }
        return null;
    }
}