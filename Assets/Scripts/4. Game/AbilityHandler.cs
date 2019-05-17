using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
    The script responsible for handling ability UI and indicator prefabs
*/
/// <summary>
/// The script responsible for handling ability UI and indicator prefabs.
/// </summary>
public class AbilityHandler : MonoBehaviour {

    // Public variables
    public static AbilityHandler Instance;

    public delegate void OnAbilityActivated(Ability ability);
    public static OnAbilityActivated onAbilityActivated;

    public GameObject projectileIndicatorPrefab;
    public GameObject aoeRangePrefab;
    public GameObject aoeIndicatorPrefab;
    public GameObject scopeIndicatorPrefab;

    public enum Abilities { Passive, Q, W, E, R, F, G, B };
    public enum AbilityTypes { Spell, Directional, AOE, Scope };
    public enum DamageTypes { PhysicalDamage, MagicDamage, Health };

    [Header("Tooltip")]
    public GameObject tooltip;
    public TMP_Text tooltipText;
    public TMP_Text tooltipInfo;

    [Header("Recall")]
    public Button recallButton;
    public GameObject recallContainer;
    public Image recallFill;
    public TMP_Text recallText;

    // Private variables
    Ability aimingAbility;
    MapProperties mapProperties;

    // Indicators
    GameObject directionalIndicator;
    GameObject aoeRangeIndicator;
    GameObject aoeIndicatorIndicator;
    GameObject scopeIndicator;

    bool gameEnded;

    // Allow other scripts to reference this on the game start.
    void Awake() {
        Instance = this;
    }

    // Add event listeners on the game start
    void Start () {
        GameHandler.onGameStart += OnGameStart;
        GameHandler.onGameEnd += OnGameEnd;
    }

    /// <summary>
    /// Checks to see if an ability is being aimed.
    /// </summary>
    /// <param name="ability">The ability to be checked</param>
    /// <remarks>
    /// Only one ability can be aimed at any time.
    /// </remarks>
    /// <returns>
    /// True if the parameter ability is being aimed; false if not.
    /// </returns>
    public bool IsAiming(Ability ability) {
        return aimingAbility == ability;
    }

    /// <summary>
    /// Fires a Raycast from the mouse position to the Floor layer.
    /// </summary>
    /// <param name="player">The player GameObject the call is in reference to</param>
    /// <remarks>
    /// The Y position returned will be the same as the Y position of player parameter.
    /// </remarks>
    /// <returns>
    /// The 3D coordinates of the mouse position if the raycast hits; a zero vector if not.
    /// </returns>
    public Vector3 GetMousePosition(GameObject player) {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Floor")))
            return new Vector3(hit.point.x, player.transform.position.y, hit.point.z);
        return Vector3.zero;
    }

    /// <summary>
    /// Searches a Champion's abilities to find the one corresponding with the keyboard shortcut.
    /// </summary>
    /// <param name="champion">The Champion class</param>
    /// <param name="abilityKey">The default key the ability uses</param>
    /// <returns>
    /// The ability associated with a champion if it exists; null if not.
    /// </returns>
    public Ability GetChampionAbility(Champion champion, Abilities abilityKey) {
        foreach(Ability ability in champion.abilities) {
            if (ability.abilityKey == abilityKey)
                return ability;
        }
        return null;
    }

    /// <summary>
    /// Sets up the ability indicators for the local player
    /// </summary>
    /// <param name="player">The GameObject to instantiate the indicators on</param>
    public void SetupAbilityIndicators(GameObject player) {
        if (PhotonNetwork.player.IsLocal) {
            SetupDirectionalIndicator(player);
            SetupAOEIndicator(player);
            SetupScopeIndicator(player);
        }
    }

    /// <summary>
    /// Updates the orientation of the ability indicators for the local player
    /// </summary>
    /// <param name="player">The GameObject with the indicators as children</param>
    /// <param name="ability">The Ability to update the indicator for</param>
    public void UpdateIndicator(GameObject player, Ability ability) {

        // Directional
        if (ability.abilityType == AbilityTypes.Directional) {
            if (mapProperties.display == PlayerCamera.CameraDisplays.TopDown)
                directionalIndicator.transform.LookAt(GetMousePosition(player));
            else
                directionalIndicator.transform.localEulerAngles = Vector3.zero;
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

        // Scope
        if(ability.abilityType == AbilityTypes.Scope) {
            if (mapProperties.display == PlayerCamera.CameraDisplays.TopDown)
                scopeIndicator.transform.LookAt(GetMousePosition(player));
            else
                scopeIndicator.transform.localEulerAngles = Vector3.zero;
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
            // The larger circle indicating the maximum range
            aoeRangeIndicator = Instantiate(aoeRangePrefab, player.transform.position, Quaternion.identity);
            aoeRangeIndicator.name = "AOERange";
            aoeRangeIndicator.transform.parent = player.transform;
            aoeRangeIndicator.transform.localPosition = Vector3.zero;
            aoeRangeIndicator.SetActive(false);

            // The smaller circle following the user's mouse position
            aoeIndicatorIndicator = Instantiate(aoeIndicatorPrefab, player.transform.position, Quaternion.identity);
            aoeIndicatorIndicator.name = "AOEIndicator";
            aoeIndicatorIndicator.transform.parent = player.transform;
            aoeIndicatorIndicator.transform.localPosition = Vector3.zero;
            aoeIndicatorIndicator.SetActive(false);
        }
    }

    // Set up the Scope indicator for the caller (typically called on spawn)
    void SetupScopeIndicator(GameObject player) {
        scopeIndicator = Instantiate(scopeIndicatorPrefab, player.transform.position, Quaternion.identity);
        scopeIndicator.name = "ScopeIndicator";
        scopeIndicator.transform.parent = player.transform;
        scopeIndicator.transform.localPosition = Vector3.zero;
        scopeIndicator.SetActive(false);
    }

    /// <summary>
    /// Called when an ability begins to be cast, displaying the corresponding indicator.
    /// </summary>
    /// <param name="player">The GameObject casting the ability</param>
    /// <param name="ability">The Ability being cast</param>
    public void StartCasting(GameObject player, Ability ability) {
        if (!gameEnded) {

            // If now casting a new ability, stop the previous
            if (aimingAbility != ability)
                StopCasting(player);

            // Update which ability is being aimed
            aimingAbility = ability;
            
            // AOE Indicator
            if(ability.abilityType == AbilityTypes.AOE) {
                if (aoeRangeIndicator != null && aoeIndicatorIndicator != null) {
                    aoeRangeIndicator.transform.localScale = new Vector3(ability.range, aoeRangeIndicator.transform.localScale.y, ability.range);
                    aoeIndicatorIndicator.transform.localScale = new Vector3(ability.damageRadius, aoeIndicatorIndicator.transform.localScale.y, ability.damageRadius);
                    aoeRangeIndicator.SetActive(true);
                    aoeIndicatorIndicator.SetActive(true);
                }
            }

            // Scope Indicator
            if(ability.abilityType == AbilityTypes.Scope) {
                if(scopeIndicator != null) {
                    scopeIndicator.SetActive(true);
                }
            }
        }
    }

    /// <summary>
    /// Called when an ability stops being cast, hides all attack indicators.
    /// </summary>
    /// <param name="player">The GameObject casting the ability</param>
    public void StopCasting(GameObject player) {
        aimingAbility = null;
        directionalIndicator.SetActive(false);
        aoeRangeIndicator.SetActive(false);
        aoeIndicatorIndicator.SetActive(false);
        scopeIndicator.SetActive(false);
    }

    /// <summary>
    /// Called when an ability has been activated, but not fired.
    /// </summary>
    /// <param name="ability">The Ability which was activated</param>
    public void AbilityActivated(Ability ability) {
        if (onAbilityActivated != null)
            onAbilityActivated(ability);
    }

    /// <summary>
    /// Called when an ability has been fired (after showing indicator).
    /// </summary>
    /// <param name="player">The GameObject casting the ability</param>
    /// <param name="ability">The Ability which was fired</param>
    /// <param name="cooldown">The cooldown time for the ability</param>
    /// <param name="lookAtMouse">True if the player should look at the 3D mouse position</param>
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
        mapProperties = MapManager.Instance.GetMapProperties();
    }
    void OnGameEnd() {
        gameEnded = true;
    }

    // Positions
    /// <summary>
    /// Used to get the world position of the AOE attack indicator.
    /// </summary>
    /// <returns>
    /// The world position of the AOE attack indicator.
    /// </returns>
    public Vector3 GetAOEPosition() {
        return aoeIndicatorIndicator.transform.position;
    }

    /// <summary>
    /// Finds the level of a Champion's specified ability.
    /// </summary>
    /// <param name="champion">The Champion with the ability</param>
    /// <param name="ability">The Ability to find the level of</param>
    /// <returns>
    /// The level of a Champion's specific ability if it can be found; fallback return value is 1.
    /// </returns>
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
