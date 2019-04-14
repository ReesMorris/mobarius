using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour {

    public delegate void OnPlayerMove();
    public static OnPlayerMove onPlayerMove;

    PhotonView photonView;
    Vector3 trueLoc;
    Quaternion trueRot;
    NavMeshAgent navMeshAgent;
    Champion champion;
    PlayerChampion playerChampion;
    PlayerAnimator playerAnimator;
    DefaultAttack defaultAttack;
    MapProperties mapProperties;

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

    public void StopMovement() {
        if (this != null) {
            playerAnimator.PlayAnimation("Idle");
            defaultAttack.target = null;
            navMeshAgent.velocity = Vector3.zero;
            navMeshAgent.isStopped = true;
        }
    }

    void Update() {
        if (!photonView.isMine) {
            transform.position = Vector3.Lerp(transform.position, trueLoc, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, trueRot, Time.deltaTime * 5);
        } else {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            navMeshAgent.speed = (champion.movementSpeed / 120f);
            if (Input.GetButton("Fire2") && !playerChampion.IsDead) {
                if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Targetable"))) {
                    Targetable targetable = hit.transform.GetComponent<Targetable>();
                    if (targetable == null || (targetable != null && targetable.allowTargetingBy == photonView.owner.GetTeam()))
                        defaultAttack.target = hit.transform.gameObject;
                } else if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Floor"))) {
                    if (mapProperties.display == PlayerCamera.CameraDisplays.TopDown) {
                        defaultAttack.target = null;
                        navMeshAgent.destination = hit.point;
                        navMeshAgent.isStopped = false;
                    }
                }
            }
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

            if (navMeshAgent.remainingDistance <= 0.2f || playerChampion.IsDead) {
                if (!navMeshAgent.isStopped)
                    playerAnimator.PlayAnimation("Idle");
                navMeshAgent.velocity = Vector3.zero;
                navMeshAgent.isStopped = true;
            }

            if (!navMeshAgent.isStopped && playerAnimator.CurrentAnimation != "Walking") {
                playerAnimator.PlayAnimation("Walking");
            }
            if (playerChampion.IsDead) {
                defaultAttack.target = null;
            }
            if (!navMeshAgent.isStopped) {
                if(onPlayerMove != null)
                onPlayerMove();
            }
        }
    }

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

    void OnGameEnd() {
        StopMovement();
        Destroy(this);
    }
}
