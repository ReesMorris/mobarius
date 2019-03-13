using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityHandler : MonoBehaviour {

    public delegate void OnAbilityActivated(Ability ability);
    public static OnAbilityActivated onAbilityActivated;

    public GameObject projectileIndicatorPrefab;
    public GameObject aoeRangePrefab;
    public GameObject aoeIndicatorPrefab;

    public enum Abilities { Passive, Q, W, E, R, D, F };
    public enum AbilityTypes { Directional, AOE };
    public enum DamageTypes { PhysicalDamage, MagicDamage };

    [Header("Tooltip")]
    public GameObject tooltip;
    public TMP_Text tooltipText;
    public TMP_Text tooltipInfo;

    [Header("Recall")]
    public Button recallButton;
    public float recallDuration;
    public GameObject recallContainer;
    public Image recallFill;
    public TMP_Text recallText;

    public static AbilityHandler Instance;
    public bool Aiming { get; protected set; }

    // Indicators
    GameObject directionalIndicator;
    GameObject aoeRangeIndicator;
    GameObject aoeIndicatorIndicator;

    bool gameEnded;

    void Awake() {
        Instance = this;
    }

    void Start () {
        GameHandler.onGameStart += OnGameStart;
        GameHandler.onGameEnd += OnGameEnd;
    }

    // Returns the direction an indicator will need to be in relation to a player
    public Vector3 GetMousePosition(GameObject player) {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Floor")))
            return new Vector3(hit.point.x, player.transform.position.y, hit.point.z);
        return Vector3.zero;
    }

    // Returns the ability associated with a champion
    public Ability GetChampionAbility(Champion champion, Abilities abilityKey) {
        foreach(Ability ability in champion.abilities) {
            if (ability.abilityKey == abilityKey)
                return ability;
        }
        return null;
    }

    public void SetupAbilityIndicators(GameObject player) {
        if (PhotonNetwork.player.IsLocal) {
            SetupDirectionalIndicator(player);
            SetupAOEIndicator(player);
        }
    }

    public void UpdateIndicator(GameObject player, Ability ability) {

        // Directional
        if (ability.abilityType == AbilityTypes.Directional) {
            directionalIndicator.transform.LookAt(GetMousePosition(player));
        }

        // AOE
        if(ability.abilityType == AbilityTypes.AOE) {
            Vector3 mousePos = GetMousePosition(player);
            float distance = Vector3.Distance(mousePos, player.transform.position);
            float maxDist = (ability.range / 2f) + ability.damageRadius / 2f;

            // Source: https://answers.unity.com/questions/1309521/how-to-keep-an-object-within-a-circlesphere-radius.html [Accessed 22 February 2019]
            if (distance > maxDist) {
                Vector3 fromOriginToObject = mousePos - player.transform.position;
                fromOriginToObject *= maxDist / distance;
                mousePos = player.transform.position + fromOriginToObject;
            }
            aoeIndicatorIndicator.transform.position = mousePos;
            aoeRangeIndicator.transform.eulerAngles = Vector3.zero;
        }
    }

    // Sets up a projectile indicator for the caller (typically called on spawn)
    void SetupDirectionalIndicator(GameObject player) {
        if (!gameEnded) {
            directionalIndicator = Instantiate(projectileIndicatorPrefab, player.transform.position, Quaternion.identity);
            directionalIndicator.name = "DirectionalIndicator";
            directionalIndicator.transform.parent = player.transform;
            directionalIndicator.transform.localPosition = Vector3.zero;
            directionalIndicator.SetActive(false);
        }
    }

    // Set up an AOE indicator for the caller (typically called on spawn)
    void SetupAOEIndicator(GameObject player) {
        if (!gameEnded) {
            aoeRangeIndicator = Instantiate(aoeRangePrefab, player.transform.position, Quaternion.identity);
            aoeRangeIndicator.name = "AOERange";
            aoeRangeIndicator.transform.parent = player.transform;
            aoeRangeIndicator.transform.localPosition = Vector3.zero;
            aoeRangeIndicator.SetActive(false);

            aoeIndicatorIndicator = Instantiate(aoeIndicatorPrefab, player.transform.position, Quaternion.identity);
            aoeIndicatorIndicator.name = "AOEIndicator";
            aoeIndicatorIndicator.transform.parent = player.transform;
            aoeIndicatorIndicator.transform.localPosition = Vector3.zero;
            aoeIndicatorIndicator.SetActive(false);
        }
    }

    // Called when an ability begins to be cast (displays indicator)
    public void StartCasting(GameObject player, Ability ability) {
        if (!gameEnded) {
            Aiming = true;
            
            if(ability.abilityType == AbilityTypes.AOE) {
                if (aoeRangeIndicator != null && aoeIndicatorIndicator != null) {
                    aoeRangeIndicator.transform.localScale = new Vector3(ability.range, aoeRangeIndicator.transform.localScale.y, ability.range);
                    aoeIndicatorIndicator.transform.localScale = new Vector3(ability.damageRadius, aoeIndicatorIndicator.transform.localScale.y, ability.damageRadius);
                    aoeRangeIndicator.SetActive(true);
                    aoeIndicatorIndicator.SetActive(true);
                }
            }
        }
    }

    // Called when an ability stops being cast
    public void StopCasting(GameObject player) {
        Aiming = false;
        directionalIndicator.SetActive(false);
        aoeRangeIndicator.SetActive(false);
        aoeIndicatorIndicator.SetActive(false);
    }

    // Calls when an ability has been activated, but not fired
    public void AbilityActivated(Ability ability) {
        if (onAbilityActivated != null)
            onAbilityActivated(ability);
    }

    // Calls when an ability has been fired (after showing indicator)
    public void OnAbilityCast(GameObject player, Abilities ability, float cooldown, bool lookAtMouse) {
        if (!gameEnded) {
            StopCasting(player);
            GameUIHandler.Instance.OnAbilityCasted(ability, cooldown);
            if (lookAtMouse)
                player.transform.LookAt(GetMousePosition(player));
        }
    }

    // Game Start and End functions
    void OnGameStart() {
        gameEnded = false;
    }
    void OnGameEnd() {
        gameEnded = true;
    }

    // Positions
    public Vector3 GetAOEPosition() {
        return aoeIndicatorIndicator.transform.position;
    }

    public int GetAbilityLevel(Champion champion, Ability ability) {
        switch (ability.abilityKey) {
            case Abilities.Q:
                return champion.qLevel;
            case Abilities.W:
                return champion.wLevel;
            case Abilities.E:
                return champion.eLevel;
            case Abilities.R:
                return champion.rLevel;
        }
        return 1;
    }
}
