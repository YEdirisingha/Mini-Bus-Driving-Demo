using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;
using Unity.Mathematics;

public class CarSplineFollow : MonoBehaviour
{
    public SplineContainer splineContainer;
    public List<SplineContainer> laneSplines;
    public float speed = 5f;
    public float laneChangeCooldown = 2f;

    private float t = 0f;
    private float laneChangeTimer = 0f;
    private bool isBlocked = false;

    public Detector frontDetector;
    public Detector rightDetector;
    public Detector leftDetector;
    public float startNode = 0f; // New variable to determine where the car starts on the spline (0 to 1)

    void Start()
    {
        t = startNode; // Set the starting position based on startNode value
    }
    void Update()
    {
        if (splineContainer == null) return;

        if (!isBlocked)
        {
            t += speed * Time.deltaTime / splineContainer.CalculateLength();
            if (t > 1f) t = 0f; // Loop back if needed
        }

        // Set car position along the spline
        transform.position = splineContainer.EvaluatePosition(t);

        // Set car rotation along the tangent
        Vector3 forward = splineContainer.EvaluateTangent(t);
        if (forward != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(forward);
            transform.rotation = lookRotation;
        }

        // Update lane change cooldown timer
        laneChangeTimer -= Time.deltaTime;

        // Check if the car should change lanes or stop
        DetectCarAheadAndChangeLane();
    }

    void DetectCarAheadAndChangeLane()
    {
        if (laneChangeTimer > 0f) return;

        // If there's a player ahead (in front), try to change lanes
        if (frontDetector.playerDetected)
        {
            bool changed = TryChangeLane();
            if (!changed)
            {
                isBlocked = true; // Stop if the car can't change lanes
            }
            else
            {
                isBlocked = false; // Car can move again
            }

            laneChangeTimer = laneChangeCooldown; // Reset lane change cooldown
        }
        else
        {
            isBlocked = false; // No need to block the car if there is no player or vehicle in front
        }
    }

    bool TryChangeLane()
    {
        foreach (var lane in laneSplines)
        {
            if (lane == splineContainer) continue; // Skip current lane

            // Check if side lane is clear
            bool sideBlocked = false;

            if (lane.transform.position.x > transform.position.x)
            {
                // Target lane is to the right
                sideBlocked = rightDetector.playerDetected;
            }
            else
            {
                // Target lane is to the left
                sideBlocked = leftDetector.playerDetected;
            }

            if (!sideBlocked)
            {
                // Change to the new lane if it's clear
                splineContainer = lane;
                var spline = splineContainer.Spline;
                SplineUtility.GetNearestPoint(spline, transform.position, out float3 nearestT, out var nearestPoint);
                return true; // Lane change successful
            }
        }

        return false; // No valid lane to change to
    }
}
