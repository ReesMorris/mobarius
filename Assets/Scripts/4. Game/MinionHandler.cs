using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionHandler : MonoBehaviour {

    public GameObject minionPrefab;

    void Start() {
        GameUIHandler.onGameTimeUpdate += OnGameTimeUpdate;
    }

    void OnGameTimeUpdate(int newTime) {
        if(newTime == 3)
            MinionWave();
        if (newTime == 35)
            GameUIHandler.Instance.MessageWithSound("Announcer/Minions30", "Thirty seconds until minions spawn");
        else if(newTime == 65)
            GameUIHandler.Instance.MessageWithSound("Announcer/Minions0", "Minions have spawned");
        if(newTime >= 65) {
            if(newTime % 30 == 5) {
                MinionWave();
            }
        }
    }

    void MinionWave() {
        if (PhotonNetwork.isMasterClient) {
            StartCoroutine(SpawnMinions(PunTeams.Team.blue));
            StartCoroutine(SpawnMinions(PunTeams.Team.red));
        }
    }

    IEnumerator SpawnMinions(PunTeams.Team team) {
        if (PhotonNetwork.isMasterClient) {
            for(int i = 0; i < 6; i++) {
                SpawnMinion(i, team);
                yield return new WaitForSeconds(1.2f);
            }
        }
    }

    [PunRPC]
    void SpawnMinion(int packIndex, PunTeams.Team team) {
        if (PhotonNetwork.isMasterClient) {
            GameObject minion = PhotonNetwork.Instantiate(minionPrefab.name, Vector3.zero, Quaternion.identity, 0);
            minion.GetComponent<Entity>().Init(477);
            minion.GetComponent<Minion>().Init(packIndex, team);
        }
    }
}
