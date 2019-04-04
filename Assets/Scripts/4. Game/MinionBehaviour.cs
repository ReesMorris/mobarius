using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MinionBehaviour : MonoBehaviour {

    public GameObject radiusTrigger;
    public Minion Minion { get; protected set; }

    List<Entity> withinRadius;
    PhotonView photonView;
    NavMeshAgent navMeshAgent;
    Entity target;
    float timeSinceLastShot;

    void Start() {
        withinRadius = new List<Entity>();
        Minion = GetComponent<Minion>();
        photonView = GetComponent<PhotonView>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        radiusTrigger.transform.localScale = new Vector3(Minion.range / 12f, radiusTrigger.transform.localScale.y, Minion.range / 12f);
    }
	void Update () {
        if (photonView.isMine) {
            RemoveDeadEntities();
            if(target == null || target.enabled == false) {
                Minion.GoToWaypoint();
                target = GetBestEnemy();
            } else {
                if (Minion.Entity.GetMovementSpeed() != 0f) {
                    if (Vector3.Distance(transform.position, target.transform.position) <= Minion.range / 12f) {
                        navMeshAgent.isStopped = true;
                        if (timeSinceLastShot + Minion.attackSpeed < Time.time) {
                            timeSinceLastShot = Time.time;
                            photonView.RPC("Shoot", PhotonTargets.All, 100f, Minion.Entity.team, transform.position, Minion.attackDamage, target.GetComponent<PhotonView>().viewID, photonView.viewID);
                        }
                    } else {
                        navMeshAgent.destination = target.transform.position;
                    }
                }
            }
        }
	}

    // Removes dead entities from the array
    void RemoveDeadEntities() {
        foreach(Entity e in withinRadius) {
            if(e != null)
                if(e.GetIsDead() || e.enabled == false)
                    EnemyLeaveRadius(e);
        }
    }

    // Will focus on minions more than players
    Entity GetBestEnemy() {
        if(withinRadius.Count > 0) {
            foreach(Entity e in withinRadius) {
                if (e == null)
                    EnemyLeaveRadius(e);
                else if (e.GetComponent<PlayerChampion>() == null) {
                    return e;
                }
            }
            return withinRadius[0];
        }
        return null;
    }

    public void EnemyEnterRadius(Entity entity) {
        withinRadius.Add(entity);
    }

    public void EnemyLeaveRadius(Entity entity) {
        withinRadius.Remove(entity);
        if (target == entity)
            target = null;
    }

    [PunRPC]
    void Shoot(float speed, PunTeams.Team team, Vector3 spawnPos, float damage, int photonId, int shooter) {
        GameObject bullet = Instantiate(Minion.bulletPrefab, spawnPos, transform.rotation);
        Bullet b = bullet.GetComponent<Bullet>();
        b.Setup(damage, team, transform.position, photonId, shooter);
    }
}
