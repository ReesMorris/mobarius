using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Minion : MonoBehaviour {

    public float speed;
    public float range;

    PhotonView photonView;
    Vector3 trueLoc;
    Quaternion trueRot;
    NavMeshAgent navMeshAgent;
    Transform[] waypoints;
    Targetable targetable;
    int waypointIndex;

	void Start () {
        photonView = GetComponent<PhotonView>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        targetable = GetComponent<Targetable>();
	}

    public void Init(Transform spawnPoint, Transform[] _waypoints, PunTeams.Team _team) {
        Start();
        transform.position = spawnPoint.position;
        waypoints = _waypoints;
        GetComponent<Entity>().team = _team;

        if (_team == PunTeams.Team.blue)
            targetable.allowTargetingBy = PunTeams.Team.red;
        else if (_team == PunTeams.Team.red)
            targetable.allowTargetingBy = PunTeams.Team.blue;

        navMeshAgent.enabled = true;
        navMeshAgent.speed = speed / 120f;
        GoToNextWaypoint();
    }
	
	void Update () {
        if (!photonView.isMine) {
            transform.position = Vector3.Lerp(transform.position, trueLoc, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, trueRot, Time.deltaTime * 5);
        } else {
            if(navMeshAgent.remainingDistance <= 0.2f) {
                if(waypoints.Length > (waypointIndex + 1)) {
                    waypointIndex++;
                    GoToNextWaypoint();
                }
            }
        }
	}

    void GoToNextWaypoint() {
        navMeshAgent.destination = waypoints[waypointIndex].position;
        navMeshAgent.isStopped = false;
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
