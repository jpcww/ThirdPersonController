using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    // References on Player    
    InputManager inputManager;
    PlayerManager playerManager;
    AnimatorManager animatorManager;
    public Rigidbody playerRigidbody;

    // References out of Player
    Vector3 moveDirection;  // holding in which direction Player goes
    Transform cameraObject; // the main camera is required since player's movement direction is based on where User sees through the camera

    [Header("Movement Flags")]
    public bool isSprinting;    // to tell whether Player is sprinting or not
    public bool isGrounded;     // to tell whether Player is on the ground or not
    public bool isJumping;      // to tell whether Player is jumping

    [Header("Movement Speed")]
    // Speeds of each locomotion
    public float walkingSpeed = 1.5f;
    public float runningSpeed = 5;
    public float sprintingSpeed = 7;
    public float rotatingSpeed = 15;

    [Header("Falling")]
    public float inAirTimer;
    public float leapingVelocity;
    public float fallingVelocity;
    public float rayCastHeightOffSet = 0.5f;    // a value to have the detecting the ground start from above Player's feet not right botton of them
    public LayerMask groundLayer;

    [Header("Jumping")]
    public float jumpHeight = 3;
    public float gravityIntensity = -15;


    private void Awake()
    {
        // Get references on the required components on Player
        inputManager = GetComponent<InputManager>();
        playerRigidbody = GetComponent<Rigidbody>();
        playerManager = GetComponent<PlayerManager>();
        animatorManager = GetComponent<AnimatorManager>();

        // Get a reference on the main camera
        cameraObject = Camera.main.transform;
    }

    // a function to encapsulate all functions related with Player movement 
    public void HandleAllMovement()
    {
        HandleFallingAndLanding();  // To make sure to handle falling and landing first : when falling, Player must fall

        // Stop Player from moving/rotating while performing actions such as falling, landing, or attacking etc
        if (playerManager.isInteracting)
            return;

        // Stop Player from moving/rotating while jumping
        if (isJumping)
            return;

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
        Vector3 movementVelocity = moveDirection;   // Just to make the codes tidy, and can be used somewhere else
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
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotatingSpeed * Time.deltaTime);    // Process the rotation with Slerp()

        // Apply the rotation
        transform.rotation = playerRotation;
    }

    private void HandleFallingAndLanding()
    {
        RaycastHit hit;
        Vector3 rayCastOrigin = transform.position;

        rayCastOrigin.y = rayCastOrigin.y + rayCastHeightOffSet;    // Set offset in the raycast upwards from Player's feet

        // for stairs and slopes
        Vector3 targetPosition;
        targetPosition = transform.position;

        // when not grounded and not jumping : while jumping, falling animation won't play
        if (!isGrounded && !isJumping)
        {
            // when Player is not performing any actions
            if (!playerManager.isInteracting)
            {
                // Play the animation of falling, setting the flag to override Locomotion
                animatorManager.PlayTargetAnimaiton("Fall", true);
            }

            animatorManager.animator.SetBool("isUsingRootMotion", false);   // off the root motion when falling after dodging

            inAirTimer += Time.deltaTime;   // a value that increases while Player is in the air : like Gravity
            playerRigidbody.AddForce(transform.forward * leapingVelocity);   // Add force of leaping to Player 
            playerRigidbody.AddForce(-Vector3.up * fallingVelocity * inAirTimer);  // Add force of falling to Player. The longer in the air, the quicker Player falls
        }


        // when the ground is detected before landing
        if (Physics.SphereCast(rayCastOrigin, 0.2f, -Vector3.up, out hit, groundLayer))
        {
            //Debug.Log("ground detected");
            //Debug.Log("isGrounded : " + isGrounded);
            //Debug.Log("isInteracting : " + playerManager.isInteracting);
            // this part is not necessary for now
            // without this, landing animation is played due to the animator 
            // if not yet touched the ground (for now "isInteracting" does not do anything)
            if (!isGrounded && !playerManager.isInteracting)
            {
                // Play the animation of landing, setting the flag to override Locomotion
                animatorManager.PlayTargetAnimaiton("Land", true);
                Debug.Log("land animation");
            }

            // for stairs and slopes
            Vector3 rayCastHitPoint = hit.point;    // the postion of the detected ground
            targetPosition.y = rayCastHitPoint.y; // a new position for feet
            // Reset the associated flags, "isInterating" is reset in "ResetBool.cs"
            inAirTimer = 0;
            isGrounded = true;
        }

        // while still falling
        else
        {
            isGrounded = false;
        }

        // on the ground, not jumping
        if (isGrounded && !isJumping)
        {
            if (playerManager.isInteracting || inputManager.moveAmount > 0) // performing an action or moving : climbing stairs or slopes
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / 0.1f);   // push Player along with the height of stairs/slopes
            }

            else
            {
                // when ideling
                transform.position = targetPosition;
            }
        }
    }

    public void HandleJumping()
    {
        // Jumping is allowed when Player is on the ground
        if(isGrounded)
        {
            // Set the flag on
            animatorManager.animator.SetBool("isJumping", true);
            // Play Jumping animation
            animatorManager.PlayTargetAnimaiton("Jump", false); // isInteracting should be false to allow movement while jumping

            float jumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpHeight); // Calcuate a velocity for jumping
            Vector3 playerVeleocity = moveDirection;    // if Player is running the running velocity has been applied in "moveDirection"                                            
            playerVeleocity.y = jumpingVelocity;        // Add the jumping velocity into Player's current velocity
            playerRigidbody.velocity = playerVeleocity; // Move Player according to the velocity
        }
    }

    public void HandleDodging()
    {
        if (playerManager.isInteracting)    // if Player is performing another action, doding is not allowed
            return;

        // Playe dodging animation
        animatorManager.PlayTargetAnimaiton("Dodge", true, true); // Set the flag to tell Player is performing an action

        // TODO : Toggle Invulnerable bool for no hp damage during dodging
    }
}
