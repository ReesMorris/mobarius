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
        if (animator != null) {
            CurrentAnimation = name;
            photonView.RPC("Animate", PhotonTargets.All, name);
        }
    }

    void Update() {
        if(!animator.GetCurrentAnimatorStateInfo(0).IsName(CurrentAnimation))
            PlayAnimation(CurrentAnimation);
    }

    [PunRPC]
    void Animate(string name) {
        if (animator != null) {
            animator.Play(name);
        }
    }
}
