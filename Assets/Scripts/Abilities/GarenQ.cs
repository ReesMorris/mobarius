using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarenQ : MonoBehaviour {

    public float range;
    public float cooldown;
    public float speed;
    public float damage;
    public GameObject bulletPrefab;
    PlayerChampion playerChampion;
    PhotonView photonView;

    GameObject indicator;
    AbilityHandler abilityHandler;

    bool preparing;

    void Start() {
        playerChampion = GetComponent<PlayerChampion>();
        photonView = GetComponent<PhotonView>();
        abilityHandler = AbilityHandler.Instance;
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
                if (GameUIHandler.Instance.CanCastAbility(AbilityHandler.Abilities.Q)) {

                    // Is Q being pressed down?
                    if (Input.GetKeyDown(KeyCode.Q))
                        abilityHandler.StartCasting(indicator, range);

                    // Is Q being released?
                    if (Input.GetKeyUp(KeyCode.Q))
                        abilityHandler.StopCasting(indicator);

                    // Are we trying to fire?
                    if(Input.GetMouseButtonDown(0)) {
                        // Are we aiming? This ability requires aiming
                        if (abilityHandler.Aiming) {
                            // Tell the AbilityHandler that we've used this ability
                            abilityHandler.OnAbilityCast(gameObject, indicator, AbilityHandler.Abilities.Q, cooldown, true);

                            // Handle the actual unique part of this ability
                            photonView.RPC("Shoot", PhotonTargets.All, new object[] { speed, damage });
                        }
                    }
                }
            }
        }
    }

    // The actual unique part of this ability
    [PunRPC]
    void Shoot(float speed, float damage) {
        GameObject bullet = Instantiate(bulletPrefab, (transform.position + transform.forward), transform.rotation);
        Bullet b = bullet.GetComponent<Bullet>();
        b.Setup(damage, transform.position, range);
        bullet.GetComponent<Rigidbody>().velocity = transform.forward * speed;
    }
}
