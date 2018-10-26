using UnityEngine;

[CreateAssetMenu(menuName="MOBA / Champion", order = 999)]
public class Champion : ScriptableObject {

    public bool isOwned;
    public bool isFree;
    public bool isAvailable;

    [Header("Stats")]
    public string championName;
    public Sprite icon;
    public float health;
    public float healthRegen;
    public float mana;
    public float manaRegen;
    public float range;
    public float attackDamage;
    public float attackSpeed;
    public float armour;
    public float magicResist;
    public float movementSpeed;

    public bool IsOwned {
        get { return isOwned || isFree; }
    }

    public bool IsAvailable {
        get { return isAvailable;  }
    }
}