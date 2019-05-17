using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    The class responsible for holding details about a Champion's single Ability
*/
/// <summary>
/// The class responsible for holding details about a Champion's single Ability.
/// </summary>
[System.Serializable]
public class Ability {

    // Public variables
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
    public float duration;
    public float speed;
    public int maxLevel = 5;

    [Header("Damage")]
    public AbilityDamage[] damage;

    /// <summary>
    /// Finds the damage this ability does at its current level.
    /// </summary>
    /// <param name="champion">The Champion class the damage value is for</param>
    /// <param name="key">The key identifier for the damage name</param>
    /// <remarks>
    /// Does not have a failsafe for incorrect parameters.
    /// </remarks>
    /// <returns>
    /// The amount of damage this ability does at its current level.
    /// </returns>
    public float GetDamage(Champion champion, string key) {
        return GetDamage(key, AbilityHandler.Instance.GetAbilityLevel(champion, this));
    }

    /// <summary>
    /// Finds the damage this ability does at a level.
    /// </summary>
    /// <param name="key">The key identifier for the damage name</param>
    /// <param name="level">The level of the ability</param>
    /// <remarks>
    /// Called by other scripts to get the amount of damage this ability does at a specific level.
    /// Does not have a failsafe for incorrect parameters.
    /// </remarks>
    /// <returns>
    /// The amount of damage this ability does at a level.
    /// </returns>
    public float GetDamage(string key, int level) {
        foreach(AbilityDamage ad in damage) {
            if (ad.key == key) {
                return ad.GetDamage(level);
            }
        }
        return 0;
    }
}
