using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    The script used to set up particle effects
*/
/// <summary>
/// The script used to set up particle effects.
/// </summary>
public class EffectsHandler : MonoBehaviour {

    // Public variables
    public static EffectsHandler Instance;
    public enum Effects { Recall };

    public Effect recallEffectPrefab;

    // Private variables
    PhotonView photonView;
    public Effect recallEffect { get; protected set; }

    // Allow other scripts to access this and set up references on game start
    void Awake() {
        Instance = this;
        photonView = GetComponent<PhotonView>();
    }

    /// <summary>
    /// Sets up the particle effects on a specified player.
    /// </summary>
    /// <param name="player">The GameObject of the player</param>
    /// <param name="photonView">The PhotonView of the player</param>
    public void SetupEffects(GameObject player, PhotonView photonView) {
        SetupRecallEffect(player, photonView);
    }

    // Sets up the recall effect
    void SetupRecallEffect(GameObject player, PhotonView pv) {
        recallEffect = PhotonNetwork.Instantiate(recallEffectPrefab.name, player.transform.position, Quaternion.identity, 0).GetComponent<Effect>();
        recallEffect.Init(pv.viewID);
    }

}
