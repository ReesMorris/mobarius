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

    void Start() {
        photonView = GetComponent<PhotonView>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerChampion = GetComponent<PlayerChampion>();
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
                if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Floor"))) {
                    navMeshAgent.destination = hit.point;
                    navMeshAgent.isStopped = false;
                    navMeshAgent.speed = (champion.movementSpeed / 120f);
                }
            }
            if (navMeshAgent.remainingDistance <= 0.2f) {
                navMeshAgent.velocity = Vector3.zero;
                navMeshAgent.Stop();
            }
            //transform.LookAt(new Vector3(navMeshAgent.destination.x, transform.position.y, navMeshAgent.destination.z));
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
