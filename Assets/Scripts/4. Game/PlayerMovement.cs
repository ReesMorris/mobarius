using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour {

    PhotonView photonView;
    Vector3 trueLoc;
    Quaternion trueRot;
    NavMeshAgent navMeshAgent;
    Champion champion;
    PlayerChampion playerChampion;
    DefaultAttack defaultAttack;

    void Start() {
        photonView = GetComponent<PhotonView>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerChampion = GetComponent<PlayerChampion>();
        defaultAttack = GetComponent<DefaultAttack>();
        champion = playerChampion.Champion;
    }

    void Update() {
        if (!photonView.isMine) {
            transform.position = Vector3.Lerp(transform.position, trueLoc, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, trueRot, Time.deltaTime * 5);
        } else {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Input.GetButton("Fire2") && !playerChampion.IsDead) {
                if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Targetable"))) {
                    Targetable targetable = hit.transform.GetComponent<Targetable>();
                    if (targetable == null || (targetable != null && targetable.allowTargetingBy == photonView.owner.GetTeam()))
                        defaultAttack.target = hit.transform.gameObject;
                } else if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Floor"))) {
                    if(!AbilityHandler.Instance.Aiming) {
                        defaultAttack.target = null;
                        navMeshAgent.destination = hit.point;
                        navMeshAgent.isStopped = false;
                        navMeshAgent.speed = (champion.movementSpeed / 120f);
                    }
                }
            }
            if (navMeshAgent.remainingDistance <= 0.2f || playerChampion.IsDead) {
                navMeshAgent.velocity = Vector3.zero;
                navMeshAgent.isStopped = true;
                defaultAttack.target = null;
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isReading) {
            if(!photonView.isMine) {
                this.trueLoc = (Vector3)stream.ReceiveNext();
            }
        } else {
            if(photonView.isMine){
                stream.SendNext(transform.position);
            }
        }
    }
}
