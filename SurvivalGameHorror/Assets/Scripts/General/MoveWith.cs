using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWith : MonoBehaviour
{
    public Transform target; // The target to rotate towards (camera)
    public float smoothness = 5.0f; // Smoothness of the rotation
    public float lagTime = 0.2f; // Lag time in seconds

    private Quaternion targetRotation;

    void Start()
    {
        // Set initial target rotation
        targetRotation = transform.rotation;
    }

    void LateUpdate()
    {
        // If no target is set, exit
        if (target == null)
            return;

        // Calculate the rotation difference between camera and flashlight
        Quaternion newRotation = Quaternion.LookRotation(target.forward, target.up);

        // Smoothly interpolate between current rotation and target rotation with some lag
        targetRotation = Quaternion.Slerp(targetRotation, newRotation, Time.deltaTime * smoothness * (1 / lagTime));

        // Apply the new rotation to the flashlight
        transform.rotation = targetRotation;
    }
}
