using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
    This script focuses on handling player character movement
*/
/// <summary>
/// This script focuses on handling player character movement.
/// </summary>
public class PlayerMovement : MonoBehaviour {

    // Public variables
    public delegate void OnPlayerMove();
    public static OnPlayerMove onPlayerMove;

    // Private variables
    PhotonView photonView;
    Vector3 trueLoc;
    Quaternion trueRot;
    NavMeshAgent navMeshAgent;
    Champion champion;
    PlayerChampion playerChampion;
    PlayerAnimator playerAnimator;
    DefaultAttack defaultAttack;
    MapProperties mapProperties;

    // Subscribe to delegates and assign references when the game starts.
    void Start() {
        GameHandler.onGameEnd += OnGameEnd;
        trueRot = Quaternion.identity;
        photonView = GetComponent<PhotonView>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerChampion = GetComponent<PlayerChampion>();
        defaultAttack = GetComponent<DefaultAttack>();
        playerAnimator = GetComponent<PlayerAnimator>();
        champion = playerChampion.Champion;
        mapProperties = MapManager.Instance.GetMapProperties();
    }

    /// <summary>
    /// Stops the local player's movement. Does not prevent further movements from being made immediately after.
    /// </summary>
    public void StopMovement() {
        if (this != null) {
            playerAnimator.PlayAnimation("Idle");
            defaultAttack.target = null;
            navMeshAgent.velocity = Vector3.zero;
            navMeshAgent.isStopped = true;
        }
    }

    // Called every frame
    void Update() {

        // Tell other clients where this player is in the world (and their rotation)
        if (!photonView.isMine) {
            transform.position = Vector3.Lerp(transform.position, trueLoc, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, trueRot, Time.deltaTime * 5);
        }
        
        // Move this player?
        else {

            // Get current 3D mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Set the movement speed
            navMeshAgent.speed = (champion.movementSpeed / 120f);

            if (Input.GetButton("Fire2") && !playerChampion.IsDead) {

                // Are we attacking an enemy?
                if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Targetable"))) {
                    Targetable targetable = hit.transform.GetComponent<Targetable>();

                    // Is the enemy actually an enemy (on the other team)?
                    if (targetable == null || (targetable != null && targetable.allowTargetingBy == photonView.owner.GetTeam()))
                        defaultAttack.target = hit.transform.gameObject;
                } 
                
                // Are we trying to move on the ground?
                else if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Floor"))) {

                    // Are we in the top-down camera mode?
                    if (mapProperties.display == PlayerCamera.CameraDisplays.TopDown) {
                        defaultAttack.target = null;
                        navMeshAgent.destination = hit.point;
                        navMeshAgent.isStopped = false;
                    }
                }
            }

            // Support for third-person camera mode using WASD instead of right-click
            if (mapProperties.display == PlayerCamera.CameraDisplays.ThirdPerson) {
                if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
                    playerAnimator.PlayAnimation("Walking");
                else
                    playerAnimator.PlayAnimation("Idle");
                if (Input.GetKey(KeyCode.W))
                    navMeshAgent.Move(transform.forward / 18f);
                if (Input.GetKey(KeyCode.A))
                    transform.Rotate(Vector3.down * 3f);
                if (Input.GetKey(KeyCode.S))
                    navMeshAgent.Move(-transform.forward / 18f);
                if (Input.GetKey(KeyCode.D))
                    transform.Rotate(Vector3.up * 3f);
            }
            
            // Stop moving if we have reached the destination or we are dead
            if (navMeshAgent.remainingDistance <= 0.2f || playerChampion.IsDead) {
                if (!navMeshAgent.isStopped)
                    playerAnimator.PlayAnimation("Idle");
                navMeshAgent.velocity = Vector3.zero;
                navMeshAgent.isStopped = true;
            }

            // Update the animation if all else fails
            if (!navMeshAgent.isStopped && playerAnimator.CurrentAnimation != "Walking") {
                playerAnimator.PlayAnimation("Walking");
            }

            // Clear the target of our default attack when we die
            if (playerChampion.IsDead) {
                defaultAttack.target = null;
            }

            // Update the animation to walking if all else fails (volume 2)
            if (!navMeshAgent.isStopped) {
                if(onPlayerMove != null)
                onPlayerMove();
            }
        }
    }

    /// <summary>
    /// Updates player position over the network.
    /// </summary>
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isReading) {
            if (!photonView.isMine) {
                this.trueLoc = (Vector3)stream.ReceiveNext();
            }
        } else {
            if (photonView.isMine) {
                stream.SendNext(transform.position);
            }
        }
    }

    // Called when the game ends
    void OnGameEnd() {
        StopMovement();
        Destroy(this);
    }
}
