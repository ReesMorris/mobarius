using UnityEngine;
using System.Collections.Generic;

//[CreateAssetMenu(menuName="MOBA / Champion", order = 999)]
public class Champion : ScriptableObject {

    public bool isOwned;
    public bool isFree;
    public bool isAvailable;

    [Header("Abilities")]
    public Ability[] abilities;

    [Header("Stats")]
    public string championName;
    public Sprite icon;
    public float maxHealth;
    public float healthIncrease;
    public float healthRegen;
    public float healthRegenIncrease;
    public float maxMana;
    public float manaIncrease;
    public float manaRegen;
    public float manaRegenIncrease;
    public float range;
    public float attackDamage;
    public float attackDamageIncrease;
    public float attackSpeed;
    public float attackSpeedIncrease;
    public float armour;
    public float armourIncrease;
    public float magicResist;
    public float magicResistIncrease;
    public float movementSpeed;
    public float movementSpeedIncrease;

    [HideInInspector] public float health;
    [HideInInspector] public float mana;
    [HideInInspector] public string owner;
    [HideInInspector] public bool invincible;

    // Hidden stats
    [HideInInspector] public float physicalDamage;
    [HideInInspector] public float magicDamage;
    [HideInInspector] public float abilityPower;
    [HideInInspector] public List<Damage> damage;

    public void Init(Champion c, string o) {
        owner = o;
        isOwned = c.isOwned;
        isFree = c.isFree;
        isAvailable = c.isAvailable;
        championName = c.championName;
        icon = c.icon;
        maxHealth = c.maxHealth;
        healthRegen = c.healthRegen;
        maxMana = c.maxMana;
        manaRegen = c.manaRegen;
        range = c.range;
        attackDamage = c.attackDamage;
        attackSpeed = c.attackSpeed;
        armour = c.armour;
        magicResist = c.magicResist;
        movementSpeed = c.movementSpeed;
        invincible = c.invincible;

        damage = new List<Damage>();
    }

    public bool IsOwned {
        get { return isOwned || isFree; }
    }

    public bool IsAvailable {
        get { return isAvailable;  }
    }

    public void ResetDamage() {
        damage.Clear();
    }
    public PhotonPlayer GetKiller() {
        foreach(Damage d in damage) {
            if (d.player != null && d.timeInflicted + 10f >= GameUIHandler.Instance.TimeElapsed)
                return d.player;
        }
        return null;
    }
}