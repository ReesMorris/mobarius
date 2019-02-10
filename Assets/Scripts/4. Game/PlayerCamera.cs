using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {

    public Transform target;
    public Vector3 distance;
    public Vector3 rotation;
    public float scrollSensitivity;
    public Vector2 minMaxFOV;

    PhotonView photonView;
    float padding = 0.95f;
    float cameraSpeed = 0.17f;
    bool lockedToPlayer;

    Vector3 gameOverTarget = Vector3.zero;
    float smoothTime = 1F;
    Vector3 velocity = Vector3.zero;

    void Start() {
        lockedToPlayer = true;
        photonView = target.GetComponent<PhotonView>();
    }

	void Update () {
        if (photonView.isMine) {
            if (gameOverTarget != Vector3.zero) {
                transform.position = Vector3.SmoothDamp(transform.position, gameOverTarget, ref velocity, smoothTime); // Source: https://docs.unity3d.com/ScriptReference/Vector3.SmoothDamp.html [10 February 2019]
            } else {
                SetFOV();
                CheckForInput();
                CheckForMouseOnCorner();
                if (lockedToPlayer)
                    CenterCamera();
            }
        }
	}

    // Recenter the camera on the player if input pressed
    void CheckForInput() {
        if(Input.GetKey(KeyCode.Space)) {
            CenterCamera();
        }
        if(Input.GetKeyDown(KeyCode.Y)) {
            lockedToPlayer = !lockedToPlayer;
        }
    }

    // Lock the camera to the player
    public void FocusOnPlayer(bool lockOn) {
        if(lockOn)
            lockedToPlayer = true;
        CenterCamera();
    }

    // Set the field of view (Src: https://answers.unity.com/questions/218347/how-do-i-make-the-camera-zoom-in-and-out-with-the.html)
    void SetFOV() {
        float fov = Camera.main.fieldOfView;
        fov += Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity;
        fov = Mathf.Clamp(fov, minMaxFOV.x, minMaxFOV.y);
        Camera.main.fieldOfView = fov;
    }

    // Center the camera on the player
    void CenterCamera() {
        transform.localPosition = target.position + distance;
        transform.localEulerAngles = rotation;
    }

    // Check to see if the player is moving the screen
    void CheckForMouseOnCorner() {
        if(Application.isFocused && !lockedToPlayer) {
            if (Input.mousePosition.y >= Screen.height * padding) {
                lockedToPlayer = false;
                transform.position += (Vector3.forward * cameraSpeed);
            }
            if(Input.mousePosition.y <= padding) {
                lockedToPlayer = false;
                transform.position += (Vector3.back * cameraSpeed);
            }
            if(Input.mousePosition.x >= Screen.width * padding) {
                lockedToPlayer = false;
                transform.position += (Vector3.right * cameraSpeed);
            }
            if(Input.mousePosition.x <= padding) {
                lockedToPlayer = false;
                transform.position += (Vector3.left * cameraSpeed);
            }
        }
    }

    // End of game target
    public void SetEndOfGameTarget(Vector3 target) {
        gameOverTarget = target;
    }
}
