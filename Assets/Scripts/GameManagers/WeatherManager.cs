using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    public static WeatherManager Instance;
    public GameObject rainEffect;
    private bool isRaining;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateWeather(GameTimestamp.Season currentSeason)
    {
        if (currentSeason == GameTimestamp.Season.Hujan)
        {
            // In the rainy season, there's a 60% chance it will rain
            isRaining = Random.value < 0.6f;
            //Debug.Log($"Season is HUJAN. Raining today? {isRaining}");
        }
        else
        {
            isRaining = false;
        }

        rainEffect.SetActive(isRaining);
    }

    public bool IsRaining()
    {
        return isRaining;
    }
}
