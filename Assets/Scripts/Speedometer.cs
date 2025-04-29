using UnityEngine;

public class Speedometer : MonoBehaviour
{
    [Header("References")]
    public Transform indicator;
    public Rigidbody playerRigidbody;

    [Header("Speedometer Settings")]
    public float maxSpeed = 100f;

    public float minZRotation = 140f; // For zero speed
    public float maxZRotation = -130f; // For max speed

    void Update()
    {
        // Get the current speed ()
        float currentSpeed = playerRigidbody.velocity.magnitude;

        // Clamp the normalized speed value between 0 and 1
        float t = Mathf.Clamp01(currentSpeed / maxSpeed);

        // Lerp between the specified rotation boundaries
        float zRotation = Mathf.Lerp(minZRotation, maxZRotation, t);

        // Set the indicator's rotation.
        Vector3 currentRotation = indicator.localEulerAngles;
        indicator.localEulerAngles = new Vector3(currentRotation.x, currentRotation.y, zRotation);
    }
}
