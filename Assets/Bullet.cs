using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    float damage;
    float range;
    Vector3 startingPos;

    void OnPhotonInstantiate(PhotonMessageInfo info) {
    }

    public void Setup(float _damage, Vector3 _startingPos, float _range) {
        damage = _damage;
        startingPos = _startingPos;
        range = _range;
    }

    void Update() {
        if(Vector3.Distance(transform.position, startingPos) > range) {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision) {
        print(collision.gameObject.name);
        PlayerChampion playerChampion = collision.gameObject.GetComponent<PlayerChampion>();
        print(playerChampion);
        if (playerChampion != null) {
            PhotonView photonView = playerChampion.GetComponent<PhotonView>();
            if (PhotonNetwork.isMasterClient) {
                print("damage!");
                photonView.RPC("Damage", PhotonTargets.All, damage);
            }
        }
        Destroy(gameObject);
    }
}
