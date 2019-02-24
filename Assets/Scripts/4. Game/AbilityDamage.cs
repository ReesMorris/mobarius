using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbilityDamage {
    public string key;
    public float damageLevel1;
    public float damageLevel2;
    public float damageLevel3;
    public float damageLevel4;
    public float damageLevel5;
    public AbilityHandler.DamageTypes damageType;

    int level = 1;
    public int Level { get { return level; } }
    public readonly int maxLevel = 5;

    public void LevelUp() {
        level = Mathf.Min(level + 1, 4);
    }

    public float GetDamage() {
        switch(level) {
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

    public float GetDamageAtLevel(int level) {
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
