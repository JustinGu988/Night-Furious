using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarWheelController : MonoBehaviour
{
    [SerializeField] WheelCollider frontRightCollider;
    [SerializeField] WheelCollider frontLeftCollider;
    [SerializeField] WheelCollider backLeftCollider;
    [SerializeField] WheelCollider backRightCollider;

    [SerializeField] Transform frontRightTransform;
    [SerializeField] Transform frontLeftTransform;
    [SerializeField] Transform backLeftTransform;
    [SerializeField] Transform backRightTransform;

    [SerializeField] private float accelerationFactor = 100f;
    [SerializeField] private float steerFactor = 20f;
    [SerializeField] private float dampingFactor = 10f;

    private float acceleration = 0f;
    private float steerAngle = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Get inputs
        acceleration = Input.GetAxis("Vertical") * accelerationFactor;
        steerAngle = Input.GetAxis("Horizontal") * steerFactor;

        // Acceleration on each wheel (all wheel drive)
        accelerate(frontRightCollider);
        accelerate(frontLeftCollider);
        accelerate(backLeftCollider);
        accelerate(backRightCollider);

        // Steering (front wheels only)
        frontRightCollider.steerAngle = steerAngle;
        frontLeftCollider.steerAngle = steerAngle;

        // Rotate wheels when driving
        moveWheels(frontLeftCollider, frontLeftTransform);
        moveWheels(frontRightCollider, frontRightTransform);
        moveWheels(backLeftCollider, backLeftTransform);
        moveWheels(backRightCollider, backRightTransform);
    }

    /**
     * Apply acceleration and damping to the given wheel collider
     */ 
    private void accelerate(WheelCollider collider)
    {
        collider.motorTorque = acceleration;
        collider.wheelDampingRate = dampingFactor;
    }

    private void moveWheels(WheelCollider collider, Transform transform)
    {
        Vector3 pos;
        Quaternion rot;
        collider.GetWorldPose(out pos, out rot);
        transform.SetPositionAndRotation(pos, rot);
    }
}
