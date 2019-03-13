using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlucardQ : MonoBehaviour {

    public GameObject prefab;

    AbilityHandler.Abilities abilityKey = AbilityHandler.Abilities.Q;
    PhotonView photonView;
    PlayerChampion playerChampion;
    AbilityHandler abilityHandler;
    PlayerAnimator playerAnimator;
    PlayerMovement playerMovement;
    Ability ability;
    bool sequenceActive;

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

    void Update() {
        AttemptAbility(false);
    }

    public void AttemptAbility(bool buttonPressed) {
        if (photonView.isMine) {
            if (!playerChampion.IsDead) {

                // Update the aim position
                if (abilityHandler.Aiming) {
                    abilityHandler.UpdateIndicator(gameObject, ability);
                }

                // Toggle the ability off/on
                if (Input.GetKeyDown(KeyCode.Q) || buttonPressed) {
                    if (!abilityHandler.Aiming) {
                        if (GameUIHandler.Instance.CanCastAbility(abilityKey, ability, playerChampion.Champion)) {
                            abilityHandler.StartCasting(gameObject, ability);
                        }
                    } else {
                        abilityHandler.StopCasting(gameObject);
                    }
                }

                // Are we firing?
                if (Input.GetMouseButtonDown(0)) {
                    if (abilityHandler.Aiming) {
                        if (!sequenceActive) {
                            StartCoroutine("AbilitySequence");
                        }
                    }
                }
            }
        }
    }

    void OnAbilityActivated(Ability _ability) {
        if(_ability != ability)
            StopSequence();
    }

    public void StopSequence() {
        if(sequenceActive)
            StopCoroutine("AbilitySequence");
    }

    IEnumerator AbilitySequence() {
        sequenceActive = true;

        // Send out an event to say that we're about to fire something
        abilityHandler.AbilityActivated(ability);

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
        AlucardQ_Effect e = PhotonNetwork.Instantiate(prefab.name, abilityHandler.GetAOEPosition(), Quaternion.identity, 0).GetComponent<AlucardQ_Effect>();
        e.Init(ability.damageRadius, ability.GetDamage(playerChampion.Champion, "damage"), photonView.viewID);

        // Take mana from the player
        playerChampion.PhotonView.RPC("TakeMana", PhotonTargets.All, ability.cost);
    }
}
