using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// a class where all functions made for Player are called
/// </summary>
public class PlayerManager : MonoBehaviour
{
    // References on Player
    InputManager inputManager;
    PlayerLocomotion playerLocomotion;
    Animator animator;
    // References outside of Player
    CameraManager cameraManager;

    // Parameters on Animator
    public bool isInteracting;
    public bool isUsingRootMotion;

    private void Awake()
    {
        // Get references on the required components on Player
        inputManager = GetComponent<InputManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        animator = GetComponent<Animator>();
        // Get Reference outside of Player
        cameraManager = FindObjectOfType<CameraManager>(); // CameraManager is not attached to Player
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
        // Move the camera
        cameraManager.HandleAllCameraMovement();

        // Check the parameters in the animator
        isInteracting = animator.GetBool("isInteracting");
        isUsingRootMotion = animator.GetBool("isUsingRootMotion");
        playerLocomotion.isJumping = animator.GetBool("isJumping");

        // Update the parameters in the animator
        animator.SetBool("isGrounded", playerLocomotion.isGrounded);
    }
}
