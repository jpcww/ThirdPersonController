using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// a class where all functions made for Player are called
/// </summary>
public class PlayerManager : MonoBehaviour
{
    InputManager inputManager;
    CameraManager cameraManager;
    PlayerLocomotion playerLocomotion;

    private void Awake()
    {
        // Get references on the required components on Player
        inputManager = GetComponent<InputManager>();
        cameraManager = FindObjectOfType<CameraManager>(); // CameraManager is not attached to Player
        playerLocomotion = GetComponent<PlayerLocomotion>();
    }

    private void Update()
    {
        // Get input
        inputManager.HandleAllInputs();
    }

    private void FixedUpdate()  // FinxedUpdate() is supposed to be used when moving a Rigidbody or when applying physics based on the physics engine
    {
        // Move Player
        playerLocomotion.HandleAllMovement();
    }

    private void LateUpdate()   // LateUpdate() is used since the camera is not to follow Player right away
    {
        cameraManager.HandleAllCameraMovement(); 
    }
}
