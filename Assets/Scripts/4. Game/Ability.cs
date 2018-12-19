using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ability {

    public string name;
    [TextArea(3, 12)] public string desc;
    public float cost;
    public Sprite icon;

}
