using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    [ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllPublic)]
    public class RTESkyboxSetter : MonoBehaviour
    {
        private Material[] skyboxMats;
        [SerializeField] private GlobalEnums.SKYBOXTYPE skyboxType = GlobalEnums.SKYBOXTYPE.DayTime;
        private GlobalEnums.SKYBOXTYPE prevSkyboxType;

        private void Start()
        {
            prevSkyboxType = skyboxType;
            GetSkyboxes();
            ApplySkybox();
        }

        private void Update()
        {
            if(skyboxType != prevSkyboxType)
            {
                ApplySkybox();
            }
        }

        private void GetSkyboxes()
        {
            skyboxMats = RuntimeEditorUltiltes.instance.GetSkyboxMats();
        }

        private void ApplySkybox()
        {
            prevSkyboxType = skyboxType;

            Material skyboxMat;

            switch (skyboxType)
            {
                case GlobalEnums.SKYBOXTYPE.DayTime:
                    skyboxMat = skyboxMats[2];
                    if (RealtimeGlobalLightingManager.instance)
                        RealtimeGlobalLightingManager.instance.SetLightMode(GlobalEnums.SKYBOXTYPE.DayTime);
                    break;
                case GlobalEnums.SKYBOXTYPE.Sunset:
                    skyboxMat = skyboxMats[0];
                    if (RealtimeGlobalLightingManager.instance)
                        RealtimeGlobalLightingManager.instance.SetLightMode(GlobalEnums.SKYBOXTYPE.Sunset);
                    break;
                case GlobalEnums.SKYBOXTYPE.Midnight:
                    skyboxMat = skyboxMats[1];
                    if (RealtimeGlobalLightingManager.instance)
                        RealtimeGlobalLightingManager.instance.SetLightMode(GlobalEnums.SKYBOXTYPE.Midnight);
                    break;
                default:
                    skyboxMat = skyboxMats[0];
                    if (RealtimeGlobalLightingManager.instance)
                        RealtimeGlobalLightingManager.instance.SetLightMode(GlobalEnums.SKYBOXTYPE.Sunset);
                    break;
            }

            RenderSettings.skybox = skyboxMat;
            DynamicGI.UpdateEnvironment();
        }
    }
}