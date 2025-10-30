using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
[ExecuteAlways]
public class LightManage : MonoBehaviour
{
    [SerializeField] private Light DirectionalLight;
    [SerializeField] private LightPreset Preset;
    public TimeManager time;
    GameTimestamp timestamp;

    private float TimeofDay;

    private void Update()
    {
        timestamp = time.TimeGiver();
        if (Preset == null)
            return;
        if (Application.isPlaying)
        {
            float TimeofDay = (timestamp.hour*100)+(((float)timestamp.minute /60f)*100);
            TimeofDay %= 2400f;
            UpdateLight(TimeofDay / 2400f);
        }
    }

    private void UpdateLight(float timePercent)
    {
        RenderSettings.ambientLight = Preset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = Preset.FogColor.Evaluate(timePercent);
        
        if(DirectionalLight!=null)
        {
            DirectionalLight.color = Preset.DirectionalColor.Evaluate(timePercent);
            
            DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f)-60f, -30f, 0));
        }
    }

    private void OnValidate()
    {
        if(DirectionalLight != null)
            return;
        if (RenderSettings.sun != null)
            DirectionalLight = RenderSettings.sun;
        else
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach(Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    DirectionalLight = light;
                    return;
                }
                    
            }
        }
    }
}
