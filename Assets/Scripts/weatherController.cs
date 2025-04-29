using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherController : MonoBehaviour
{
    public GameObject weather;
    public bool activateWeather; // true = start weather, false = end weather

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (weather != null)
            {
                weather.SetActive(activateWeather);
            }
        }
    }
}
