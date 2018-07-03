using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    [ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllPublic)]
    public class RTESkyboxSetter : MonoBehaviour
    {
        private Material[] skyboxMats;
        [SerializeField] private enum SKYBOXTYPE { DayTime, Sunset, Midnight_With_Moon, DayTime2, DayTime3, Noon };
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
                    skyboxMat = skyboxMats[0];
                    break;
                case SKYBOXTYPE.Sunset:
                    skyboxMat = skyboxMats[1];
                    break;
                case SKYBOXTYPE.Midnight_With_Moon:
                    skyboxMat = skyboxMats[2];
                    break;
                case SKYBOXTYPE.DayTime2:
                    skyboxMat = skyboxMats[3];
                    break;
                case SKYBOXTYPE.DayTime3:
                    skyboxMat = skyboxMats[4];
                    break;
                case SKYBOXTYPE.Noon:
                    skyboxMat = skyboxMats[5];
                    break;
                default:
                    skyboxMat = skyboxMats[0];
                    break;
            }

            RenderSettings.skybox = skyboxMat;
        }
    }
}