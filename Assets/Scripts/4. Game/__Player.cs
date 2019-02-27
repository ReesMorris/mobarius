using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Add this to a new player champion to add all of the necessary files

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
