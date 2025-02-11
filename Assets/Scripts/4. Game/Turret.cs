﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Turret : MonoBehaviour {

    // Delegates
    public delegate void OnTurretDestroyed(Turret turret);
    public static OnTurretDestroyed onTurretDestroyed;

    // Public variables
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

    // Private variables
    List<Entity> enemies;
    float currentDamage;
    Entity currentTarget;
    PhotonView photonView;
    public float CurrentHealth { get; protected set; }
    bool started;
    float maxRegenHealth;
    bool ready;

    // Set up variables when the tower is spawned.
    void Start() {
        photonView = GetComponent<PhotonView>();
        ResetDamage();
        enemies = new List<Entity>();
        radiusTrigger.transform.localScale = new Vector3(radius, radiusTrigger.transform.localScale.y, radius);
        CurrentHealth = maxRegenHealth = baseHealth;
        healthImage.fillAmount = 1;
        GetComponent<Entity>().team = team;
    }

    // Update tower health bar colour and start some scripts when the game is actually started.
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

    /// <summary>
    /// Adds the enemy to a list of enemies for the turret to shoot at.
    /// <param name="enemy">The entity who entered the radius</param>
    /// </summary>
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

    // Wait for the tower to be ready first
    void Update() {
        if (healthImage != null && !ready)
            OnGameStart();
        CheckInvincibility();
    }

    // Master client will regenerate the tower health over the network
    IEnumerator RegenerateHealth() {
        while(CurrentHealth > 0f) {

            // Algorithm for regeneration amount
            float percentageHealth = (CurrentHealth / baseHealth) * 100;
            if (percentageHealth < 33.3f)
                maxRegenHealth = (baseHealth * 0.3f);
            if (percentageHealth < 66.6f)
                maxRegenHealth = (baseHealth * 0.6f);
            else
                maxRegenHealth = baseHealth;

            // Network heal request
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

    /// <summary>
    /// Applies health to the building.
    /// </summary>
    /// <param name="amount">The amount of health to regenerate</param>
    [PunRPC]
    public void Heal(float amount) {
        CurrentHealth = Mathf.Min(maxRegenHealth, CurrentHealth + amount);
        healthImage.fillAmount = (CurrentHealth / baseHealth);
    }

    /// <summary>
    /// Applies damage to the building.
    /// </summary>
    /// <param name="amount">The amount of damage to take</param>
    /// <param name="shooterId">The viewID of the attacker</param>
    [PunRPC]
    public void Damage(float amount, int shooterId) {
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

    /// <summary>
    /// Removes the enemy from the list of enemies within range.
    /// <param name="enemy">The entity who exited the radius</param>
    /// </summary>
    public void EnemyLeaveRadius(Entity enemy) {
        enemies.Remove(enemy);
        if (!enemies.Contains(currentTarget)) {
            ResetDamage();
            currentTarget = null;
            if(enemies.Count > 0) {
                // Smarter prioritising for which enemy to target; players will be targeted last
                foreach(Entity e in enemies) {
                    if (e == null)
                        continue;
                    PlayerChampion c = e.GetComponent<PlayerChampion>();
                    if(c == null)
                        currentTarget = e;
                }
                if (currentTarget == null)
                    currentTarget = enemies[0];
            } else {
                StopCoroutine("TargetEnemies");
                started = false;
            }
        }
    }

    // Prevent incremental base damage on a new enemy
    void ResetDamage() {
        currentDamage = baseDamage;
    }

    // Find new enemies to target and attack if one exists
    IEnumerator TargetEnemies() {
        while(CurrentHealth > 0f) {
            if(currentTarget == null || currentTarget.GetIsDead())
                EnemyLeaveRadius(currentTarget);
            else {
                photonView.RPC("Shoot", PhotonTargets.All, 100f, bulletSpawn.position, currentDamage, currentTarget.GetComponent<PhotonView>().viewID, photonView.viewID);
                currentDamage += baseDamage;
            }
            yield return new WaitForSeconds(0.833f);
        }
    }

    // Shoot the bullet across the network
    [PunRPC]
    void Shoot(float speed, Vector3 spawnPos, float damage, int photonId, int shooter) {
        GameObject bullet = Instantiate(bulletPrefab, spawnPos, transform.rotation);
        Bullet b = bullet.GetComponent<Bullet>();
        b.Setup(damage, team, transform.position, photonId, shooter);
    }
}
