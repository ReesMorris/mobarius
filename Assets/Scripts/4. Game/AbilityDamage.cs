using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    The class responsible for storing the damage an ability does at each level
*/
/// <summary>
/// The class responsible for storing the damage an ability does at each level.
/// </summary>
[System.Serializable]
public class AbilityDamage {
    public string key;
    public float damageLevel1;
    public float damageLevel2;
    public float damageLevel3;
    public float damageLevel4;
    public float damageLevel5;
    public AbilityHandler.DamageTypes damageType;

    /// <summary>
    /// Finds the damage dealt at a certain level.
    /// </summary>
    /// <param name="level">The level of the ability</param>
    /// <remarks>
    /// The level variable must be within the range of [1,5].
    /// </remarks>
    /// <returns>
    /// The damage dealt at a certain level. If the level is invalid, the damage at level 5 is returned.
    /// </returns>
    public float GetDamage(int level) {
        switch (level) {
            case 1:
                return damageLevel1;
            case 2:
                return damageLevel2;
            case 3:
                return damageLevel3;
            case 4:
                return damageLevel4;
            default:
                return damageLevel5;
        }
    }
}
