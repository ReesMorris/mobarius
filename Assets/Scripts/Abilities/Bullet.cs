using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    float damage;
    float range;
    Vector3 startingPos;
    PhotonPlayer shooter;
    GameObject target;

    public void Setup(float _damage, Vector3 _startingPos, float _range, PhotonPlayer _shooter) {
        shooter = _shooter;
        damage = _damage;
        startingPos = _startingPos;
        range = _range;
    }
    public void Setup(float _damage, Vector3 _startingPos, int photonId, PhotonPlayer _shooter) {
        shooter = _shooter;
        damage = _damage;
        startingPos = _startingPos;
        target = PhotonView.Find(photonId).gameObject;
    }

    void Update() {
        if(target != null) {
            if (target.layer == LayerMask.NameToLayer("Targetable")) {
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, 10f * Time.deltaTime);
            } else {
                target = null;
                Destroy(gameObject);
            }
        } else {
            if(Vector3.Distance(transform.position, startingPos) > range) {
                Destroy(gameObject);
            }
        }
    }

    void OnCollisionEnter(Collision collision) {
        PlayerChampion playerChampion = collision.gameObject.GetComponent<PlayerChampion>();
        if (playerChampion != null) {
            PhotonView photonView = playerChampion.GetComponent<PhotonView>();
            if (PhotonNetwork.isMasterClient) {
                photonView.RPC("Damage", PhotonTargets.All, damage);
            }
        }

        Turret turret = collision.gameObject.GetComponent<Turret>();
        if(turret != null) {
            PhotonView photonView = turret.GetComponent<PhotonView>();
            if (PhotonNetwork.isMasterClient) {
                photonView.RPC("Damage", PhotonTargets.All, damage);
            }
        }


        Destroy(gameObject);
    }
}
