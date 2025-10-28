using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class FogManager : MonoBehaviour
{
    [SerializeField] private LightPreset preset;
    [SerializeField] private TimeManager timeManager;

    [Header("Fog Settings")]
    [Tooltip("Enable dynamic density changes")]
    public bool controlFogDensity = true;
    [Range(0f, 0.1f)] public float baseFogDensity = 0.01f;
    [Range(0f, 0.05f)] public float maxFogVariation = 0.02f;

    [Header("Active Time Range")]
    [Range(0, 23)] public int startFogHour = 18;
    [Range(0, 23)] public int endFogHour = 6;

    private void Update()
    {
        if (preset == null || timeManager == null) return;

        GameTimestamp timestamp = timeManager.TimeGiver();
        float timePercent = GetTimePercent(timestamp);

        bool shouldFogBeActive = IsWithinFogTime(timestamp.hour);

        if (shouldFogBeActive)
        {
            UpdateFog(timePercent);
        }
        else
        {
            RenderSettings.fog = false; // turn off fog
        }
    }

    float GetTimePercent(GameTimestamp timestamp)
    {
        float timeOfDay = (timestamp.hour * 100) + ((float)timestamp.minute / 60f) * 100;
        return (timeOfDay % 2400f) / 2400f;
    }

    bool IsWithinFogTime(int hour)
    {
        if (startFogHour < endFogHour)
        {
            return hour >= startFogHour && hour < endFogHour;
        }
        else
        {
            // Handles fog time crossing midnight
            return hour >= startFogHour || hour < endFogHour;
        }
    }

    void UpdateFog(float timePercent)
    {
        RenderSettings.fog = true;
        RenderSettings.fogColor = preset.FogColor.Evaluate(timePercent);

        if (controlFogDensity)
        {
            float curve = Mathf.Sin(timePercent * Mathf.PI * 2); // denser at night
            float dynamicDensity = baseFogDensity + (curve * maxFogVariation);
            RenderSettings.fogDensity = Mathf.Clamp(dynamicDensity, 0f, 1f);
        }
    }
}
