using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlucardW_Effect : MonoBehaviour {

    PhotonView photonView;
    PhotonPlayer attacker;
    float damageBase;
    int attackerIdBase;

    public void Init(float radius, float damage, Vector3 direction, int attackerId) {
        photonView = GetComponent<PhotonView>();
        photonView.RPC("InitRPC", PhotonTargets.AllBuffered, radius, damage, direction, attackerId);
    }

    [PunRPC]
    void InitRPC(float radius, float damage, Vector3 direction, int attackerId) {
        attackerIdBase = attackerId;
        attacker = PhotonView.Find(attackerId).owner;
        damageBase = damage;
        gameObject.transform.LookAt(direction);

        if (PhotonNetwork.isMasterClient)
            StartCoroutine("Delete");
    }

    // Called by AlucardW_Trigger by the Master Client
    public void OnTrigger(Entity entity) {
        PhotonView targetView = entity.GetComponent<PhotonView>();
        PhotonPlayer target = targetView.owner;
        if (entity.team != attacker.GetTeam()) {
            if (entity.GetComponent<PlayerChampion>() != null)
                targetView.RPC("Damage", PhotonTargets.AllBuffered, damageBase, attackerIdBase);
            else if (entity.GetComponent<Minion>() != null)
                targetView.RPC("EntityDamage", PhotonTargets.AllBuffered, damageBase, attackerIdBase);
        }
    }

    IEnumerator Delete() {
        yield return new WaitForSeconds(1.5f);
        PhotonNetwork.Destroy(gameObject);
    }
}