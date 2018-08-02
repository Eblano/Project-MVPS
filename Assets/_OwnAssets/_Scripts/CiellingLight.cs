using Battlehub.RTSaveLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    [ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllPublic)]
    public class CiellingLight : MonoBehaviour, IActions, INetworkCommandable
    {
        private Light spotLight;
        private Light pointLight;
        private BoxCollider collider;
        private List<string> actionableList = new List<string>();

        [SerializeField] private bool lightsOn = true;
        [SerializeField] private float range = 20f;
        [SerializeField] private float intensity = 1f;
        [Range(10, 150)] [SerializeField] private float angle = 100f;
        [SerializeField] private Color lightColor = Color.white;
        
        private void Start()
        {
            collider = GetComponent<BoxCollider>();
        }

        private void CreateLightOBJ()
        {
            GameObject spotLightGO = new GameObject();
            spotLightGO.name = "spotLight";
            spotLightGO.transform.SetParent(gameObject.transform);
            spotLightGO.transform.localPosition = new Vector3(0, -0.1f, 0);
            spotLightGO.AddComponent<PersistentIgnore>();

            spotLight = spotLightGO.AddComponent<Light>();
            spotLight.type = LightType.Spot;
            spotLight.shadows = LightShadows.Soft;
            spotLight.renderMode = LightRenderMode.ForcePixel;

            GameObject pointLightGO = new GameObject();
            pointLightGO.name = "pointLight";
            pointLightGO.transform.SetParent(gameObject.transform);
            pointLightGO.transform.localPosition = new Vector3(0, -0.5f, 0);
            pointLightGO.AddComponent<PersistentIgnore>();

            pointLight = pointLightGO.AddComponent<Light>();
            pointLight.type = LightType.Point;
            pointLight.shadows = LightShadows.Soft;
            pointLight.renderMode = LightRenderMode.Auto;
            pointLight.range = 0.5f;
            pointLight.intensity = 3f;

            spotLight.cullingMask = ~(1 << LayerMask.NameToLayer("AreaMarker"));
        }

        private void Update()
        {
            if(spotLight)
            {
                UpdateLightParams();
                UpdateActionables();
            }
        }

        private void UpdateLightParams()
        {
            if (lightsOn)
            {
                spotLight.enabled = true;
                pointLight.enabled = true;
            }
            else
            {
                spotLight.enabled = false;
                pointLight.enabled = false;
            }

            if(spotLight)
            {
                spotLight.range = range;
                spotLight.intensity = intensity;
                spotLight.spotAngle = angle;
                spotLight.color = lightColor;
                spotLight.transform.rotation = transform.rotation * Quaternion.Euler(90, 0, 0);
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
            if(spotLight)
                Destroy(spotLight.gameObject);

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
