using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Turret : MonoBehaviour {

    public float baseHealth;
    public float baseDamage;
    public Image healthImage;
    public float incDamage;
    public PunTeams.Team team;
    public GameObject bulletPrefab;
    public float radius;
    public GameObject radiusTrigger;

    List<PlayerChampion> enemies;
    float currentDamage;
    PlayerChampion currentTarget;
    PhotonView photonView;
    bool started;
    float currentHealth;

    void Start() {
        photonView = GetComponent<PhotonView>();
        ResetDamage();
        enemies = new List<PlayerChampion>();
        radiusTrigger.transform.localScale = new Vector3(radius, radiusTrigger.transform.localScale.y, radius);
        currentHealth = baseHealth;
        healthImage.fillAmount = 1;
    }

    public void EnemyEnterRadius(PlayerChampion enemy) {
        enemies.Add(enemy);
        currentTarget = enemies[0];
        if(!started && PhotonNetwork.isMasterClient) {
            started = true;
            StartCoroutine("TargetEnemies");
        }
    }

    [PunRPC]
    public void Damage(float amount) {
        currentHealth = Mathf.Max(0f, currentHealth - amount);
        healthImage.fillAmount = (currentHealth / baseHealth);
        if (currentHealth == 0f)
            Destroy(gameObject);
    }

    public void EnemyLeaveRadius(PlayerChampion enemy) {
        enemies.Remove(enemy);
        if (!enemies.Contains(currentTarget)) {
            ResetDamage();
            currentTarget = null;
            if(enemies.Count > 0) {
                currentTarget = enemies[0];
            } else {
                StopCoroutine("TargetEnemies");
                started = false;
            }
        }
    }

    void ResetDamage() {
        currentDamage = baseDamage;
    }

    IEnumerator TargetEnemies() {
        while(true) {
            if(currentTarget != null) {
                photonView.RPC("Shoot", PhotonTargets.All, 100f, currentDamage, currentTarget.GetComponent<PhotonView>().viewID, photonView.owner);
                currentDamage += baseDamage;
            }
            yield return new WaitForSeconds(0.833f);
        }
    }

    [PunRPC]
    void Shoot(float speed, float damage, int photonId, PhotonPlayer shooter) {
        GameObject bullet = Instantiate(bulletPrefab, (transform.position + transform.forward), transform.rotation);
        Bullet b = bullet.GetComponent<Bullet>();
        b.Setup(damage, transform.position, photonId, shooter);
    }
}
