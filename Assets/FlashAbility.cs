using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class FlashAbility : MonoBehaviour {

    AbilityHandler.Abilities abilityType = AbilityHandler.Abilities.D;
    PhotonView photonView;
    AbilityHandler abilityHandler;
    PlayerChampion playerChampion;
    PlayerMovement playerMovement;
    NavMeshAgent navMeshAgent;
    Ability ability;
    bool gameEnded;

    void Start() {
        GameUIHandler.Instance.abilityD.GetComponent<Button>().onClick.AddListener(AttemptAbility);
        abilityHandler = AbilityHandler.Instance;
        photonView = GetComponent<PhotonView>();
        playerChampion = GetComponent<PlayerChampion>();
        playerMovement = GetComponent<PlayerMovement>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        ability = abilityHandler.GetChampionAbilities(playerChampion.Champion.championName, abilityType);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.D)) {
            AttemptAbility();
        }
    }

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
