using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    InputManager inputManager;

    Vector3 moveDirection;  // holding in which direction Player goes
    Transform cameraObject; // the main camera is required since player's movement direction is based on where User sees through the camera

    Rigidbody playerRigidbody;

    public bool isSprinting;    // to tell whether Player is sprinting or not

    [Header("Movement Speed")]
    // Speeds of each locomotion
    public float walkingSpeed = 1.5f;
    public float runningSpeed = 5;
    public float sprintingSpeed = 7;
    public float rotaionSpeed = 15;

    private void Awake()
    {
        // Get references on the required components on Player
        inputManager = GetComponent<InputManager>();
        playerRigidbody = GetComponent<Rigidbody>();

        // Get a reference on the main camera
        cameraObject = Camera.main.transform;
    }

    // a function to encapsulate all functions related with Player movement 
    public void HandleAllMovement()
    {
        HandleMovement();
        HandleRotation();
    }

    /// <summary>
    /// a function that handles Player's movements
    /// </summary>
    private void HandleMovement()
    {
        // Calculate the movement based on the inputs along with each axis
        moveDirection = cameraObject.forward * inputManager.verticalInput;  // Player's movement direction along with the vertical input. // Transform.forward is the direction the transform is looking at
        moveDirection += cameraObject.right * inputManager.horizontalInput; // Player's movement direction along with the horizontal input
                                                                            // The main direction is forward, so the input from the x-axis should be added or the input processed later will occupy ignoring the first one
                                                                            // Or, Bring the horizontal input part before and put =
        // Normalize the movement direction
        moveDirection.Normalize();
        // In order to prevent Player from moving towards the sky
        moveDirection.y = 0;

        // Apply speed for each state;
        // when sprinting
        if (isSprinting)
        {
            moveDirection *= sprintingSpeed;
        }

        // when not sprinting
        else
        {
            // when running
            if (inputManager.moveAmount >= 0.5f)
                moveDirection *= runningSpeed;
            // when walking
            else
                moveDirection *= walkingSpeed;
        }


        // Apply the process result
        Vector3 movementVelocity = moveDirection;   // Just to make the codes tidy
        playerRigidbody.velocity = movementVelocity;// Change the posiiton of Player's Rigidody along with the velocity
    }

    /// <summary>
    /// a function that handles Player's rotation
    /// </summary>
    private void HandleRotation()
    {
        // Initialize the value of rotation
        Vector3 targetDirection = Vector3.zero;

        // Calculate the target direction based on the inputs along with each axis
        targetDirection = cameraObject.forward * inputManager.verticalInput;
        targetDirection += cameraObject.right * inputManager.horizontalInput;
        // Normalize the movement direction
        targetDirection.Normalize();
        // In order to prevent Player from rotating towards the sky
        targetDirection.y = 0;

        // To prevent Player from snapping to forward direction when there is no input
        if(targetDirection == Vector3.zero)
        {
            targetDirection = transform.forward;    // Transform.forward is the direction Player is looking at
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);   // Transform a rotation in Vector3 to a rotation in Quaternion, setting it as a forward direction
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotaionSpeed * Time.deltaTime);    // Process the rotation with Slerp()

        // Apply the rotation
        transform.rotation = playerRotation;
    }
}
