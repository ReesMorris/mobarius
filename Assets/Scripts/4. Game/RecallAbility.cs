using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecallAbility : MonoBehaviour {

    AbilityHandler.Abilities abilityKey = AbilityHandler.Abilities.B;
    PhotonView photonView;
    AbilityHandler abilityHandler;
    PlayerChampion playerChampion;
    PlayerMovement playerMovement;
    Ability ability;
    float recallTimeRemaining;
    bool isRecalling;
    bool gameEnded;

    Effect recallEffect;

    void Start() {
        abilityHandler = AbilityHandler.Instance;
        photonView = GetComponent<PhotonView>();
        playerMovement = GetComponent<PlayerMovement>();
        playerChampion = GetComponent<PlayerChampion>();
        ability = abilityHandler.GetChampionAbility(playerChampion.Champion, abilityKey);
        StopChannel();

        // Listeners
        GameHandler.onGameEnd += OnGameEnd;
        PlayerMovement.onPlayerMove += StopChannel;
        PlayerChampion.onPlayerDamaged += StopChannel;
        AbilityHandler.onAbilityActivated += OnAbilityActivated;
        GameUIHandler.Instance.abilityB.GetComponent<Button>().onClick.AddListener(AttemptRecall);
    }

    void OnGameEnd() {
        gameEnded = true;
        PlayerMovement.onPlayerMove -= StopChannel;
        PlayerChampion.onPlayerDamaged -= StopChannel;
        if (this != null)
            StopAllCoroutines();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.B)) {
            if (!ChatHandler.Instance.inputField.gameObject.activeSelf) {
                AttemptRecall();
            }
        }
    }

    // Called when an ability is called
    void OnAbilityActivated(Ability ability) {
        StopChannel();
    }

    // Recall if all conditions match
    void AttemptRecall() {
        if (photonView.isMine) {
            if (!gameEnded) {
                if (!playerChampion.IsDead) {
                    if(!isRecalling) {
                        StartChannel();
                    }
                }
            }
        }
    }

    // Begin channeling from the start
    void StartChannel() {
        if (!gameEnded) {
            if(recallEffect == null)
                recallEffect = EffectsHandler.Instance.recallEffect;
            if (recallEffect != null)
                recallEffect.Show();

            playerMovement.StopMovement();
            isRecalling = true;
            StartCoroutine("Channel");
        }
    }

    // Stop channeling for any reason
    void StopChannel() {
        if (this != null) {
            if(recallEffect != null)
                recallEffect.Hide();
            isRecalling = false;
            StopCoroutine("Channel");
            abilityHandler.recallContainer.SetActive(false);
        }
    }

    // Channel UI & Handler
    IEnumerator Channel() {
        recallTimeRemaining = ability.duration;
        while (recallTimeRemaining > 0.1f) {
            yield return new WaitForSeconds(0.1f);
            recallTimeRemaining -= 0.1f;
            abilityHandler.recallFill.fillAmount = (recallTimeRemaining / ability.duration);
            abilityHandler.recallText.text = recallTimeRemaining.ToString("F1");
            abilityHandler.recallContainer.SetActive(true);
        }
        playerChampion.Respawn();
        StopChannel();
    }
}
