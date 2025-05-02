using UnityEngine;

public class Speedometer : MonoBehaviour
{
    [Header("References")]
    public Transform indicator;
    public Rigidbody carRb;

    [Header("Speedometer Settings")]
    public float maxSpeed = 300f; // Max speed in km/h
    public float minZRotation = 140f; // Needle at 0 km/h
    public float maxZRotation = -130f; // Needle at maxSpeed

    void Update()
    {
        // Get current speed in km/h
        float currentSpeed = carRb.velocity.magnitude * 3.6f;

        // Clamp and normalize speed value
        float t = Mathf.Clamp01(currentSpeed / maxSpeed);

        // Lerp Z rotation for indicator
        float zRotation = Mathf.Lerp(minZRotation, maxZRotation, t);

        // Apply rotation to indicator needle
        Vector3 currentRotation = indicator.localEulerAngles;
        indicator.localEulerAngles = new Vector3(currentRotation.x, currentRotation.y, zRotation);
    }
}
