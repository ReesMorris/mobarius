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

    [Header("Health")]
    public bool regeneratesHealth;
    public float healthRegenAmount;
    public float healthRegenDelay;

    [Header("Conditionals")]
    public List<Turret> prerequisites;

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
    float currentHealth;
    bool started;
    float maxRegenHealth;

    void Start() {
        GameHandler.onGameStart += OnGameStart;
        photonView = GetComponent<PhotonView>();
        ResetDamage();
        enemies = new List<PlayerChampion>();
        radiusTrigger.transform.localScale = new Vector3(radius, radiusTrigger.transform.localScale.y, radius);
        currentHealth = maxRegenHealth = baseHealth;
        healthImage.fillAmount = 1;
    }

    void OnGameStart() {
        // Set health bar colours
        healthImage.color = GameUIHandler.Instance.enemyHealthColour;
        if (PhotonNetwork.player.GetTeam() == team)
            healthImage.color = GameUIHandler.Instance.allyHealthColour;

        // Set master client things
        if(PhotonNetwork.isMasterClient) {
            if(regeneratesHealth)
                StartCoroutine("RegenerateHealth");
        }
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

    IEnumerator RegenerateHealth() {
        while(currentHealth > 0f) {
            float percentageHealth = (currentHealth / baseHealth) * 100;
            if (percentageHealth < 33.3f)
                maxRegenHealth = (baseHealth * 0.3f);
            if (percentageHealth < 66.6f)
                maxRegenHealth = (baseHealth * 0.6f);
            else
                maxRegenHealth = baseHealth;

            photonView.RPC("Heal", PhotonTargets.All, healthRegenAmount);
            yield return new WaitForSeconds(healthRegenDelay);
        }
    }

    // A turret is invincible if it has prerequisites which are alive
    void CheckInvincibility() {
        if (gameObject.layer == LayerMask.NameToLayer("Default")) {
            if(prerequisites.Count > 0) {
                foreach(Turret t in prerequisites) {
                    if(t == null)
                        prerequisites.Remove(t);
                    else if (t.currentHealth <= 0f)
                        prerequisites.Remove(t);
                }
            }
            if (prerequisites.Count == 0)
                gameObject.layer = LayerMask.NameToLayer("Targetable");
            healthbarUI.SetActive(prerequisites.Count == 0);
        }
    }

    [PunRPC]
    public void Heal(float amount) {
        currentHealth = Mathf.Min(maxRegenHealth, currentHealth + amount);
        healthImage.fillAmount = (currentHealth / baseHealth);
    }

    [PunRPC]
    public void Damage(float amount, PhotonPlayer shooter) {
        currentHealth = Mathf.Max(0f, currentHealth - amount);
        healthImage.fillAmount = (currentHealth / baseHealth);
        if (currentHealth == 0f) {
            if (PhotonNetwork.player.GetTeam() == team)
                GameUIHandler.Instance.KillMessage("Announcer/AllyTurretDestroyed", "Ally turret destroyed!");
            else
                GameUIHandler.Instance.KillMessage("Announcer/EnemyTurretDestroyed", "Enemy turret destroyed!");
            Destroy(gameObject);
        }
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
        while(currentHealth > 0f) {
            if(currentTarget != null) {
                if(currentTarget.IsDead) {
                    EnemyLeaveRadius(currentTarget);
                } else {
                    photonView.RPC("Shoot", PhotonTargets.All, 100f, bulletSpawn.position, currentDamage, currentTarget.GetComponent<PhotonView>().viewID, null);
                    currentDamage += baseDamage;
                }
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
