using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAnimator : MonoBehaviour {

    Animator animator;
    PhotonView photonView;
    public string CurrentAnimation { get; protected set; }
    NavMeshAgent navMeshAgent;
    PlayerChampion playerChampion;

    void Start() {
        animator = GetComponent<Animator>();
        photonView = GetComponent<PhotonView>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerChampion = GetComponent<PlayerChampion>();
    }

    public void PlayAnimation(string name) {
        if (photonView.isMine) {
            if (animator != null) {
                CurrentAnimation = name;
                photonView.RPC("Animate", PhotonTargets.All, name);
            }
        }
    }

    void Update() {
        if (photonView.isMine) {
            if (animator != null && CurrentAnimation != null)
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName(CurrentAnimation) && !CurrentAnimation.Contains("Ability"))
                    PlayAnimation(CurrentAnimation);
            photonView.RPC("SetAnimatorSpeed", PhotonTargets.All);
        }
    }

    [PunRPC]
    void Animate(string name) {
        if (animator != null) {
            animator.Play(name);
            
        }
    }

    [PunRPC]
    void SetAnimatorSpeed() {
        if (CurrentAnimation == "Walking")
            animator.speed = (navMeshAgent.speed / 3.4f);
        else
            animator.speed = 1;
    }
}
