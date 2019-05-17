using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
    This script contains functions relevant to minion behaviour
*/
/// <summary>
/// This script contains functions relevant to minion behaviour.
/// </summary>
public class Minion : MonoBehaviour {

    // Public variables
    public float speed;
    public float range;
    public float attackSpeed;
    public float attackDamage;
    public GameObject bulletPrefab;
    public Entity Entity { get; protected set; }

    // Private variables
    PhotonView photonView;
    Quaternion trueRot;
    NavMeshAgent navMeshAgent;
    Transform[] waypoints;
    Targetable targetable;
    int waypointIndex;

    // Set up private variables when the game begins.
    void Start() {
        Entity = GetComponent<Entity>();
        photonView = GetComponent<PhotonView>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        targetable = GetComponent<Targetable>();
    }

    /// <summary>
    /// Initiates a minion on the network.
    /// </summary>
    /// <param name="packIndex">The position of the minion in the group it spawns with</param>
    /// <param name="team">The team of the minion</param>
    public void Init(int packIndex, PunTeams.Team team) {
        Start();
        photonView.RPC("MinionInit", PhotonTargets.All, packIndex, team);
    }

    // Initiates the minion across the network
    [PunRPC]
    void MinionInit(int packIndex, PunTeams.Team team) {
        Start();
        if (photonView.isMine) {
            MinionWaypoints minionData;

            // Set the team
            if (team == PunTeams.Team.blue)
                minionData = GameHandler.Instance.currentMap.blueMinions[0];
            else
                minionData = GameHandler.Instance.currentMap.redMinions[0];

            // Set up waypoints and the spawn position
            transform.position = minionData.spawnPosition.position;
            waypoints = minionData.destinations;
            navMeshAgent.enabled = true;
            GoToWaypoint();
        }
        GetComponent<Entity>().team = team;

        // Set up name and attack range
        gameObject.name = "Minion";
        range += packIndex;

        // Set up enemies
        if (team == PunTeams.Team.blue)
            targetable.allowTargetingBy = PunTeams.Team.red;
        else if (team == PunTeams.Team.red)
            targetable.allowTargetingBy = PunTeams.Team.blue;
    }

    // Attempt to go to the next waypoint every frame
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

    /// <summary>
    /// The minion will approach the next waypoint.
    /// </summary>
    public void GoToWaypoint() {
        navMeshAgent.destination = waypoints[waypointIndex].position;
        navMeshAgent.isStopped = false;
    }
}
