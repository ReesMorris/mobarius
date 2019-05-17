using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This script handles functions for spawning minion waves
*/
/// <summary>
/// This script handles functions for spawning minion waves.
/// </summary>
public class MinionHandler : MonoBehaviour {

    // Public variables
    public GameObject minionPrefab;

    // Private variables
    MapProperties mapProperties;

    // Listen for delegates when the game begins.
    void Start() {
        GameUIHandler.onGameTimeUpdate += OnGameTimeUpdate;
        GameHandler.onGameStart += OnGameStart;
    }

    // When the match begins, pull the latest properties of the current map.
    void OnGameStart() {
        mapProperties = MapManager.Instance.GetMapProperties();
    }

    // Every time the game timer is updated, check to see if we should spawn a new minion wave
    void OnGameTimeUpdate(int newTime) {
        if (newTime == (mapProperties.minionSpawnTime - 30))
            GameUIHandler.Instance.MessageWithSound("Announcer/Minions30", "Thirty seconds until minions spawn");
        else if(newTime == mapProperties.minionSpawnTime)
            GameUIHandler.Instance.MessageWithSound("Announcer/Minions0", "Minions have spawned");
        if(newTime >= mapProperties.minionSpawnTime) {
            if((newTime - mapProperties.minionSpawnTime) % mapProperties.minionSpawnDelay == 0) {
                MinionWave();
            }
        }
    }

    // Master client to spawn minion waves for each team
    void MinionWave() {
        if (PhotonNetwork.isMasterClient) {
            StartCoroutine(SpawnMinions(PunTeams.Team.blue));
            StartCoroutine(SpawnMinions(PunTeams.Team.red));
        }
    }

    // Spawn minions spaced apart from one another by 1.2 seconds
    IEnumerator SpawnMinions(PunTeams.Team team) {
        if (PhotonNetwork.isMasterClient) {
            for(int i = 0; i < 6; i++) {
                SpawnMinion(i, team);
                yield return new WaitForSeconds(1.2f);
            }
        }
    }

    // Spawn minions on the network and initialise them
    [PunRPC]
    void SpawnMinion(int packIndex, PunTeams.Team team) {
        if (PhotonNetwork.isMasterClient) {
            GameObject minion = PhotonNetwork.Instantiate(minionPrefab.name, Vector3.zero, Quaternion.identity, 0);
            minion.GetComponent<Entity>().Init(477);
            minion.GetComponent<Minion>().Init(packIndex, team);
        }
    }
}
