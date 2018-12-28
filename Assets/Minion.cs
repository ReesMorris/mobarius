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

    public void Init(PunTeams.Team team) {
        Start();
        photonView.RPC("MinionInit", PhotonTargets.All, team);
    }

    [PunRPC]
    void MinionInit(PunTeams.Team team) {
        Start();
        if (photonView.isMine) {
            MinionWaypoints minionData;
            if (team == PunTeams.Team.blue)
                minionData = GameHandler.Instance.currentMap.blueMinions[0];
            else
                minionData = GameHandler.Instance.currentMap.redMinions[0];
            transform.position = minionData.spawnPosition.position;
            waypoints = minionData.destinations;
            navMeshAgent.enabled = true;
            navMeshAgent.speed = speed / 120f;
            GoToNextWaypoint();
        }
        GetComponent<Entity>().team = team;
        gameObject.name = "Minion";
        if (team == PunTeams.Team.blue)
            targetable.allowTargetingBy = PunTeams.Team.red;
        else if (team == PunTeams.Team.red)
            targetable.allowTargetingBy = PunTeams.Team.blue;
    }
	
	void Update () {
        if (!photonView.isMine) {
            //transform.position = Vector3.Lerp(transform.position, trueLoc, Time.deltaTime * 5);
            //transform.rotation = Quaternion.Lerp(transform.rotation, trueRot, Time.deltaTime * 5);
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
