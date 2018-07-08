using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    [ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllPublic)]
    public class RTESkyboxSetter : MonoBehaviour
    {
        private Material[] skyboxMats;
        [SerializeField] private enum SKYBOXTYPE { Sunset, Midnight, DayTime };
        [SerializeField] private SKYBOXTYPE skyboxType = SKYBOXTYPE.DayTime;
        private SKYBOXTYPE prevSkyboxType;

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
                case SKYBOXTYPE.DayTime:
                    skyboxMat = skyboxMats[2];
                    break;
                case SKYBOXTYPE.Sunset:
                    skyboxMat = skyboxMats[0];
                    break;
                case SKYBOXTYPE.Midnight:
                    skyboxMat = skyboxMats[1];
                    break;
                default:
                    skyboxMat = skyboxMats[0];
                    break;
            }

            RenderSettings.skybox = skyboxMat;
            DynamicGI.UpdateEnvironment();
        }
    }
}