using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Turret : MonoBehaviour {

    [Header("Damage")]
    public float baseDamage;
    public float incDamage;
    public PunTeams.Team team;
    public float radius;

    [Header("Health")]
    public float baseHealth;
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

    [Header("XP")]
    public int XPOnDeath;

    List<Entity> enemies;
    float currentDamage;
    Entity currentTarget;
    PhotonView photonView;
    public float CurrentHealth { get; protected set; }
    bool started;
    float maxRegenHealth;
    bool ready;

    public delegate void OnTurretDestroyed(Turret turret);
    public static OnTurretDestroyed onTurretDestroyed;

    void Start() {
        photonView = GetComponent<PhotonView>();
        ResetDamage();
        enemies = new List<Entity>();
        radiusTrigger.transform.localScale = new Vector3(radius, radiusTrigger.transform.localScale.y, radius);
        CurrentHealth = maxRegenHealth = baseHealth;
        healthImage.fillAmount = 1;
        GetComponent<Entity>().team = team;
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
        ready = true;
    }

    public void EnemyEnterRadius(Entity enemy) {
        if(!enemy.GetComponent<Entity>().GetIsDead()) {
            enemies.Add(enemy);
            currentTarget = enemies[0];
            if(!started && PhotonNetwork.isMasterClient) {
                started = true;
                StartCoroutine("TargetEnemies");
            }
        }
    }

    void Update() {
        if (healthImage != null && !ready)
            OnGameStart();
        CheckInvincibility();
    }

    IEnumerator RegenerateHealth() {
        while(CurrentHealth > 0f) {
            float percentageHealth = (CurrentHealth / baseHealth) * 100;
            if (percentageHealth < 33.3f)
                maxRegenHealth = (baseHealth * 0.3f);
            if (percentageHealth < 66.6f)
                maxRegenHealth = (baseHealth * 0.6f);
            else
                maxRegenHealth = baseHealth;

            photonView.RPC("Heal", PhotonTargets.AllBuffered, healthRegenAmount);
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
                    else if (t.CurrentHealth <= 0f)
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
        CurrentHealth = Mathf.Min(maxRegenHealth, CurrentHealth + amount);
        healthImage.fillAmount = (CurrentHealth / baseHealth);
    }

    [PunRPC]
    public void Damage(float amount, PhotonPlayer shooter) {
        CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);
        healthImage.fillAmount = (CurrentHealth / baseHealth);
        if (CurrentHealth == 0f) {
            if (PhotonNetwork.player.GetTeam() == team)
                GameUIHandler.Instance.MessageWithSound("Announcer/AllyTurretDestroyed", "Ally turret destroyed!");
            else
                GameUIHandler.Instance.MessageWithSound("Announcer/EnemyTurretDestroyed", "Enemy turret destroyed!");

            // Tell other scripts that this turret has been destroyed
            if (onTurretDestroyed != null)
                onTurretDestroyed(this);
            Destroy(gameObject);
        }
    }

    public void EnemyLeaveRadius(Entity enemy) {
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
        while(CurrentHealth > 0f) {
            if(currentTarget == null || currentTarget.GetIsDead())
                EnemyLeaveRadius(currentTarget);
            else {
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
        b.Setup(damage, team, transform.position, photonId, shooter);
    }
}
