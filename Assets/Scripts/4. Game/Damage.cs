using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour {

    public Damage(PhotonPlayer _player, float _amount, int _timeInflicted)  {
        player = _player;
        amount = _amount;
        timeInflicted = _timeInflicted;
    }

    public PhotonPlayer player;
    public float amount;
    public int timeInflicted;
}
