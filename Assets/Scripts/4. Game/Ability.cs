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

    [Header("Damage")]
    public AbilityDamage[] damage;

    // Returns the amount of damage this ability does at its current level
    public float GetDamage(Champion champion, string key) {
        return GetDamage(key, AbilityHandler.Instance.GetAbilityLevel(champion, this));
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
