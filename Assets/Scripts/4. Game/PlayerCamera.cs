﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
    This script contains functions for how the player camera works
*/
/// <summary>
/// This script contains functions for how the player camera works.
/// </summary>
public class PlayerCamera : MonoBehaviour {

    // Public variables
    public Transform target;
    public Vector3 distance;
    public Vector3 rotation;
    public float scrollSensitivity;
    public Vector2 minMaxFOV;
    public enum CameraDisplays { TopDown, ThirdPerson };

    // Private variables
    PhotonView photonView;
    float padding = 0.95f;
    float cameraSpeed = 0.17f;
    bool lockedToPlayer;
    MapProperties mapProperties;

    Vector3 gameOverTarget = Vector3.zero;
    float smoothTime;
    Vector3 velocity = Vector3.zero;

    // Necessary for playing another game after the first (otherwise Start does not get called again) Source: https://answers.unity.com/questions/762811/start-method-is-not-being-called-every-time-game-o.html [Accessed 15 April 2019]
    void OnEnable() {
        Start();
    }

    // Set up initial variables when the game starts
    void Start() {
        lockedToPlayer = true;
        photonView = target.GetComponent<PhotonView>();

        // Map Properties to determine whether this is third person or top-down
        mapProperties = MapManager.Instance.GetMapProperties();
        if (mapProperties.display == CameraDisplays.ThirdPerson) {
            transform.parent = target;
            transform.localPosition = new Vector3(0, 0, 0);
            transform.localEulerAngles = new Vector3(0, -90, 0);
        }
    }

    // Move the camera every frame in relation to the display
	void Update () {
        if (photonView.isMine) {

            // If a nexus has been destroyed, pan over to that instead
            if (gameOverTarget != Vector3.zero) {
                transform.parent = null;
                transform.position = Vector3.SmoothDamp(transform.position, gameOverTarget, ref velocity, smoothTime); // Source: https://docs.unity3d.com/ScriptReference/Vector3.SmoothDamp.html [10 February 2019]
            }
            
            // If the game is still playing...
            else {
                SetFOV();
                if (mapProperties.display == CameraDisplays.TopDown) {
                    CheckForInput();
                    CheckForMouseOnCorner();
                    if (lockedToPlayer)
                        CenterCameraTopDown();
                } else if(mapProperties.display == CameraDisplays.ThirdPerson) {
                    CenterCameraThirdPerson();
                }
            }
        }
	}

    // Recenter the camera on the player if input pressed
    void CheckForInput() {
        if (!ChatHandler.Instance.inputField.gameObject.activeSelf) {
            if (Input.GetKey(KeyCode.Space))
                CenterCameraTopDown();
            else if (Input.GetKeyDown(KeyCode.Y))
                lockedToPlayer = !lockedToPlayer;
        }
    }

    /// <summary>
    /// Moves the camera to re-center itself on the local player character.
    /// <param name="lockOn">If true, will remain locked on the player even when the button is released</param>
    /// </summary>
    public void FocusOnPlayer(bool lockOn) {
        if(lockOn)
            lockedToPlayer = true;
        CenterCameraTopDown();
    }

    // Set the field of view (Src: https://answers.unity.com/questions/218347/how-do-i-make-the-camera-zoom-in-and-out-with-the.html)
    void SetFOV() {
        float fov = Camera.main.fieldOfView;
        fov += Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity;
        fov = Mathf.Clamp(fov, minMaxFOV.x, minMaxFOV.y);
        Camera.main.fieldOfView = fov;
    }

    // Center the camera on the player
    void CenterCameraTopDown() {
        if (target != null) {
            transform.localPosition = target.position + distance;
            transform.localEulerAngles = rotation;
        }
    }

    // Center the camera on the player
    void CenterCameraThirdPerson() {
        transform.localPosition = new Vector3(0, 2.85f, -3.14f);
        transform.localEulerAngles = new Vector3(10, 0, 0);
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

    /// <summary>
    /// Sets the end of game target (destroyed nexus), removing control from the player.
    /// </summary>
    public void SetEndOfGameTarget(Vector3 target, float smoothingTime) {
        gameOverTarget = target;
        smoothTime = smoothingTime;
    }

    /// <summary>
    /// Clears the end of game target (nexus) so the camera will focus on the player character.
    /// </summary>
    public void ClearEndOfGameTarget() {
        gameOverTarget = Vector3.zero;
        this.enabled = false;
    }
}
