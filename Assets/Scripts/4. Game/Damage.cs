using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    The class handling information about damage received by a player
*/
/// <summary>
/// The class handling information about damage received by a player.
/// </summary>
public class Damage : MonoBehaviour {

    // Public variables
    public PhotonView player;
    public float amount;
    public int timeInflicted;

    /// <summary>
    /// Called to initialise the class.
    /// </summary>
    /// <param name="_player">The PhotonView of the attacker</param>
    /// <param name="_amount">The amount of damage taken</param>
    /// <param name="_timeInflicted">The time the damage was taken</param>
    public Damage(PhotonView _player, float _amount, int _timeInflicted)  {
        player = _player;
        amount = _amount;
        timeInflicted = _timeInflicted;
    }
}
