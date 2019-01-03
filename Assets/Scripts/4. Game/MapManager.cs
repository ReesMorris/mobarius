using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {

    public static MapManager Instance;

    public Map[] maps;

    void Start() {
        Instance = this;
    }

    public Map GetMap(string name) {
        foreach(Map map in maps) {
            if (map.name == name)
                return map;
        }
        return null;
    }
}