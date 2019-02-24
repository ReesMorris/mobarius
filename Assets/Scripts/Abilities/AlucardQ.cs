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
    Ability ability;

    void Start() {
        photonView = GetComponent<PhotonView>();
        playerChampion = GetComponent<PlayerChampion>();
        abilityHandler = AbilityHandler.Instance;
        ability = abilityHandler.GetChampionAbilities(playerChampion.Champion.championName, abilityKey);
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
                        
                        // Tell the AbilityHandler that we've used this ability
                        abilityHandler.OnAbilityCast(gameObject, abilityKey, ability.cooldown, false);

                        // Do ability stuff
                        AlucardQ_Effect e = PhotonNetwork.Instantiate(prefab.name, abilityHandler.GetAOEPosition(), Quaternion.identity, 0).GetComponent<AlucardQ_Effect>();
                        e.Init(ability.damageRadius, ability, photonView.viewID);

                        // Take mana from the player
                        playerChampion.PhotonView.RPC("TakeMana", PhotonTargets.All, ability.cost);
                    }
                }
            }
        }
    }
}
