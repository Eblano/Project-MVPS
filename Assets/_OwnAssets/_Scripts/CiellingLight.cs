using Battlehub.RTSaveLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    [ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllPublic)]
    public class CiellingLight : MonoBehaviour, IActions, INetworkCommandable
    {
        private Light pointLight;
        private BoxCollider collider;
        private List<string> actionableList = new List<string>();

        [SerializeField] private bool lightsOn = true;
        [SerializeField] private float range = 10f;
        [SerializeField] private float intensity = 2f;
        [SerializeField] private Color lightColor = Color.white;
        
        private void Start()
        {
            collider = GetComponent<BoxCollider>();
        }

        private void CreateLightOBJ()
        {
            GameObject pointLightGO = new GameObject();
            pointLightGO.name = "pointLight";
            pointLightGO.transform.SetParent(gameObject.transform);
            pointLightGO.transform.localPosition = new Vector3(0, -3f, 0);
            pointLightGO.AddComponent<PersistentIgnore>();

            pointLight = pointLightGO.AddComponent<Light>();
            pointLight.type = LightType.Point;
            pointLight.shadows = LightShadows.Soft;
            pointLight.renderMode = LightRenderMode.ForcePixel;
            pointLight.range = range;
            pointLight.intensity = intensity;

            pointLight.cullingMask = ~(1 << LayerMask.NameToLayer("AreaMarker"));
        }

        private void Update()
        {
            if(pointLight)
            {
                UpdateLightParams();
                UpdateActionables();
            }
        }

        private void UpdateLightParams()
        {
            if (lightsOn)
            {
                pointLight.enabled = true;
            }
            else
            {
                pointLight.enabled = false;
            }

            if(lightsOn)
            {
                pointLight.range = range;
                pointLight.intensity = intensity;
                pointLight.color = lightColor;
            }
        }

        private void UpdateActionables()
        {
            if(!lightsOn && !actionableList.Contains("On"))
            {
                actionableList.Add("On");
                if(actionableList.Contains("Off"))
                    actionableList.Remove("Off");
            }
            if(lightsOn && !actionableList.Contains("Off"))
            {
                actionableList.Add("Off");
                if (actionableList.Contains("On"))
                    actionableList.Remove("On");
            }
        }

        private void OnDisable()
        {
            if(pointLight)
                Destroy(pointLight.gameObject);

            GameManager.instance.UnregisterNetCmdObj(gameObject);
        }

        private void OnEnable()
        {
            CreateLightOBJ();
            GameManager.instance.RegisterNetCmdObj(gameObject, false);
        }

        public List<string> GetActions()
        {
            return actionableList;
        }

        public Collider GetCollider()
        {
            return collider;
        }

        public Vector3 GetHighestPointPos()
        {
            return transform.position;
        }

        public Transform GetHighestPointTransform()
        {
            return transform;
        }

        public string GetName()
        {
            return gameObject.name;
        }

        public void SetAction(string action)
        {
            switch (action)
            {
                case "On":
                    lightsOn = true;
                    GameManager.instance.SendNetCmdObjMsg(gameObject, "On");
                    break;
                case "Off":
                    lightsOn = false;
                    GameManager.instance.SendNetCmdObjMsg(gameObject, "Off");
                    break;
            }
        }

        public void RecieveCommand(string command)
        {
            switch(command)
            {
                case "On":
                    lightsOn = true;
                    break;

                case "Off":
                    lightsOn = false;
                    break;
            }
        }
    }
}
