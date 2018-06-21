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
        protected DynamicBillboard dynamicBillboard;
        [SerializeField] protected GameObject canvas_Prefab;
        private GameObject canvas;
        protected float canvasVerticalOffset = 0.8f;

        protected void Start()
        {
            if (GetComponent<DynamicBillboard>())
                dynamicBillboard = GetComponent<DynamicBillboard>();
            else
                dynamicBillboard = gameObject.AddComponent<DynamicBillboard>();


            canvas = Instantiate(canvas_Prefab, transform.position, Quaternion.identity);
            canvas.AddComponent<Battlehub.RTSaveLoad.PersistentIgnore>();
            canvas.transform.SetParent(gameObject.transform);
            canvas.transform.localPosition += new Vector3(0, canvasVerticalOffset, 0);
            dynamicBillboard.canvas = canvas;
        }

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
            Destroy(canvas);
            Destroy(dynamicBillboard);
        }

        private void OnDisable()
        {
            GameManager.instance.UnregisterMarker(gameObject);
        }
    }
}
