using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    public class CiellingLight : MonoBehaviour, IActions
    {
        [Battlehub.SerializeIgnore] [SerializeField] private Light light;
        private BoxCollider collider;
        private List<string> actionableList = new List<string>();

        [Battlehub.SerializeIgnore] [SerializeField] private Transform highestPoint;
        [SerializeField] private bool lightsOn = true;
        [SerializeField] private float range = 10f;
        [SerializeField] private float intensity = 1f;
        [Range(10, 130)] [SerializeField] private float angle = 50f;
        [SerializeField] private Color lightColor = Color.white;
        
        private void Start()
        {
            collider = GetComponent<BoxCollider>();
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
                light.enabled = true;
            else
                light.enabled = false;

            light.range = range;
            light.intensity = intensity;
            light.spotAngle = angle;
            light.color = lightColor;
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
            return highestPoint.position;
        }

        public Transform GetHighestPointTransform()
        {
            return highestPoint;
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
                    break;
                case "Off":
                    lightsOn = false;
                    break;
            }
        }
    }
}
