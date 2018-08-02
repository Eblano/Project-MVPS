using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    public class RealtimeGlobalLightingManager : MonoBehaviour
    {
        [System.Serializable]
        private class LightSettings
        {
            public Color lightColor;
            public float lightIntensity;
        }

        public static RealtimeGlobalLightingManager instance;
        [SerializeField] private List<Light> lights;

        [Header("Lighting Settings")]
        [SerializeField] private LightSettings sunsetLight;
        [SerializeField] private LightSettings midnightLight;
        [SerializeField] private LightSettings daytimeLight;

        private void Start()
        {
            if (instance == null)
                instance = this;
            else
            {
                Debug.Log("Duplicate RealtimeGlobalLightingManager, removing this");
                Destroy(this);
            }

            //foreach (Light light in lights)
            //{
            //    light.shadowResolution = UnityEngine.Rendering.LightShadowResolution.High;
            //}
        }

        public void SetLightMode(GlobalEnums.SKYBOXTYPE skyboxType)
        {
            switch (skyboxType)
            {
                case GlobalEnums.SKYBOXTYPE.Sunset:
                    foreach(Light light in lights)
                    {
                        light.color = sunsetLight.lightColor;
                        light.intensity = sunsetLight.lightIntensity;
                    }
                    break;
                case GlobalEnums.SKYBOXTYPE.Midnight:
                    foreach (Light light in lights)
                    {
                        light.color = midnightLight.lightColor;
                        light.intensity = midnightLight.lightIntensity;
                    }
                    break;
                case GlobalEnums.SKYBOXTYPE.DayTime:
                    foreach (Light light in lights)
                    {
                        light.color = daytimeLight.lightColor;
                        light.intensity = daytimeLight.lightIntensity;
                    }
                    break;
            }
        }
    }
}

