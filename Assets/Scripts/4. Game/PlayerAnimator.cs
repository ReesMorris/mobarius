using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
    This script contains functions relating to player character animations
*/
/// <summary>
/// This script contains functions relating to player character animations.
/// </summary>
public class PlayerAnimator : MonoBehaviour {

    // Private variables
    Animator animator;
    PhotonView photonView;
    public string CurrentAnimation { get; protected set; }
    NavMeshAgent navMeshAgent;
    PlayerChampion playerChampion;

    // Set up the references to other scripts when the game starts
    void Start() {
        animator = GetComponent<Animator>();
        photonView = GetComponent<PhotonView>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerChampion = GetComponent<PlayerChampion>();
    }

    /// <summary>
    /// Plays an animation for the local player across the network.
    /// <param name="name">The name of the animation</param>
    /// </summary>
    public void PlayAnimation(string name) {
        if (photonView.isMine) {
            if (animator != null) {
                CurrentAnimation = name;
                photonView.RPC("Animate", PhotonTargets.All, name);
            }
        }
    }

    // Constantly attempts to play the latest animation for the player character in the event that it is skipped for a frame
    void Update() {
        if (photonView.isMine) {
            if (animator != null && CurrentAnimation != null)
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName(CurrentAnimation) && !CurrentAnimation.Contains("Ability"))
                    PlayAnimation(CurrentAnimation);
            photonView.RPC("SetAnimatorSpeed", PhotonTargets.All);
        }
    }

    // Play an animation across the network
    [PunRPC]
    void Animate(string name) {
        if (animator != null) {
            animator.Play(name);
            
        }
    }

    // Set the animation speed for player animations across the network
    [PunRPC]
    void SetAnimatorSpeed() {
        if (CurrentAnimation == "Walking")
            animator.speed = (navMeshAgent.speed / 3.4f);
        else
            animator.speed = 1;
    }
}
