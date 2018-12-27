using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DefaultAttack : MonoBehaviour {

    public float cooldownTime;
    public GameObject bulletPrefab;

    PhotonView photonView;
    PlayerChampion playerChampion;
    NavMeshAgent navMeshAgent;
    float lastShotTime;

    [HideInInspector] public GameObject target;

    void Start() {
        photonView = GetComponent<PhotonView>();
        playerChampion = GetComponent<PlayerChampion>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update() {
        if(photonView.isMine) {
            if (!playerChampion.IsDead) {
                if(target != null) {
                    PlayerChampion targetChampion = target.GetComponent<PlayerChampion>();
                    if (targetChampion != null)
                        if (targetChampion.IsDead)
                            target = null;
                    if(target != null) {
                        if (target.GetComponent<PhotonView>() != photonView) {
                            if (Vector3.Distance(target.transform.position, transform.position) < playerChampion.Champion.range) {
                                if(Time.time >= (lastShotTime + cooldownTime)) {
                                    lastShotTime = Time.time;
                                    photonView.RPC("Shoot", PhotonTargets.All, 100f, playerChampion.Champion.attackDamage, target.transform.position, target.GetComponent<PhotonView>().viewID, photonView.owner);
                                    navMeshAgent.destination = transform.position;
                                }
                            } else {
                                navMeshAgent.destination = target.transform.position;
                                navMeshAgent.isStopped = false;
                            }
                        }
                    }
                }
            }
        }
    }

    [PunRPC]
    void Shoot(float speed, float damage, Vector3 position, int photonId, PhotonPlayer shooter) {
        transform.LookAt(new Vector3(position.x, transform.position.y, position.z));
        GameObject bullet = Instantiate(bulletPrefab, (transform.position + transform.forward), transform.rotation);
        Bullet b = bullet.GetComponent<Bullet>();
        b.Setup(damage, transform.position, photonId, shooter);
    }
}
