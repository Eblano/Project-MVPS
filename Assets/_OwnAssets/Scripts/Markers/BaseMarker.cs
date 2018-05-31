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
        /// <summary>
        /// Register marker to current active GameManager
        /// </summary>
        /// <param name="markerType"></param>
        protected void RegisterMarkerOnGameManager(GameManager.MARKER_TYPE markerType)
        {
            if (GameManager.localInstance)
            {
                GameManager.localInstance.RegisterMarker(
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
        public virtual void CleanUpForSimulationStart() { }
    }
}
