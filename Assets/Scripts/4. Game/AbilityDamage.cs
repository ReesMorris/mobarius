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
