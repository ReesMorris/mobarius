using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityHandler : MonoBehaviour {

    public GameObject projectileIndicatorPrefab;
    public enum Abilities { Passive, Q, W, E, R, D, F };
    public enum DamageTypes { PhysicalDamage, MagicDamage };

    [Header("Recall")]
    public Button recallButton;
    public float recallDuration;
    public GameObject recallContainer;
    public Image recallFill;
    public TMP_Text recallText;

    public static AbilityHandler Instance;
    public bool Aiming { get; protected set; }

    bool gameEnded;

    void Start () {
        Instance = this;
        GameHandler.onGameEnd += OnGameEnd;
    }

    // Returns the direction an indicator will need to be in relation to a player
    public Vector3 GetDirection(GameObject player) {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Floor")))
            return new Vector3(hit.point.x, player.transform.position.y, hit.point.z);
        return Vector3.zero;
    }

    public void UpdateIndicatorRotation(GameObject indicator, GameObject player) {
        indicator.transform.LookAt(GetDirection(player));
    }

    // Returns the ability associated with a champion
    public Ability GetChampionAbilities(string championName, Abilities _ability) {
        Champion champion = ChampionRoster.Instance.GetChampion(championName);
        foreach(Ability ability in champion.abilities) {
            if (ability.abilityKey == _ability)
                return ability;
        }
        return null;
    }

    // Sets up a projectile indicator for the caller
    public GameObject SetupProjectileIndicator(GameObject player) {
        if (!gameEnded) {
            GameObject indicator = Instantiate(projectileIndicatorPrefab, player.transform.position, Quaternion.identity);
            indicator.transform.parent = player.gameObject.transform;
            indicator.transform.localPosition = Vector3.zero;
            indicator.SetActive(false);
            return indicator;
        }
        return null;
    }

    // Called when an ability begins to be cast (displays indicator)
    public void StartCasting(GameObject indicator, float range) {
        if (!gameEnded) {
            Aiming = true;
            indicator.transform.localScale = new Vector3(indicator.transform.localScale.x, indicator.transform.localScale.y, range * 20f);
            indicator.SetActive(true);
        }
    }

    // Called when an ability stops being cast
    public void StopCasting(GameObject indicator) {
        Aiming = false;
        indicator.SetActive(false);
    }

    // Calls when an ability has been fired (after showing indicator)
    public void OnAbilityCast(GameObject player, GameObject indicator, Abilities ability, float cooldown, bool lookAtMouse) {
        if (!gameEnded) {
            StopCasting(indicator);
            player.transform.LookAt(GetDirection(player));
            GameUIHandler.Instance.OnAbilityCasted(ability, cooldown);
        }
    }

    // Returns the amount of damage a specific key in an ability will do
    public float GetDamageFromAbility(Ability ability, string key) {
        foreach(AbilityDamage abilityDamage in ability.damage) {
            if (abilityDamage.key == key)
                return abilityDamage.damage;
        }
        return 0f;
    }

    void OnGameEnd() {
        gameEnded = true;
    }
}
