using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    // Components on Player
    public Animator animator;
    public PlayerManager playerManager;
    public PlayerLocomotion playerLocomotion;

    // IDs of the parameters in the Animator to reference them in the script
    int horizontal;
    int vertical;

    private void Awake()
    {
        // Referencing components on Player
        animator = GetComponent<Animator>();
        playerManager = GetComponent<PlayerManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();

        // Reference the parameters in the Animator with IDs of Int
        horizontal = Animator.StringToHash("Horizontal");   // the name of the parameters shoudl be exact
        vertical = Animator.StringToHash("Vertical");       // With the IDs, the parameters can be accessed and modified in the script
    }

    // Play a target animation overriding locomotion with the bool, locking Player into that animation
    public void PlayTargetAnimaiton(string targetAnimation, bool isInteracting, bool useRootMotion = false)
    {
        // Change the bool parameter on the animator to decide whether to override locomotion or not
        animator.SetBool("isInteracting", isInteracting);

        // Change the bool parameter on the animator to decide whether a root motion is used
        animator.SetBool("isUsingRootMotion", useRootMotion);

        // Play the target animation crossfading from the previous animaiton
        animator.CrossFade(targetAnimation, 0.2f);
    }

    // Pass the movement input to the animator (Humanoid)
    public void UpdateAnimatorValues(float horizontalMovement, float verticalMovement, bool isSprinting)
    {
        // Animation Snapping by rounding the values
        // so that there is not subtly blended animations between walking and running
        float snappedHorizontal;
        float snappedVertical;

        #region Horizontal Snapping
        if (horizontalMovement > 0 && horizontalMovement < 0.55f)   // snapped to walking
            snappedHorizontal = 0.5f;
        else if (horizontalMovement > 0.55f)                        // snapped to running
            snappedHorizontal = 1;
        else if (horizontalMovement < 0 && horizontalMovement > -0.55f) // snapped to walking backwards(TODO later)
            snappedHorizontal = -0.5f;
        else if (horizontalMovement < -0.55f)                       // snapped to running backwards(TODO later)
            snappedHorizontal = -1;
        else
            snappedHorizontal = 0;                                  // snapped to idling
        #endregion
        #region Vertical Snapping
        if (verticalMovement > 0 && verticalMovement < 0.55f)   // snapped to walking
            snappedVertical = 0.5f;
        else if (verticalMovement > 0.55f)                      // snapped to running
            snappedVertical = 1;
        else if (verticalMovement < 0 && verticalMovement > -0.55f) // snapped to walking backwards(TODO later)
            snappedVertical = -0.5f;
        else if (verticalMovement < -0.55f)                     // snapped to running backwards(TODO later)
            snappedVertical = -1;
        else
            snappedVertical = 0;                                // snapped to idling
        #endregion

        // when sprinting
        if(isSprinting)
        {
            snappedHorizontal = horizontalMovement; // allowing the blending
            snappedVertical = 2;                    // Fix the vertical value to 2;
        }

        // Change the Horizontal/Vertical parameters on the animator by accessing them with their IDs
        animator.SetFloat(horizontal, snappedHorizontal, 0.1f, Time.deltaTime); // the Damp time is time for blending, or time to take to have a result of blending animations
        animator.SetFloat(vertical, snappedVertical, 0.1f, Time.deltaTime);
    }

    private void OnAnimatorMove()   // called in every frame while an animation is playing
    {
        if(playerManager.isUsingRootMotion) // when using a root motion
        {
            playerLocomotion.playerRigidbody.drag = 0;  // Reset the drag
            Vector3 deltaPositon = animator.deltaPosition;  // the position of Player during the animation
            deltaPositon.y = 0; // To prevent Player from moving in the air
            Vector3 velocity = deltaPositon / Time.deltaTime;   // the velocity Player is moving with the animation in

            // Move Player along with the animation of Root Motion
            playerLocomotion.playerRigidbody.velocity = velocity;
        }
    }
}
