using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private InputManager inputManager;  // Referencing InputManager

    public Transform targetTransform;   // the object the camera will follow : Player
    public Transform cameraPivot;       // the object that the camera uses to pivot
    private Vector3 cameraFollowVelocity = Vector3.zero;

    public float cameraFollowSpeed = 0.2f;
    public float cameraLookSpeed = 2;
    public float cameraPivotSpeed = 2;

    public float lookAngle; // the angle the camera looks up and down in
    public float pivotAngle;// the angle the camera looks right and left

    private void Awake()
    {
        // Set the InputManager
        inputManager = FindObjectOfType<InputManager>();
        // Set the target transform to Player's transform
        targetTransform = FindObjectOfType<PlayerManager>().transform;
    }

    // a function to encapsulate all functions related with Camera movement 
    public void HandleAllCameraMovement()
    {
        FollowTarget();
        RotateCamera();
    }

    // Have the camera follow Player
    private void FollowTarget()
    {
        // Calculate the target position that camera will follow smoothly
        Vector3 targetPosition =
            Vector3.SmoothDamp(transform.position, targetTransform.position, ref cameraFollowVelocity,cameraFollowSpeed);

        // Update the camera's position
        transform.position = targetPosition;
    }

    // Have Camera rotate along with Player
    private void RotateCamera()
    {
        // Caculate the vertical angle for the camera to follow
        lookAngle += (inputManager.horizontalCameraInput * cameraLookSpeed);
        // Caculate the horizontal angle for the camera to follow
        pivotAngle -= (inputManager.verticalCameraInput * cameraPivotSpeed);

        // the default rotation of the camera
        Vector3 rotation = Vector3.zero;

        rotation.y = lookAngle;
        Quaternion targetRotation = Quaternion.Euler(rotation); // Convert the euler angle to Quaternion
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;
    }
}
