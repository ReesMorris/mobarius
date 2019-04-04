using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlucardE : MonoBehaviour {

    public GameObject prefab;

    AbilityHandler.Abilities abilityKey = AbilityHandler.Abilities.E;
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
        abilityHandler = AbilityHandler.Instance;
        ability = abilityHandler.GetChampionAbility(playerChampion.Champion, abilityKey);
        GameUIHandler.Instance.abilityE.GetComponent<Button>().onClick.AddListener(delegate { AttemptAbility(true); });
    }

    void Update() {
        AttemptAbility(false);
    }

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
            print("healing" + ability.GetDamage(playerChampion.Champion, "damage") / ability.duration);
            playerChampion.PhotonView.RPC("Heal", PhotonTargets.All, ability.GetDamage(playerChampion.Champion, "damage") / ability.duration);
            yield return new WaitForSeconds(1f);
        }

        PhotonNetwork.Destroy(effect.gameObject);
        sequenceActive = false;
    }
}
