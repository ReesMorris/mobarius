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
    public AbilityHandler.AbilityTypes abilityType;
    public float cost;
    public float range;
    public float damageRadius;
    public float cooldown;
    public float speed;
    public int maxLevel = 5;
    int level = 1;
    public int Level { get { return level; } }

    [Header("Damage")]
    public AbilityDamage[] damage;

    // Called when levelling up this ability
    public void LevelUp() {
        level = Mathf.Min(level + 1, maxLevel);
    }

    // Returns the amount of damage this ability does at its current level
    public float GetDamage(string key) {
        return GetDamage(key, level);
    }

    // Called by other scripts to get the amount of damage this ability does at a specific level
    public float GetDamage(string key, int level) {
        foreach(AbilityDamage ad in damage) {
            if (ad.key == key) {
                return ad.GetDamage(level);
            }
        }
        return 0;
    }
}
