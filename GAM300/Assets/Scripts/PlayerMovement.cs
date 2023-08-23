using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    private float horizontalInput;
    private float verticalInput;
    private float currentSteerAngle;
    //private float currentBrakeForce;
    //private bool isBraking;

    [Header("Wheel Colliders")]
    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;

    [Header("Wheel Transforms")]
    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;

    [Header("Force Settings")]
    [SerializeField] private float motorForce;
    [SerializeField] private float maxSteerAngle;
    //[SerializeField] private float brakeForce;

    private void FixedUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis(HORIZONTAL);
        verticalInput = Input.GetAxis(VERTICAL);
        //isBraking = Input.GetKey(KeyCode.LeftShift);
    }

    private void HandleMotor()
    {
        frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
        frontRightWheelCollider.motorTorque = verticalInput * motorForce;

        //currentBrakeForce = isBraking ? brakeForce : 0f;

        //if (isBraking)
        //{
        //    ApplyBraking();
        //}
    }

    //private void ApplyBraking()
    //{
    //    frontLeftWheelCollider.brakeTorque = currentBrakeForce;
    //    frontRightWheelCollider.brakeTorque = currentBrakeForce;
    //    rearLeftWheelCollider.brakeTorque = currentBrakeForce;
    //    rearRightWheelCollider.brakeTorque = currentBrakeForce;
    //}

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;

        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCol, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;

        wheelCol.GetWorldPose(out pos, out rot);

        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }
}
