using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour {

    Animator animator;
    PhotonView photonView;
    public string CurrentAnimation { get; protected set; }

    void Start() {
        animator = GetComponent<Animator>();
        photonView = GetComponent<PhotonView>();
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
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName(CurrentAnimation) && !CurrentAnimation.Contains("Ability"))
                PlayAnimation(CurrentAnimation);
        }
    }

    [PunRPC]
    void Animate(string name) {
        if (animator != null) {
            animator.Play(name);
        }
    }
}
