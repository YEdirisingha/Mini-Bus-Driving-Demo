using UnityEngine;

public class Detector : MonoBehaviour
{
    public enum DetectionType
    {
        Front,
        Left,
        Right
    }

    public DetectionType detectionType;
    public bool playerDetected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Vehicle"))
        {
            playerDetected = true;
            // You can also add specific actions here when the player is detected
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Vehicle"))
        {
            playerDetected = false;
        }
    }
}
