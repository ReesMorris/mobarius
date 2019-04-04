using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Minion : MonoBehaviour {

    public float speed;
    public float range;
    public float attackSpeed;
    public float attackDamage;
    public GameObject bulletPrefab;
    public Entity Entity { get; protected set; }

    PhotonView photonView;
    Quaternion trueRot;
    NavMeshAgent navMeshAgent;
    Transform[] waypoints;
    Targetable targetable;
    int waypointIndex;

    void Start() {
        Entity = GetComponent<Entity>();
        photonView = GetComponent<PhotonView>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        targetable = GetComponent<Targetable>();
    }

    public void Init(int packIndex, PunTeams.Team team) {
        Start();
        photonView.RPC("MinionInit", PhotonTargets.All, packIndex, team);
    }

    [PunRPC]
    void MinionInit(int packIndex, PunTeams.Team team) {
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
            GoToWaypoint();
        }
        GetComponent<Entity>().team = team;
        gameObject.name = "Minion";
        range += packIndex;
        if (team == PunTeams.Team.blue)
            targetable.allowTargetingBy = PunTeams.Team.red;
        else if (team == PunTeams.Team.red)
            targetable.allowTargetingBy = PunTeams.Team.blue;
    }

    void Update() {
        if (photonView.isMine) {
            if (waypoints != null) {
                if (navMeshAgent.remainingDistance <= 0.2f) {
                    if (waypoints.Length > (waypointIndex + 1)) {
                        waypointIndex++;
                        GoToWaypoint();
                    }
                }
            }
            navMeshAgent.speed = speed / 120f;
        }
    }

    public void GoToWaypoint() {
        navMeshAgent.destination = waypoints[waypointIndex].position;
        navMeshAgent.isStopped = false;
    }
}
