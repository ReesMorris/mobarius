using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    This script handles Alucard's E effect
*/
/// <summary>
/// This script handles Alucard's E effect.
/// </summary>
public class AlucardE : MonoBehaviour {

    // Public variables
    public GameObject prefab;

    // Private variables
    AbilityHandler.Abilities abilityKey = AbilityHandler.Abilities.E;
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
        GameUIHandler.Instance.abilityE.GetComponent<Button>().onClick.AddListener(delegate { AttemptAbility(true); });
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
                if (Input.GetKeyDown(KeyCode.E) || buttonPressed) {
                    if (GameUIHandler.Instance.CanCastAbility(abilityKey, ability, playerChampion.Champion)) {
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

        // Send out an event to say that we're about to fire something
        abilityHandler.AbilityActivated(ability);

        // Tell the AbilityHandler that we've used this ability
        abilityHandler.OnAbilityCast(gameObject, abilityKey, ability.cooldown, false);

        // Do ability stuff
        AlucardE_Effect effect = PhotonNetwork.Instantiate(prefab.name, transform.position, Quaternion.identity, 0).GetComponent<AlucardE_Effect>();
        effect.Init(photonView.viewID);

        // Take mana from the player
        playerChampion.PhotonView.RPC("TakeMana", PhotonTargets.All, ability.cost);

        // Heal the player
        for(int i = 0; i < ability.duration; i++) {
            playerChampion.PhotonView.RPC("Heal", PhotonTargets.All, ability.GetDamage(playerChampion.Champion, "damage") / ability.duration);
            yield return new WaitForSeconds(1f);
        }

        // Destroy the particle effect from the network
        PhotonNetwork.Destroy(effect.gameObject);
        sequenceActive = false;
    }
}
