using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ability {

    public string name;
    public AbilityHandler.Abilities abilityKey;
    [TextArea(3, 12)] public string desc;
    public Sprite icon;

    [Header("Configs")]
    public float cost;
    public float range;
    public float cooldown;
    public float speed;

    [Header("Damage")]
    public float physicalDamage;
    public float magicDamage;
}
