using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampionXP : MonoBehaviour {

    public delegate void OnChampionLevelUp(Champion champion, PhotonPlayer player, int level, int unclaimedUpgrades);
    public static OnChampionLevelUp onChampionLevelUp;
    public delegate void OnChampionReceiveXP(PhotonPlayer player, float progress);
    public static OnChampionReceiveXP onChampionReceiveXP;
    public delegate void OnChampionUpgradeAbility(PhotonPlayer player, Champion champion, int unclaimedUpgrades);
    public static OnChampionUpgradeAbility onChampionUpgradeAbility;

    int maxLevel = 18;
    int firstLevelXP = 280;
    int levelIncrement = 100;

    int currentXP = 0;
    int nextLevelXP;
    int previousLevelXP;
    int unclaimedUpgrades;
    int totalUnclaimed;
    Champion champion;
    public PhotonView photonView { get; protected set; }

    void Start() {
        photonView = GetComponent<PhotonView>();
        nextLevelXP = firstLevelXP;
        champion = GetComponent<PlayerChampion>().Champion;
        GameUIHandler.onUpgradeButtonClicked += OnUpgradeButtonClicked;

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

    void Update() {
        if(photonView.isMine) {
            if(Input.GetKeyDown(KeyCode.Z)) {
                photonView.RPC("GiveXP", PhotonTargets.AllBuffered, 50);
            }
        }
    }

    [PunRPC]
    public void GiveXP(int amount) {
        if(champion.currentLevel < maxLevel) {

            // Give the XP
            currentXP += amount;

            // Have we levelled up?
            while (currentXP > nextLevelXP) {
                previousLevelXP = nextLevelXP;
                champion.currentLevel = Mathf.Min(champion.currentLevel + 1, maxLevel);
                nextLevelXP += levelIncrement;

                if (totalUnclaimed < maxLevel) {
                    unclaimedUpgrades++;
                    totalUnclaimed++;
                }

                if (onChampionLevelUp != null && photonView.isMine) {
                    champion.OnLevelUp();
                    onChampionLevelUp(champion, photonView.owner, champion.currentLevel, unclaimedUpgrades);
                }
            }

            // Make a call saying we've awarded XP (after checking level)
            float progress = ((float)(currentXP- previousLevelXP) / (float)(nextLevelXP-previousLevelXP));
            if (champion.currentLevel == maxLevel) progress = 1;
            if (onChampionReceiveXP != null)
                onChampionReceiveXP(photonView.owner, progress);
        }
    }

    // Called when an upgrade button is clicked by the player
    void OnUpgradeButtonClicked(AbilityHandler.Abilities abilityKey) {
        unclaimedUpgrades = Mathf.Max(unclaimedUpgrades - 1, 0);
        foreach(Ability ability in champion.abilities) {
            if(ability.abilityKey == abilityKey) {
                champion.UpgradeAbility(abilityKey);
                
                // Tell other scripts we upgraded
                if (onChampionUpgradeAbility != null)
                    onChampionUpgradeAbility(photonView.owner, champion, unclaimedUpgrades);
                break;
            }
        }
    }
}
