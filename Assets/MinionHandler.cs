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

            if(newTime == 1f) {
                StartCoroutine(SpawnMinions(PunTeams.Team.blue));
                StartCoroutine(SpawnMinions(PunTeams.Team.red));
            }

            /*
            if(newTime == 35)
                GameUIHandler.Instance.MessageWithSound("Announcer/Minions30", "Thirty seconds until minions spawn");
            else if(newTime == 65)
                GameUIHandler.Instance.MessageWithSound("Announcer/Minions0", "Minions have spawned");
            else if(newTime >= 65) {
                if(newTime % 30 == 5) {
                    print("spawn miniones!");
                }
            }*/
        }
    }

    IEnumerator SpawnMinions(PunTeams.Team team) {
        int spawned = 0;
        while(spawned < 6) {
            photonView.RPC("SpawnMinion", PhotonTargets.All, team);
            spawned++;
            yield return new WaitForSeconds(0.5f);
        }
    }

    [PunRPC]
    void SpawnMinion(PunTeams.Team team) {
        GameObject minion = PhotonNetwork.Instantiate(minionPrefab.name, Vector3.zero, Quaternion.identity, 0);
        minion.name = minionPrefab.name;

        MinionWaypoints minionData;
        if (team == PunTeams.Team.blue)
            minionData = GameHandler.Instance.currentMap.blueMinions[0];
        else
            minionData = GameHandler.Instance.currentMap.redMinions[0];

        minion.GetComponent<Entity>().Init(477);
        minion.GetComponent<Minion>().Init(minionData.spawnPosition, minionData.destinations, team);
    }
}
