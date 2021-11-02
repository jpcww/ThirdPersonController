using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private InputManager inputManager;  // Referencing InputManager

    public Transform targetTransform;   // the object the camera will follow : Player
    public Transform cameraPivot;       // the object that the camera uses to pivot
    private Transform cameraTransform;   // the tranform of the actual camera object in the scene
    private float defaultPosition;      // Camera's Z position : the depth of camera from Player
    private Vector3 cameraVectorPosition;

    private Vector3 cameraFollowVelocity = Vector3.zero; // the current velocity that the camera follows the target with 
    public float cameraFollowSpeed = 0.2f;
    public float horizontalCameraSpeed = 2;
    public float verticalCameraSpeed = 2;

    public float horizontalAngle; // the angle rotating along with x-axis to look up and down(lookAngle)
    public float verticalAngle;// the angle rotating along with y-axis to look left and right(pivotAngle)
    public float minVerticalAngel = -35;
    public float maxVerticalAngle = 35;

    public float cameraCollisionRadius = 2;
    public float cameraCollisionOffset = 0.2f;  // How much the camera will jump off of objects its colliding with
    public float minimumCollisionOffset = 0.2f;
    public LayerMask collisionLayer;    // The layers that the camera collides with
    

    private void Awake()
    {
        // Set the InputManager
        inputManager = FindObjectOfType<InputManager>();
        // Set the target transform to Player's transform
        targetTransform = FindObjectOfType<PlayerManager>().transform;
        cameraTransform = Camera.main.transform;
        defaultPosition = cameraTransform.localPosition.z;  // localPosition : the relative distance between Camera and Player
    }

    // a function to encapsulate all functions related with Camera movement 
    public void HandleAllCameraMovement()
    {
        FollowTarget();
        RotateCamera();
        HandleCameraCollision();
    }

    // Have the camera follow Player
        // This gets called in LateUpdate() for it to move towards the target after every frame has been processed
    private void FollowTarget()
    {
        // Calculate the target position that camera will follow smoothly
        // Vector3.SmoothDamp() is used to move a camera smoothly
            // the velocity of the object can be changed in run time
            // the smooth time(cameraFollowSpeed) is an interval for the camera to be snapped to the target
        Vector3 targetPosition =
            Vector3.SmoothDamp(transform.position, targetTransform.position, ref cameraFollowVelocity, cameraFollowSpeed);

        // Update the camera's position
        transform.position = targetPosition;
    }

    // Have Camera rotate along with Player
    private void RotateCamera()
    {
        Vector3 rotation;

        // Caculate the vertical angle for the camera to follow
            // actually, this varaible should be called, "cameraYAxis" : Rotation is to be calculated along with axes
        horizontalAngle += (inputManager.horizontalCameraInput * horizontalCameraSpeed);
        // Caculate the horizontal angle for the camera to follow
            // actually, this varaible should be called, "cameraXAxis" : Rotation is to be calculated along with axes
        verticalAngle -= (inputManager.verticalCameraInput * verticalCameraSpeed);
        verticalAngle = Mathf.Clamp(verticalAngle, minVerticalAngel, maxVerticalAngle);

        // Calculate the horizontal rotation
        rotation = Vector3.zero;    // Reset the camera's rotation : without resetting, the base axis of rotation changes every frame
        rotation.y = horizontalAngle;
        Quaternion targetRotation = Quaternion.Euler(rotation); // Convert the euler angle to Quaternion
        transform.rotation = targetRotation;

        // Calculate the vertical rotation
        rotation = Vector3.zero;    // Reset the camera's rotation : without resetting, the base axis of rotation changes every frame
        rotation.x = verticalAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation; // for pivoting, it's supposed to be the local rotation
    }

    private void HandleCameraCollision()
    {
        // Reset Camera's depth to its default position everytime
        float targetPosition = defaultPosition;

        RaycastHit hit;
        Vector3 direction = cameraTransform.position - cameraPivot.position;
        direction.Normalize();

        if(Physics.SphereCast(
            cameraPivot.transform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetPosition), collisionLayer))
        {
            float distance = Vector3.Distance(cameraPivot.position, hit.point);
            Debug.Log("distance : " + distance);
            targetPosition = - (distance - cameraCollisionOffset);
        }

        if(Mathf.Abs(targetPosition) < minimumCollisionOffset)
        {
            targetPosition = targetPosition - minimumCollisionOffset;
        }

        cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, 0.2f);
        cameraTransform.localPosition = cameraVectorPosition;
    }
}
