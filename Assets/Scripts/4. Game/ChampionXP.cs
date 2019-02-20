using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampionXP : MonoBehaviour {

    public delegate void OnChampionLevelUp(PhotonPlayer player);
    public static OnChampionLevelUp onChampionLevelUp;
    public delegate void OnChampionReceiveXP(PhotonPlayer player);
    public static OnChampionReceiveXP onChampionReceiveXP;

    int maxLevel = 18;
    int firstLevelXP = 280;
    int levelIncrement = 100;

    int currentXP = 0;
    int currentLevel = 1;
    int nextLevelXP;
    public PhotonView photonView { get; protected set; }

    void Start() {
        photonView = GetComponent<PhotonView>();
        nextLevelXP = firstLevelXP;

        Turret.onTurretDestroyed += OnTurretDestroyed;
    }

    // Called when a turret is destroyed; give global XP to every player on the team who destroyed it
    void OnTurretDestroyed(Turret t) {
        if(photonView.isMine) {
            if (t.team != PhotonNetwork.player.GetTeam()) {
                photonView.RPC("GiveXP", PhotonTargets.AllBuffered, t.XPOnDeath);
            }
        }
    }

    [PunRPC]
    public void GiveXP(int amount) {
        if(currentLevel < maxLevel) {

            // Give the XP and call the delegate to say we're awarding XP
            currentXP += amount;
            if (onChampionReceiveXP != null)
                onChampionReceiveXP(photonView.owner);

            // Have we levelled up?
            while (currentXP > nextLevelXP) {
                currentLevel = Mathf.Min(currentLevel + 1, maxLevel);
                nextLevelXP += levelIncrement;

                if (onChampionLevelUp != null)
                    onChampionLevelUp(photonView.owner);
            }
        }
    }
}
