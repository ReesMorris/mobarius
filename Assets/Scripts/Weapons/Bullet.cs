using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This script handles the bullet
*/
/// <summary>
/// This script handles the bullet.
/// </summary>
public class Bullet : MonoBehaviour {

    // Private variables
    float damage;
    float range;
    Vector3 startingPos;
    int shooterId;
    GameObject target;
    PunTeams.Team team;
    CapsuleCollider capsuleCollider;
    bool hadTarget;

    /// <summary>
    /// Set up the bullet on the network to travel a limited distance in a straight direction.
    /// </summary>
    /// <param name="_damage">The damage dealt by the bullet</param>
    /// <param name="_team">The team the bullet is assigned to</param>
    /// <param name="_startingPos">The 3D position in the world the bullet spawns from</param>
    /// <param name="_range">The range of travel for the bullet before it is destroyed</param>
    /// <param name="_shooterId">The viewID of the attacker</param>
    public void Setup(float _damage, PunTeams.Team _team, Vector3 _startingPos, float _range, int _shooterId) {
        ValidateSetup();
        shooterId = _shooterId;
        team = _team;
        damage = _damage;
        startingPos = _startingPos;
        range = _range;
    }

    /// <summary>
    /// Set up the bullet on the network to guarantee a hit on a target.
    /// </summary>
    /// <param name="_damage">The damage dealt by the bullet</param>
    /// <param name="_team">The team the bullet is assigned to</param>
    /// <param name="_startingPos">The 3D position in the world the bullet spawns from</param>
    /// <param name="photonId">The viewID of the target</param>
    /// <param name="_shooterId">The viewID of the attacker</param>
    public void Setup(float _damage, PunTeams.Team _team, Vector3 _startingPos, int photonId, int _shooterId) {
        ValidateSetup();
        shooterId = _shooterId;
        team = _team;
        damage = _damage;
        startingPos = _startingPos;
        target = PhotonView.Find(photonId).gameObject;
        capsuleCollider.enabled = false;
        hadTarget = true;
    }

    // Grabs the capsule collider of the bullet (to be replaced when a new mesh is used)
    void ValidateSetup() {
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    // Called every frame
    void Update() {

        // Does a target exist?
        if(target != null) {

            // Is the target.. targetable (ie. alive)?
            if (target.layer == LayerMask.NameToLayer("Targetable")) {
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, 10f * Time.deltaTime);
                if(Vector3.Distance(transform.position, target.transform.position) < .5f) {
                    OnCollide(target);
                }
            } else {
                target = null;
                Destroy(gameObject);
            }
        }
        
        // Target does not exist
        else {

            // Destroy the bullet after exceeding a certain range
            if(hadTarget || Vector3.Distance(transform.position, startingPos) > range) {
                Destroy(gameObject);
            }
        }
    }

    // Called when the bullet collides with an object
    void OnCollisionEnter(Collision collision) {
        OnCollide(collision.gameObject);
    }

    // Called when the bullet collides with an object
    void OnCollide(GameObject collision) {
        PlayerChampion playerChampion = collision.GetComponent<PlayerChampion>();
        Entity entity = collision.GetComponent<Entity>();
        Turret turret = collision.GetComponent<Turret>();
        Inhibitor inhibitor = collision.GetComponent<Inhibitor>();
        Nexus nexus = collision.GetComponent<Nexus>();

        // Collision with player champion
        if (playerChampion != null) {
            PhotonView photonView = playerChampion.GetComponent<PhotonView>();
            if (PhotonNetwork.isMasterClient && photonView.owner.GetTeam() != team) {
                photonView.RPC("Damage", PhotonTargets.All, damage, shooterId);
            }
        }
        
        // Collision with turret
        else if (turret != null) {
            PhotonView photonView = turret.GetComponent<PhotonView>();
            Targetable targetable = turret.GetComponent<Targetable>();
            if (PhotonNetwork.isMasterClient && targetable.allowTargetingBy == team) {
                photonView.RPC("Damage", PhotonTargets.All, damage, shooterId);
            }
        }
        
        // Collision with inhibitor
        else if (inhibitor != null) {
            PhotonView photonView = inhibitor.GetComponent<PhotonView>();
            Targetable targetable = inhibitor.GetComponent<Targetable>();
            if (PhotonNetwork.isMasterClient && targetable.allowTargetingBy == team) {
                photonView.RPC("Damage", PhotonTargets.All, damage, shooterId);
            }
        }
        
        // Collision with nexus
        else if (nexus != null) {
            PhotonView photonView = nexus.GetComponent<PhotonView>();
            Targetable targetable = nexus.GetComponent<Targetable>();
            if (PhotonNetwork.isMasterClient && targetable.allowTargetingBy == team) {
                photonView.RPC("Damage", PhotonTargets.All, damage, shooterId);
            }
        }
        
        // Collision with entity (ie. minion)
        else if (entity != null) {
            PhotonView photonView = entity.GetComponent<PhotonView>();
            if (PhotonNetwork.isMasterClient && entity.team != team) {
                photonView.RPC("EntityDamage", PhotonTargets.All, damage, shooterId);
            }
        }

        // Destroy the bullet regardless of the above
        Destroy(gameObject);
    }
}
