using UnityEngine;
using System.Collections;

public class BusStopTrigger : MonoBehaviour
{
    private float stopTimer = 0f;
    private bool busInZone = false;
    public GameObject[] peopleToBoard; // Set people GameObjects in Inspector
    public GameObject[] peopleToExist;
    public Transform busDoorLocation; // Where people walk to
    public Transform spawnLocation;
    public Transform exitLocation;

    private bool hasTriggered = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            busInZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            busInZone = false;
            stopTimer = 0f;
        }
    }

    private void Update()
    {
        if (busInZone)
        {
            stopTimer += Time.deltaTime;

            if (stopTimer >= 3f)
            {
                StartBoarding();
                StartCoroutine(ActivateExitingPeople());
                hasTriggered = true; // Prevent triggering again while bus is stopped

                busInZone = false; // Prevent triggering again
            }
        }
    }
    private void StartBoarding()
    {
        foreach (GameObject person in peopleToBoard)
        {
            if (person != null)
            {
                person.GetComponent<SimpleWalker>().WalkTo(busDoorLocation.position);
            }
        }
    }

    private IEnumerator ActivateExitingPeople()
    {
        foreach (GameObject person in peopleToExist)
        {
            if (person != null)
            {
                person.SetActive(true);
                person.GetComponent<SimpleWalker>().WalkTo(exitLocation.position);
                yield return new WaitForSeconds(1f); // Wait 1 second between each person
            }
        }
    }
}
