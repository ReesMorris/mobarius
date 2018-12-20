using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarenQ : MonoBehaviour {

    public float range;
    public float cooldown;
    public float speed;
    public float damage;
    public GameObject bulletPrefab;
    public GameObject projectileIndicatorPrefab;
    PlayerChampion playerChampion;
    PhotonView photonView;
    GameObject preview;
    Vector3 direction;

    bool preparing;

    void Start() {
        playerChampion = GetComponent<PlayerChampion>();
        photonView = GetComponent<PhotonView>();
        preview = Instantiate(projectileIndicatorPrefab, transform.position, Quaternion.identity);
        preview.transform.parent = gameObject.transform;
        preview.transform.localPosition = Vector3.zero;
        preview.SetActive(false);
    }

    void Update() {
        if (photonView.isMine) {
            if (!playerChampion.IsDead) {
                if (preview.activeSelf) {
                    preview.transform.LookAt(direction);
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 100)) {
                        direction = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                    }
                }
                if (GameUIHandler.Instance.CanCastAbility(GameUIHandler.Abilities.Q)) {
                    if (Input.GetKeyDown(KeyCode.Q)) {
                        preparing = true;
                        preview.transform.localScale = new Vector3(preview.transform.localScale.x, preview.transform.localScale.y, range * 20f);
                        preview.SetActive(true);
                    }
                    if (Input.GetKeyUp(KeyCode.Q)) {
                        preparing = false;
                        preview.SetActive(false);
                    }
                    if(Input.GetMouseButtonDown(0)) {
                        if (preparing) {
                            preparing = false;
                            preview.SetActive(false);
                            print("fire!");
                            transform.LookAt(direction);
                            photonView.RPC("Shoot", PhotonTargets.All, new object[] { speed, damage });
                            GameUIHandler.Instance.OnAbilityCasted(GameUIHandler.Abilities.Q, cooldown);
                        }
                    }
                }
            } else {
                print("Cannot fire yet");
            }
        }
    }

    [PunRPC]
    void Shoot(float speed, float damage) {
        GameObject bullet = Instantiate(bulletPrefab, (transform.position + transform.forward), transform.rotation);
        Bullet b = bullet.GetComponent<Bullet>();
        b.Setup(damage, transform.position, range);
        bullet.GetComponent<Rigidbody>().velocity = transform.forward * speed;
    }
}
