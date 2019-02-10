using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    float damage;
    float range;
    Vector3 startingPos;
    PhotonPlayer shooter;
    GameObject target;
    PunTeams.Team team;
    CapsuleCollider collider;

    public void Setup(float _damage, PunTeams.Team _team, Vector3 _startingPos, float _range, PhotonPlayer _shooter) {
        ValidateSetup();
        shooter = _shooter;
        team = _team;
        damage = _damage;
        startingPos = _startingPos;
        range = _range;
    }
    public void Setup(float _damage, PunTeams.Team _team, Vector3 _startingPos, int photonId, PhotonPlayer _shooter) {
        ValidateSetup();
        shooter = _shooter;
        team = _team;
        damage = _damage;
        startingPos = _startingPos;
        target = PhotonView.Find(photonId).gameObject;
        collider.enabled = false;
    }

    void ValidateSetup() {
        collider = GetComponent<CapsuleCollider>();
    }

    void Update() {
        if(target != null) {
            if (target.layer == LayerMask.NameToLayer("Targetable")) {
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, 10f * Time.deltaTime);
                if(Vector3.Distance(transform.position, target.transform.position) < .5f) {
                    OnCollide(target);
                }
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
        OnCollide(collision.gameObject);
    }

    void OnCollide(GameObject collision) {
        PlayerChampion playerChampion = collision.GetComponent<PlayerChampion>();
        Entity entity = collision.GetComponent<Entity>();
        Turret turret = collision.GetComponent<Turret>();
        Inhibitor inhibitor = collision.GetComponent<Inhibitor>();
        Nexus nexus = collision.GetComponent<Nexus>();

        if (playerChampion != null) {
            PhotonView photonView = playerChampion.GetComponent<PhotonView>();
            if (PhotonNetwork.isMasterClient && photonView.owner.GetTeam() != team) {
                photonView.RPC("Damage", PhotonTargets.All, damage, shooter);
            }
        } else if (turret != null) {
            PhotonView photonView = turret.GetComponent<PhotonView>();
            Targetable targetable = turret.GetComponent<Targetable>();
            if (PhotonNetwork.isMasterClient && targetable.allowTargetingBy == team) {
                photonView.RPC("Damage", PhotonTargets.All, damage, shooter);
            }
        } else if (inhibitor != null) {
            PhotonView photonView = inhibitor.GetComponent<PhotonView>();
            Targetable targetable = inhibitor.GetComponent<Targetable>();
            if (PhotonNetwork.isMasterClient && targetable.allowTargetingBy == team) {
                photonView.RPC("Damage", PhotonTargets.All, damage, shooter);
            }
        } else if (nexus != null) {
            PhotonView photonView = nexus.GetComponent<PhotonView>();
            Targetable targetable = nexus.GetComponent<Targetable>();
            if (PhotonNetwork.isMasterClient && targetable.allowTargetingBy == team) {
                photonView.RPC("Damage", PhotonTargets.All, damage, shooter);
            }
        } else if (entity != null) {
            PhotonView photonView = entity.GetComponent<PhotonView>();
            if (PhotonNetwork.isMasterClient && entity.team != team) {
                photonView.RPC("EntityDamage", PhotonTargets.All, damage, shooter);
            }
        }

        Destroy(gameObject);
    }
}
