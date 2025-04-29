using UnityEngine;

public class Speedometer : MonoBehaviour
{
    [Header("References")]
    public Transform indicator;
    public WheelCollider referenceWheel; // e.g., frontLeftWheel

    [Header("Speedometer Settings")]
    public float maxSpeed = 8000f; // km/h
    public float minZRotation = 140f; // For zero speed
    public float maxZRotation = -130f; // For max speed

    void Update()
    {
        // Get speed in m/s using wheel RPM
        float wheelRPM = referenceWheel.rpm;
        float wheelRadius = referenceWheel.radius;
        float speedMS = (2 * Mathf.PI * wheelRadius * wheelRPM) / 60f;


        // Convert to km/h
        float currentSpeed = Mathf.Abs(speedMS * 3.6f);

        Debug.Log("Speed (km/h): " + currentSpeed);


        // Clamp and normalize speed value
        float t = Mathf.Clamp01(currentSpeed / maxSpeed);

        // Lerp Z rotation for indicator
        float zRotation = Mathf.Lerp(minZRotation, maxZRotation, t);

        // Apply rotation
        Vector3 currentRotation = indicator.localEulerAngles;
        indicator.localEulerAngles = new Vector3(currentRotation.x, currentRotation.y, zRotation);
    }
}
