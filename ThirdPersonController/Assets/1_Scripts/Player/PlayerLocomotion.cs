using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    InputManager inputManager;

    Vector3 moveDirection;  // holding in which direction Player goes
    Transform cameraObject; // the main camera is required since player's movement direction is based on where User sees through the camera

    Rigidbody playerRigidbody;

    public float movementSpeed = 7;
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
        // Update the movement direction along with the input
        moveDirection = cameraObject.forward * inputManager.verticalInput;  // Player's movement direction along with the vertical input
        moveDirection += cameraObject.right * inputManager.horizontalInput; // Plaeyr's movement direction along with the horizontal input
                                                                            // The main direction is forward, so the input from the x-axis should be added unless, the player will look at either of right or left
        // Normalize the movement direction
        moveDirection.Normalize();
        // In order to prevent Player from moving towards the sky
        moveDirection.y = 0;
        // Apply the movement speed;
        moveDirection *= movementSpeed;

        Vector3 movementVelocity = moveDirection;   // Just to make the codes tidy
        // Change the posiiton of the player along with the velocity
        playerRigidbody.velocity = movementVelocity;
    }

    /// <summary>
    /// a function that handles Player's rotation
    /// </summary>
    private void HandleRotation()
    {
        // Initial value of the rotation
        Vector3 targetDirection = Vector3.zero;

        // Player's rotation along with each input axis
        targetDirection = cameraObject.forward * inputManager.verticalInput;
        targetDirection += cameraObject.right * inputManager.horizontalInput;
        // Normalize the movement direction
        targetDirection.Normalize();
        // In order to prevent Player from moving towards the sky
        targetDirection.y = 0;

        // To prevent Player from snapping to forward direction when there is no input
        if(targetDirection == Vector3.zero)
        {
            targetDirection = transform.forward;    // Transform.forward is the direction Player is looking at
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);   // Transform a rotation in Vector3 to a roation in Quaternion
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotaionSpeed * Time.deltaTime);    // Process the rotation with Slerp()

        // Apply the rotation
        transform.rotation = playerRotation;
    }
}
