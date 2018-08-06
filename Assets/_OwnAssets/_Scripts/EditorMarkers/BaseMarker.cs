using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battlehub.RTGizmos;
using UnityEngine.AI;
using Battlehub.RTCommon;

namespace SealTeam4
{
    /// <summary>
    /// Base class for markers
    /// </summary>
    public class BaseMarker : MonoBehaviour, IMarkerBehaviours
    {
        protected float canvasVerticalOffset = 0;
        private string oldName;
        private ExposeToEditor m_ExposedToEditor;

        protected void Start()
        {
            m_ExposedToEditor = GetComponent<ExposeToEditor>();
        }

        protected virtual void Update()
        {
            CheckMarkerNameChange();
        }

        private void CheckMarkerNameChange()
        {
            if ((oldName != gameObject.name && !GameManager.instance.MarkerNameIsNotUsedByOtherMarkers(this)) || gameObject.name.Contains("-"))
            {
                gameObject.name = oldName;
                m_ExposedToEditor.SetName(gameObject.name);
            }
            oldName = gameObject.name;
        }

        /// <summary>
        /// Register marker to current active GameManager
        /// </summary>
        /// <param name="markerType"></param>
        protected void RegisterMarkerOnGameManager(BaseMarker marker)
        {
            if (GameManager.instance)
            {
                string newMarkerName =
                    GameManager.instance.RegisterMarker(marker);

                gameObject.name = newMarkerName;
                oldName = newMarkerName;
                m_ExposedToEditor.SetName(gameObject.name);
            }
            else
            {
                Debug.LogWarning("GameManager is not Running");
            }
        }

        /// <summary>
        /// Removes Visual indication of the marker
        /// Called by GameManager
        /// </summary>
        public virtual void CleanUpForSimulationStart()
        {
            //Destroy(GetComponent<DynamicBillboard>().GetCanvas());
        }

        private void OnDisable()
        {
            GameManager.instance.UnregisterMarker(gameObject);
        }
    }
}
