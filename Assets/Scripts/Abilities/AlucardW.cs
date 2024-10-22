﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    This script handles Alucard's W ability
*/
/// <summary>
/// This script handles Alucard's W ability.
/// </summary>
public class AlucardW : MonoBehaviour {

    // Public variables
    public GameObject prefab;

    // Private variables
    AbilityHandler.Abilities abilityKey = AbilityHandler.Abilities.W;
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
        abilityHandler = AbilityHandler.Instance;
        ability = abilityHandler.GetChampionAbility(playerChampion.Champion, abilityKey);
        GameUIHandler.Instance.abilityW.GetComponent<Button>().onClick.AddListener(delegate { AttemptAbility(true); });
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
                if (Input.GetKeyDown(KeyCode.W) || buttonPressed) {
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
                        if (!sequenceActive) {
                            StartCoroutine("AbilitySequence");
                        }
                    }
                }
            }
        }
    }

    // The sequence of this ability
    IEnumerator AbilitySequence() {
        sequenceActive = true;

        Vector3 direction = abilityHandler.GetMousePosition(gameObject);

        // Send out an event to say that we're about to fire something
        abilityHandler.AbilityActivated(ability);

        // Hide the placeholders
        abilityHandler.StopCasting(gameObject);

        // Look in the direction of fire
        transform.LookAt(direction);

        // Play an animation
        playerMovement.StopMovement();
        playerAnimator.PlayAnimation("Ability02");

        yield return new WaitForSeconds(0.4f);
        sequenceActive = false;

        // Tell the AbilityHandler that we've used this ability
        abilityHandler.OnAbilityCast(gameObject, abilityKey, ability.cooldown, false);

        // Do ability stuff
        AlucardW_Effect e = PhotonNetwork.Instantiate(prefab.name, transform.position, Quaternion.identity, 0).GetComponent<AlucardW_Effect>();
        e.Init(ability.damageRadius, ability.GetDamage(playerChampion.Champion, "damage"), direction, photonView.viewID);

        // Take mana from the player
        playerChampion.PhotonView.RPC("TakeMana", PhotonTargets.All, ability.cost);
    }
}
