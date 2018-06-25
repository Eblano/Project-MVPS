using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battlehub.RTGizmos;

namespace SealTeam4
{
    /// <summary>
    /// Base class for markers
    /// </summary>
    public class BaseMarker : MonoBehaviour, IMarkerBehaviours
    {
        protected float canvasVerticalOffset = 0;

        /// <summary>
        /// Register marker to current active GameManager
        /// </summary>
        /// <param name="markerType"></param>
        protected void RegisterMarkerOnGameManager(GameManager.MARKER_TYPE markerType)
        {
            if (GameManager.instance)
            {
                GameManager.instance.RegisterMarker(
                this.gameObject,
                markerType
                );
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
            Destroy(GetComponent<DynamicBillboard>().canvas.gameObject);
        }

        private void OnDisable()
        {
            GameManager.instance.UnregisterMarker(gameObject);
        }
    }
}
