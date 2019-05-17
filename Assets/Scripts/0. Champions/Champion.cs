using UnityEngine;
using System.Collections.Generic;

/*
    The main Champion class
    Contains all variable stats that a Champion will have
*/
/// <summary>
/// The main Champion class.
/// Contains all variable stats that a Champion will have.
/// </summary>
//[CreateAssetMenu(menuName="MOBA / Champion", order = 999)]
public class Champion : ScriptableObject {

    // Ownership variables
    public bool isOwned;
    public bool isFree;
    public bool isAvailable;

    // List of abilities this Champion has (set in the Unity Inspector)
    [Header("Abilities")]
    public Ability[] abilities;

    // Configurable attributes and stats for this Champion
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

    // Actual attributes and stats for this Champion
    [HideInInspector] public float health;
    [HideInInspector] public float mana;
    [HideInInspector] public string owner;
    [HideInInspector] public bool invincible;
    [HideInInspector] public int qLevel;
    [HideInInspector] public int wLevel;
    [HideInInspector] public int eLevel;
    [HideInInspector] public int rLevel;
    [HideInInspector] public int currentLevel;

    // Hidden stats for this Champion
    [HideInInspector] public float physicalDamage;
    [HideInInspector] public float magicDamage;
    [HideInInspector] public float abilityPower;
    [HideInInspector] public List<Damage> damage;


    /// <summary>
    /// Initialises class variables to create a new Champion instance, so that the ScriptableObject is not directly modifying.
    /// </summary>
    /// <param name="c">A Champion class</param>
    /// <param name="o">The nickname of the player who controls this Champion</param>
    public void Init(Champion c, string o) {
        owner = o;
        abilities = c.abilities;
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
        qLevel = 0;
        wLevel = 0;
        eLevel = 0;
        rLevel = 0;
        currentLevel = 0;

        damage = new List<Damage>();
    }

    /// <summary>
    /// Returns whether the Champion is owned.
    /// </summary>
    /// <returns>
    /// True if this Champion is owned by the local player or is on the free Champion rotation; false if not.
    /// </returns>
    public bool IsOwned {
        get { return isOwned || isFree; }
    }

    /// <summary>
    /// Returns whether the Champion is available.
    /// </summary>
    /// <returns>
    /// True if this Champion is marked as available; false if not.
    /// </returns>
    public bool IsAvailable {
        get { return isAvailable;  }
    }

    /// <summary>
    /// Empties the List containing the recent damaged enemies.
    /// </summary>
    public void ResetDamage() {
        damage.Clear();
    }

    /// <summary>
    /// Finds the most recent player who attacked this Champion.
    /// </summary>
    /// <returns>
    /// The Photon viewID of the attacker if found; -1 if no attacker is found.
    /// </returns>
    public int GetKiller() {
        foreach(Damage d in damage) {
            if (d.player != null && d.timeInflicted + 10f >= GameUIHandler.Instance.TimeElapsed && d.player.gameObject.GetComponent<PlayerChampion>() != null)
                return d.player.viewID;
        }
        return -1;
    }

    /// <summary>
    /// Increases Champion stats when called.
    /// </summary>
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

    /// <summary>
    /// Upgrades the level of an ability when called.
    /// </summary>
    /// <param name="abilityKey">The ability key (Q,W,E,R) to be upgraded</param>
    public void UpgradeAbility(AbilityHandler.Abilities abilityKey) {
        switch (abilityKey) {
            case AbilityHandler.Abilities.Q:
                qLevel++; break;
            case AbilityHandler.Abilities.W:
                wLevel++; break;
            case AbilityHandler.Abilities.E:
                eLevel++; break;
            case AbilityHandler.Abilities.R:
                rLevel++; break;
        }
    }
}