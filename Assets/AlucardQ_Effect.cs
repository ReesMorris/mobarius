using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlucardQ_Effect : MonoBehaviour {

    PhotonView photonView;

    public void Init(float radius, float damage, int attackerId) {
        photonView = GetComponent<PhotonView>();
        photonView.RPC("InitRPC", PhotonTargets.AllBuffered, radius, damage, attackerId);
    }

    [PunRPC]
    void InitRPC(float radius, float damage, int attackerId) {
        PhotonPlayer attacker = PhotonView.Find(attackerId).owner;

        radius /= 2f;
        transform.position += Vector3.up * 0.3f;
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren) {
            child.localScale = new Vector3(radius, 1, radius);
        }

        if(PhotonNetwork.isMasterClient) {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius * 2f);
            for (int i = 0; i < hitColliders.Length; i++) {
                Entity entity = hitColliders[i].GetComponent<Entity>();
                if (entity != null) {
                    PhotonView targetView = entity.GetComponent<PhotonView>();
                    PhotonPlayer target = targetView.owner;
                    if (entity.team != attacker.GetTeam()) {
                        if (entity.GetComponent<PlayerChampion>() != null)
                            targetView.RPC("Damage", PhotonTargets.AllBuffered, damage, attackerId);
                        else if (entity.GetComponent<Minion>() != null)
                            targetView.RPC("EntityDamage", PhotonTargets.AllBuffered, damage, attackerId);
                    }
                }
            }
        }
    }
}
