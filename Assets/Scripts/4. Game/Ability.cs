using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ability {

    public string name;
    [TextArea(3, 12)] public string desc;
    public Sprite icon;

    [Header("Configs")]
    public float cost;
    public float range;
    public float cooldown;
    public float speed;
    public float damage;
}
