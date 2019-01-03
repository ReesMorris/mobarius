using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionHandler : MonoBehaviour {

    public GameObject minionPrefab;

    PhotonView photonView;

    void Start() {
        photonView = GetComponent<PhotonView>();
        GameUIHandler.onGameTimeUpdate += OnGameTimeUpdate;
    }

    void OnGameTimeUpdate(int newTime) {
        if(PhotonNetwork.isMasterClient) {
            if(newTime == 35)
                GameUIHandler.Instance.MessageWithSound("Announcer/Minions30", "Thirty seconds until minions spawn");
            else if(newTime == 65)
                GameUIHandler.Instance.MessageWithSound("Announcer/Minions0", "Minions have spawned");
            if(newTime >= 65) {
                if(newTime % 30 == 5) {
                    StartCoroutine(SpawnMinions(PunTeams.Team.blue));
                    StartCoroutine(SpawnMinions(PunTeams.Team.red));
                }
            }
        }
    }

    IEnumerator SpawnMinions(PunTeams.Team team) {
        if (PhotonNetwork.isMasterClient) {
            for(int i = 0; i < 6; i++) {
                SpawnMinion(i, team);
                yield return new WaitForSeconds(0.5f);
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
