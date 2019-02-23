using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsHandler : MonoBehaviour {

    public static EffectsHandler Instance;
    public enum Effects { Recall };

    public Effect recallEffectPrefab;

    PhotonView photonView;
    public Effect recallEffect { get; protected set; }

    void Awake() {
        Instance = this;
        photonView = GetComponent<PhotonView>();
    }

    public void SetupEffects(GameObject player, PhotonView photonView) {
        SetupRecallEffect(player, photonView);
    }

    void SetupRecallEffect(GameObject player, PhotonView pv) {
        recallEffect = PhotonNetwork.Instantiate(recallEffectPrefab.name, player.transform.position, Quaternion.identity, 0).GetComponent<Effect>();
        recallEffect.Init(pv.viewID);
    }

}
