using Battlehub.RTSaveLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    [ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllPublic)]
    public class CiellingLight : MonoBehaviour, IActions, INetworkCommandable
    {
        private Light light;
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
            GameObject lightGo = new GameObject();
            lightGo.name = "CeillingLight";
            lightGo.AddComponent<PersistentIgnore>();
            light = lightGo.AddComponent<Light>();
            light.type = LightType.Point;
            light.shadows = LightShadows.Soft;
            light.cullingMask = ~(1 << LayerMask.NameToLayer("AreaMarker"));
        }

        private void Update()
        {
            if(light)
            {
                UpdateLightParams();
                UpdateActionables();
            }
        }

        private void UpdateLightParams()
        {
            if (lightsOn)
            {
                light.enabled = true;
            }
            else
            {
                light.enabled = false;
            }

            if(light)
            {
                light.range = range;
                light.intensity = intensity;
                light.spotAngle = angle;
                light.color = lightColor;
                light.transform.position = transform.position;
                light.transform.rotation = transform.rotation * Quaternion.Euler(90, 0, 0);
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
            if(light)
                Destroy(light.gameObject);

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
