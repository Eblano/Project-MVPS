using UnityEngine;

namespace SealTeam4
{
    /// <summary>
    /// Stores infomation of a single marker
    /// </summary>
    public class Marker
    {
        public string markerName;
        public GameManager.MARKER_TYPE markerType;
        public GameObject markerGO;

        public Marker(GameObject markerGO, GameManager.MARKER_TYPE markerType)
        {
            this.markerName = markerGO.name;
            this.markerGO = markerGO;
            this.markerType = markerType;
        }
    }
}