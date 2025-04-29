using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class playerController : MonoBehaviour
{
    [Header("Bus Settings")]
    public float motorForce = 3000f;
    public float brakeForce = 8000f;
    public float maxSteerAngle = 25f;
    public float reverseForce = 1500f;

    [Header("Engine Resistance")]
    public float engineBrakeForce = 200f; // Braking when throttle is 0
    public float accelerationRate = 2f; // How fast throttle increases
    public float decelerationRate = 5f; // How fast throttle decreases

    [Header("Wheels")]
    public WheelCollider frontLeftWheelCollider;
    public WheelCollider frontRightWheelCollider;
    public WheelCollider rearLeftWheelCollider;
    public WheelCollider rearRightWheelCollider;

    [Header("Steering Settings")]
    public float steeringSmoothTime = 0.2f;
    private float currentSteering = 0f;
    private float steeringVelocity = 0f;

    public Transform frontLeftWheelTransform;
    public Transform frontRightWheelTransform;
    public Transform rearLeftWheelTransform;
    public Transform rearRightWheelTransform;

    private Rigidbody rb;
    private float steeringInput;
    private float throttleInput;
    private float brakeInput;
    private float currentThrottle = 0f;

    public Text gearIndicator;
    private bool isAccelerating = false;
    private bool isBraking = false;


    // Reverse mode
    private bool isReversing = false;
    private float brakeHoldTime = 0f;
    private float timeToReverse = 1.5f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.drag = 0.1f; // Tiny drag for realism
        rb.angularDrag = 0.5f;
        rb.centerOfMass = new Vector3(0f, -0.5f, 0f); // Less extreme
    }

    private void FixedUpdate()
    {
        // Smoothly adjust throttleInput
        if (isAccelerating)
            throttleInput = Mathf.MoveTowards(throttleInput, 1f, accelerationRate * Time.fixedDeltaTime);
        else
            throttleInput = Mathf.MoveTowards(throttleInput, 0f, decelerationRate * Time.fixedDeltaTime);

        // Smoothly adjust brakeInput
        if (isBraking)
            brakeInput = Mathf.MoveTowards(brakeInput, 1f, accelerationRate * Time.fixedDeltaTime);
        else
            brakeInput = Mathf.MoveTowards(brakeInput, 0f, decelerationRate * Time.fixedDeltaTime);

        CheckReverseState();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
    }

    private void CheckReverseState()
    {
        if (brakeInput > 0f && throttleInput == 0f && rb.velocity.magnitude < 0.5f)
        {
            brakeHoldTime += Time.fixedDeltaTime;
            if (brakeHoldTime >= timeToReverse)
            {
                isReversing = true;
            }
        }
        else if (throttleInput > 0f)
        {
            isReversing = false;
            brakeHoldTime = 0f;
        }
        else if (brakeInput == 0f)
        {
            brakeHoldTime = 0f;
        }
    }
    private void HandleMotor()
    {
        // Smooth throttle adjustment
        if (throttleInput > 0f && !isReversing)
        {
            currentThrottle = Mathf.MoveTowards(currentThrottle, 1f, accelerationRate * Time.fixedDeltaTime);
        }
        else
        {
            currentThrottle = Mathf.MoveTowards(currentThrottle, 0f, decelerationRate * Time.fixedDeltaTime);
        }

        float motorTorque = 0f;

        if (isReversing)
        {
            motorTorque = brakeInput * -reverseForce;
            gearIndicator.text = "R";
        }
        else
        {
            motorTorque = currentThrottle * motorForce;
            gearIndicator.text = "D";
        }

        frontLeftWheelCollider.motorTorque = motorTorque;
        frontRightWheelCollider.motorTorque = motorTorque;

        float totalBrake = 0f;

        if (!isReversing)
        {
            totalBrake = brakeInput * brakeForce;

            if (Mathf.Approximately(currentThrottle, 0f) && brakeInput == 0f)
            {
                totalBrake += engineBrakeForce;

                // Limit speed reduction to simulate slow roll instead of full stop
                if (rb.velocity.magnitude < 10f)
                {
                    totalBrake = 0f; // Stop applying engine brake at very low speeds
                }
            }

        }

        frontLeftWheelCollider.brakeTorque = totalBrake;
        frontRightWheelCollider.brakeTorque = totalBrake;
        rearLeftWheelCollider.brakeTorque = totalBrake;
        rearRightWheelCollider.brakeTorque = totalBrake;
    }

    private void HandleSteering()
    {
        // Smooth steering interpolation
        currentSteering = Mathf.SmoothDamp(currentSteering, steeringInput, ref steeringVelocity, steeringSmoothTime);
        float steerAngle = currentSteering * maxSteerAngle;

        frontLeftWheelCollider.steerAngle = steerAngle;
        frontRightWheelCollider.steerAngle = steerAngle;
    }


    private void UpdateWheels()
    {
        UpdateWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateWheel(rearLeftWheelCollider, rearLeftWheelTransform);
        UpdateWheel(rearRightWheelCollider, rearRightWheelTransform);
    }

    private void UpdateWheel(WheelCollider collider, Transform visualWheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        collider.GetWorldPose(out pos, out rot);

        // Update ONLY the visual wheel
        visualWheelTransform.position = pos;
        visualWheelTransform.rotation = rot;
    }

    // PUBLIC METHODS for UI BUTTONS

    public void PressAccelerate()
    {
        isAccelerating = true;
    }

    public void ReleaseAccelerate()
    {
        isAccelerating = false;
    }

    public void PressBrake()
    {
        isBraking = true;
    }

    public void ReleaseBrake()
    {
        isBraking = false;
    }


    public void PressSteerLeft()
    {
        steeringInput = -1f;
    }

    public void PressSteerRight()
    {
        steeringInput = 1f;
    }

    public void ReleaseSteering()
    {
        steeringInput = 0f;
    }

    public void SetSteering(float input)
    {
        steeringInput = input;
    }

    public float GetSteering()
    {
        return steeringInput;
    }


}
