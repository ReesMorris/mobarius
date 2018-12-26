using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Turret : MonoBehaviour {

    [Header("Damage")]
    public float baseHealth;
    public float baseDamage;
    public float incDamage;
    public PunTeams.Team team;
    public float radius;

    [Header("Conditionals")]
    public Turret prerequisite;

    [Header("UI & Objects")]
    public Transform bulletSpawn;
    public GameObject healthbarUI;
    public Image healthImage;
    public GameObject bulletPrefab;
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

    private void Update() {
        CheckInvincibility();
    }

    void CheckInvincibility() {
        if (gameObject.layer == LayerMask.NameToLayer("Default")) {
            if(prerequisite != null) {
                if(prerequisite.currentHealth <= 0f) {
                    prerequisite = null;
                }
            }

            if (prerequisite == null)
                gameObject.layer = LayerMask.NameToLayer("Targetable");

            healthbarUI.SetActive(prerequisite == null);
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
                photonView.RPC("Shoot", PhotonTargets.All, 100f, bulletSpawn.position, currentDamage, currentTarget.GetComponent<PhotonView>().viewID, null);
                currentDamage += baseDamage;
            }
            yield return new WaitForSeconds(0.833f);
        }
    }

    [PunRPC]
    void Shoot(float speed, Vector3 spawnPos, float damage, int photonId, PhotonPlayer shooter) {
        GameObject bullet = Instantiate(bulletPrefab, spawnPos, transform.rotation);
        Bullet b = bullet.GetComponent<Bullet>();
        b.Setup(damage, transform.position, photonId, shooter);
    }
}
