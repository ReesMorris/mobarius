using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
    This script is to ensure that no core scripts are missed when adding a new player.
    Simply attach this script to a new player and run the game once.
*/

[RequireComponent(typeof(PlayerChat))]
[RequireComponent(typeof(PlayerAnimator))]
[RequireComponent(typeof(ChampionXP))]
[RequireComponent(typeof(NavMeshObstacle))]
[RequireComponent(typeof(Entity))]
[RequireComponent(typeof(RecallAbility))]
[RequireComponent(typeof(DefaultAttack))]
[RequireComponent(typeof(StatRegeneration))]
[RequireComponent(typeof(PlayerChampion))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PhotonView))]
public class __Player : MonoBehaviour {
}
