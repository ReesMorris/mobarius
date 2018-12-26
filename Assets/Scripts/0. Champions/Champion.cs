using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName="MOBA / Champion", order = 999)]
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
    public float healthRegen;
    public float maxMana;
    public float manaRegen;
    public float range;
    public float attackDamage;
    public float attackSpeed;
    public float armour;
    public float magicResist;
    public float movementSpeed;
    public bool invincible;

    [HideInInspector] public float health;
    [HideInInspector] public float mana;
    [HideInInspector] public string owner;

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
}