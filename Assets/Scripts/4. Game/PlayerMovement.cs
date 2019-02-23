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

    void Start() {
        GameHandler.onGameEnd += OnGameEnd;
        photonView = GetComponent<PhotonView>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerChampion = GetComponent<PlayerChampion>();
        defaultAttack = GetComponent<DefaultAttack>();
        playerAnimator = GetComponent<PlayerAnimator>();
        champion = playerChampion.Champion;
    }

    public void StopMovement() {
        if (!navMeshAgent.isStopped)
            playerAnimator.PlayAnimation("Idle");
        defaultAttack.target = null;
        navMeshAgent.velocity = Vector3.zero;
        navMeshAgent.isStopped = true;
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
                    if(navMeshAgent.isStopped)
                        playerAnimator.PlayAnimation("Walking");
                    defaultAttack.target = null;
                    navMeshAgent.destination = hit.point;
                    navMeshAgent.isStopped = false;
                    navMeshAgent.speed = (champion.movementSpeed / 120f);
                }
            }
            if (navMeshAgent.remainingDistance <= 0.2f || playerChampion.IsDead) {
                if (!navMeshAgent.isStopped)
                    playerAnimator.PlayAnimation("Idle");
                navMeshAgent.velocity = Vector3.zero;
                navMeshAgent.isStopped = true;
            }
            if(playerChampion.IsDead) {
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
        Destroy(this);
    }
}
