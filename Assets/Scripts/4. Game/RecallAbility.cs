using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecallAbility : MonoBehaviour {

	PhotonView photonView;
    AbilityHandler abilityHandler;
    PlayerChampion playerChampion;
    PlayerMovement playerMovement;
    float recallTimeRemaining;
    bool isRecalling;
    bool gameEnded;

    void Start() {
        // Stop channeling if player moves or is hit
        PlayerMovement.onPlayerMove += StopChannel;
        PlayerChampion.onPlayerDamaged += StopChannel;

        abilityHandler = AbilityHandler.Instance;
        abilityHandler.recallButton.onClick.AddListener(AttemptRecall);
        photonView = GetComponent<PhotonView>();
        playerMovement = GetComponent<PlayerMovement>();
        playerChampion = GetComponent<PlayerChampion>();
        StopChannel();
        GameHandler.onGameEnd += OnGameEnd;
    }

    void OnGameEnd() {
        gameEnded = true;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.B)) {
            AttemptRecall();
        }
    }

    // Recall if all conditions match
    void AttemptRecall() {
        if(photonView.isMine && !gameEnded) {
            if (!playerChampion.IsDead) {
                if(!isRecalling) {
                    StartChannel();
                }
            }
        }
    }

    // Begin channeling from the start
    void StartChannel() {
        playerMovement.StopMovement();
        isRecalling = true;
        StartCoroutine("Channel");
    }

    // Stop channeling for any reason
    void StopChannel() {
        isRecalling = false;
        StopCoroutine("Channel");
        abilityHandler.recallContainer.SetActive(false);
    }

    // Channel UI & Handler
    IEnumerator Channel() {
        recallTimeRemaining = abilityHandler.recallDuration;
        while (recallTimeRemaining > 0.1f) {
            yield return new WaitForSeconds(0.1f);
            recallTimeRemaining -= 0.1f;
            abilityHandler.recallFill.fillAmount = (recallTimeRemaining / abilityHandler.recallDuration);
            abilityHandler.recallText.text = recallTimeRemaining.ToString("F1");
            abilityHandler.recallContainer.SetActive(true);
        }
        playerChampion.Respawn();
        StopChannel();
    }
}
