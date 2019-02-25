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
        healthIncrease = c.healthIncrease;
        healthRegen = c.healthRegen;
        healthRegenIncrease = c.healthRegenIncrease;
        maxMana = c.maxMana;
        manaIncrease = c.manaIncrease;
        manaRegen = c.manaRegen;
        manaRegenIncrease = c.manaRegenIncrease;
        range = c.range;
        attackDamage = c.attackDamage;
        attackDamageIncrease = c.attackDamageIncrease;
        attackSpeed = c.attackSpeed;
        attackSpeedIncrease = c.attackSpeedIncrease;
        armour = c.armour;
        armourIncrease = c.armourIncrease;
        magicResist = c.magicResist;
        magicResistIncrease = c.magicResistIncrease;
        movementSpeed = c.movementSpeed;
        movementSpeedIncrease = c.movementSpeedIncrease;
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
    public int GetKiller() {
        foreach(Damage d in damage) {
            if (d.player != null && d.timeInflicted + 10f >= GameUIHandler.Instance.TimeElapsed && d.player.gameObject.GetComponent<PlayerChampion>() != null)
                return d.player.viewID;
        }
        return -1;
    }
    public void OnLevelUp() {
        maxHealth += healthIncrease;
        healthRegen += healthRegenIncrease;
        maxMana += manaIncrease;
        manaRegen += manaRegenIncrease;
        attackDamage += attackDamageIncrease;
        attackSpeed += attackSpeedIncrease;
        armour += armourIncrease;
        magicResist += magicResistIncrease;
        movementSpeed += movementSpeedIncrease;
    }
}