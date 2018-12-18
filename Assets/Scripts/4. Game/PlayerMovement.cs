using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour {

    PhotonView photonView;
    Vector3 trueLoc;
    Quaternion trueRot;
    NavMeshAgent navMeshAgent;

    void Start() {
        photonView = GetComponent<PhotonView>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update() {
        if (!photonView.isMine) {
            transform.position = Vector3.Lerp(transform.position, trueLoc, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, trueRot, Time.deltaTime * 5);
        } else {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Input.GetButtonDown("Fire2")) {
                if (Physics.Raycast(ray, out hit, 100)) {
                    navMeshAgent.destination = hit.point;
                    navMeshAgent.isStopped = false;
                }
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
