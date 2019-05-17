using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

/*
    This script handles Alucard's Flash ability
*/
/// <summary>
/// This script handles Alucard's Flash ability.
/// </summary>
public class FlashAbility : MonoBehaviour {

    // Private variables
    AbilityHandler.Abilities abilityType = AbilityHandler.Abilities.F;
    PhotonView photonView;
    AbilityHandler abilityHandler;
    PlayerChampion playerChampion;
    PlayerMovement playerMovement;
    NavMeshAgent navMeshAgent;
    Ability ability;
    bool gameEnded;

    // Assign listeners and references to other scripts when the game starts.
    void Start() {
        GameUIHandler.Instance.abilityF.GetComponent<Button>().onClick.AddListener(AttemptAbility);
        abilityHandler = AbilityHandler.Instance;
        photonView = GetComponent<PhotonView>();
        playerChampion = GetComponent<PlayerChampion>();
        playerMovement = GetComponent<PlayerMovement>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        ability = abilityHandler.GetChampionAbility(playerChampion.Champion, abilityType);
    }

    // Attempt to cast the ability every frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.F)) {
            AttemptAbility();
        }
    }

    // The steps for the ability to work
    void AttemptAbility() {
        // Are we the player doing this?
        if (photonView.isMine) {
            // Are we alive?
            if (!playerChampion.IsDead) {
                // Can we cast this ability?
                if (GameUIHandler.Instance.CanCastAbility(abilityType, ability, playerChampion.Champion)) {
                    transform.LookAt(abilityHandler.GetMousePosition(gameObject));
                    navMeshAgent.Move(transform.forward * ability.range);
                    abilityHandler.OnAbilityCast(gameObject, abilityType, ability.cooldown, true);

                    // Stop the player from moving because otherwise they'll likely walk backwards
                    playerMovement.StopMovement();
                }
            }
        }
    }
}
