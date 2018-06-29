using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battlehub.RTGizmos;
using UnityEngine.AI;

namespace SealTeam4
{
    /// <summary>
    /// Base class for markers
    /// </summary>
    public class BaseMarker : MonoBehaviour, IMarkerBehaviours
    {
        protected float canvasVerticalOffset = 0;
        private string oldName;

        protected virtual void Update()
        {
            //CheckMarkerNameChange();
        }

        private void CheckMarkerNameChange()
        {
            //if(oldName != gameObject.name && GameManager.instance.MarkerNameExists(gameObject.name))
            //{
            //    gameObject.name = oldName;
            //}
        }

        /// <summary>
        /// Register marker to current active GameManager
        /// </summary>
        /// <param name="markerType"></param>
        protected string RegisterMarkerOnGameManager(GameManager.MARKER_TYPE markerType)
        {
            if (GameManager.instance)
            {
                string newMarkerName = 
                    GameManager.instance.RegisterMarker(
                    this.gameObject,
                    markerType
                    );

                oldName = newMarkerName;
                return newMarkerName;
            }
            else
            {
                Debug.LogWarning("GameManager is not Running");
                return "No Name";
            }
        }

        /// <summary>
        /// Removes Visual indication of the marker
        /// Called by GameManager
        /// </summary>
        public virtual void CleanUpForSimulationStart()
        {
            Destroy(GetComponent<DynamicBillboard>().GetCanvas());
        }

        private void OnDisable()
        {
            GameManager.instance.UnregisterMarker(gameObject);
        }
    }
}
