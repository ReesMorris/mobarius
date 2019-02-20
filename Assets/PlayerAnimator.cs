using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour {

    Animator animator;
    PhotonView photonView;

    void Start() {
        animator = GetComponent<Animator>();
        photonView = GetComponent<PhotonView>();
    }

    public void PlayAnimation(string name) {
        if(animator != null)
            photonView.RPC("Animate", PhotonTargets.All, name);
    }

    [PunRPC]
    void Animate(string name) {
        animator.Play(name);
    }
}
