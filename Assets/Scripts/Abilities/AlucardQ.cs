using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    This script handles Alucard's Q ability
*/
/// <summary>
/// This script handles Alucard's Q ability.
/// </summary>
public class AlucardQ : MonoBehaviour {

    // Public variables
    public GameObject prefab;

    // Private variables
    AbilityHandler.Abilities abilityKey = AbilityHandler.Abilities.Q;
    PhotonView photonView;
    PlayerChampion playerChampion;
    AbilityHandler abilityHandler;
    PlayerAnimator playerAnimator;
    PlayerMovement playerMovement;
    Ability ability;
    bool sequenceActive;

    // Set up references and event listeners when the game begins.
    void Start() {
        photonView = GetComponent<PhotonView>();
        playerChampion = GetComponent<PlayerChampion>();
        playerAnimator = GetComponent<PlayerAnimator>();
        playerMovement = GetComponent<PlayerMovement>();
        PlayerMovement.onPlayerMove += StopSequence;
        AbilityHandler.onAbilityActivated += OnAbilityActivated;
        abilityHandler = AbilityHandler.Instance;
        ability = abilityHandler.GetChampionAbility(playerChampion.Champion, abilityKey);
        GameUIHandler.Instance.abilityQ.GetComponent<Button>().onClick.AddListener(delegate { AttemptAbility(true); });
    }

    // Every frame, check to see if the user is trying to perform an ability.
    void Update() {
        AttemptAbility(false);
    }

    /// <summary>
    /// Attempts to cast the ability sequence.
    /// </summary>
    /// <param name="buttonPressed">True if the UI icon is clicked to activate the ability</param>
    public void AttemptAbility(bool buttonPressed) {
        if (photonView.isMine) {
            if (!playerChampion.IsDead) {

                // Update the aim position
                if (abilityHandler.IsAiming(ability)) {
                    abilityHandler.UpdateIndicator(gameObject, ability);
                }

                // Toggle the ability off/on
                if (Input.GetKeyDown(KeyCode.Q) || buttonPressed) {
                    if (!sequenceActive) {
                        if (!abilityHandler.IsAiming(ability)) {
                            if (GameUIHandler.Instance.CanCastAbility(abilityKey, ability, playerChampion.Champion)) {
                                abilityHandler.StartCasting(gameObject, ability);
                            }
                        } else {
                            abilityHandler.StopCasting(gameObject);
                        }
                    }
                }

                // Are we firing?
                if (Input.GetMouseButtonDown(0)) {
                    if (abilityHandler.IsAiming(ability)) {
                        if (!sequenceActive)
                            StartCoroutine("AbilitySequence");
                    }
                }
            }
        }
    }

    // If another ability is activated, stop this one
    void OnAbilityActivated(Ability _ability) {
        if(_ability != ability)
            StopSequence();
    }

    // Stop the ability sequence
    public void StopSequence() {
        if(this != null) {
            StopCoroutine("AbilitySequence");
            sequenceActive = false;
        }
    }

    // The sequence of this ability
    IEnumerator AbilitySequence() {
        sequenceActive = true;

        // Save the AOE pos
        Vector3 aoePos = abilityHandler.GetAOEPosition();

        // Send out an event to say that we're about to fire something
        abilityHandler.AbilityActivated(ability);

        // Look in the direction of fire
        transform.LookAt(aoePos);

        // Hide the placeholders
        abilityHandler.StopCasting(gameObject);

        // Play an animation
        playerMovement.StopMovement();
        playerAnimator.PlayAnimation("Ability01");

        yield return new WaitForSeconds(0.6f);
        sequenceActive = false;

        // Tell the AbilityHandler that we've used this ability
        abilityHandler.OnAbilityCast(gameObject, abilityKey, ability.cooldown, false);

        // Do ability stuff
        AlucardQ_Effect e = PhotonNetwork.Instantiate(prefab.name, aoePos, Quaternion.identity, 0).GetComponent<AlucardQ_Effect>();
        e.Init(ability.damageRadius, ability.GetDamage(playerChampion.Champion, "damage"), photonView.viewID);

        // Take mana from the player
        playerChampion.PhotonView.RPC("TakeMana", PhotonTargets.All, ability.cost);
    }
}
