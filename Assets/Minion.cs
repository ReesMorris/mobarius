using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Minion : MonoBehaviour {

    public float speed;

    PhotonView photonView;
    NavMeshAgent navMeshAgent;
    Transform[] waypoints;
    int waypointIndex;

	void Start () {
        photonView = GetComponent<PhotonView>();
        navMeshAgent = GetComponent<NavMeshAgent>();
	}

    public void Init(Transform spawnPoint, Transform[] _waypoints, PunTeams.Team _team) {
        Start();
        transform.position = spawnPoint.position;
        waypoints = _waypoints;
        GetComponent<Entity>().team = _team;

        navMeshAgent.enabled = true;
        navMeshAgent.speed = speed / 120f;
        GoToNextWaypoint();
    }
	
	void Update () {
        if(photonView.isMine) {
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
}
