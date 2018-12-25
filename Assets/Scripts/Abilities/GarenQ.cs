using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarenQ : MonoBehaviour {

    public AbilityHandler.Abilities abilityType;
    public GameObject bulletPrefab;
    PlayerChampion playerChampion;
    PhotonView photonView;

    GameObject indicator;
    AbilityHandler abilityHandler;
    Ability ability;
    bool casting;

    void Start() {
        playerChampion = GetComponent<PlayerChampion>();
        photonView = GetComponent<PhotonView>();
        abilityHandler = AbilityHandler.Instance;
        ability = abilityHandler.GetChampionAbilities(playerChampion.Champion.championName, abilityType);
        indicator = abilityHandler.SetupProjectileIndicator(gameObject);
    }

    void Update() {
        // Are we the player doing this?
        if (photonView.isMine) {
            // Are we alive?
            if (!playerChampion.IsDead) {
                // If the indicator is visible, update its rotation
                if (indicator.activeSelf)
                    abilityHandler.UpdateIndicatorRotation(indicator, gameObject);

                // Can we cast this ability?
                if (GameUIHandler.Instance.CanCastAbility(abilityType, ability, playerChampion.Champion)) {
                    // Is Q being pressed down?
                    if (Input.GetKeyDown(KeyCode.Q)) {
                        if(!abilityHandler.Aiming)
                            abilityHandler.StartCasting(indicator, ability.range);
                        else
                            abilityHandler.StopCasting(indicator);
                    }

                    // Are we trying to fire?
                    if(Input.GetMouseButtonDown(0)) {
                        // Are we aiming? This ability requires aiming
                        if (abilityHandler.Aiming) {
                            // Tell the AbilityHandler that we've used this ability
                            abilityHandler.OnAbilityCast(gameObject, indicator, abilityType, ability.cooldown, true);

                            // Handle the actual unique part of this ability
                            photonView.RPC("Shoot", PhotonTargets.All, ability.speed, abilityHandler.GetDamageFromAbility(ability, "bullet"), abilityHandler.GetDirection(gameObject), photonView.owner);

                            // Take mana from the player
                            playerChampion.PhotonView.RPC("TakeMana", PhotonTargets.All, ability.cost);
                        }
                    }
                }
            }
        }
    }

    // The actual unique part of this ability
    [PunRPC]
    void Shoot(float speed, float damage, Vector3 lookAt, PhotonPlayer shooter) {
        transform.LookAt(lookAt);
        GameObject bullet = Instantiate(bulletPrefab, (transform.position + transform.forward), transform.rotation);
        Bullet b = bullet.GetComponent<Bullet>();
        b.Setup(damage, transform.position, ability.range, shooter);
        bullet.GetComponent<Rigidbody>().velocity = transform.forward * speed;
    }
}
