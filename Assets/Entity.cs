using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {

    public PunTeams.Team team;
    public bool IsDead;
    PhotonView photonView;

    float health;
    float maxHealth;

    void Start() {
        photonView = GetComponent<PhotonView>();
    }

    public void Init(float _maxHealth) {
        health = maxHealth = _maxHealth;
    }

    // Called when a champion takes damage by a source
    [PunRPC]
    void Damage(float amount, PhotonPlayer attacker) {
        print("entity is taking damage");
        health = Mathf.Max(health - amount, 0f);
        if(health <= 0f) {
            IsDead = true;
            gameObject.SetActive(false);
        }
    }

}
