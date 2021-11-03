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
    private Vector3 cameraVectorPosition;   // a variable hoding the result of processing jumped off camera position when colliding

    private Vector3 cameraFollowVelocity = Vector3.zero; // the current velocity that the camera follows the target with 
    public float cameraFollowSpeed = 0.2f;
    public float horizontalCameraSpeed = 2;
    public float verticalCameraSpeed = 2;

    public float horizontalAngle; // the angle rotating along with x-axis to look up and down(lookAngle)
    public float verticalAngle;// the angle rotating along with y-axis to look left and right(pivotAngle)
    public float minVerticalAngel = -35;
    public float maxVerticalAngle = 35;

    public float cameraCollisionRadius = 0.2f;
    public float cameraCollisionOffset = 0.2f;  // How much the camera will jump off of objects it's colliding with
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
        HandleCameraCollisions();
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
        Vector3 rotation;   // a variable holding the result of rotation

        // Caculate the vertical angle for the camera to follow
            // actually, this varaible should be called, "cameraYAxis" : Rotation is to be calculated along with axes
        horizontalAngle += (inputManager.horizontalCameraInput * horizontalCameraSpeed);
        // Caculate the horizontal angle for the camera to follow
            // actually, this varaible should be called, "cameraXAxis" : Rotation is to be calculated along with axes
        verticalAngle -= (inputManager.verticalCameraInput * verticalCameraSpeed);

        // Limit the vertical rotation
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

    private void HandleCameraCollisions()
    {
        // Reset Camera's depth to its default position everytime when it's free of collisions
        float targetPosition = defaultPosition;

        RaycastHit hit;
        Vector3 direction = cameraTransform.position - cameraPivot.position;    // the direction casting the sphere into
        direction.Normalize();

        // Cast a sphere from CameraPivot with a certain radius into the looking direction until Player's location, 
        // to detect whether any collision occurs with objects of specific layers
        if(Physics.SphereCast(
            cameraPivot.transform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetPosition), collisionLayer))
        {
            float distance = Vector3.Distance(cameraPivot.position, hit.point); // the distance between CameraPivot and the object causing the collision
            targetPosition = -(distance - cameraCollisionOffset);   // the z position the camera will be jumped off to
            // why -? : -z means, how far it is behind the camera
            // why "cameraCollisionOffset" : put a extra value between the obstacle and the camera to prevent camera from jitterring
                // by brining the camera towards Player a bit more
        }

        // if the distance between the camera and Player is too close,
        // let the camera pass Player through against the wall
        if(Mathf.Abs(targetPosition) < minimumCollisionOffset)
        {
            targetPosition -= minimumCollisionOffset;
        }

        // Apply the calculated value smoothly
        cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, 0.2f);
        cameraTransform.localPosition = cameraVectorPosition;
    }
}
