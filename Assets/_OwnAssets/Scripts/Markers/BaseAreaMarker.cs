using Battlehub.RTGizmos;
using Battlehub.RTSaveLoad;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    /// <summary>
    /// Base class for Area Markers
    /// </summary>
    public class BaseAreaMarker : BaseMarker
    {
        private BoxCollider boxCollider;

        // Toogle visibility of Box Collider Editor
        [SerializeField] [HideInInspector] private bool editCollider = false;
        private bool editCollider_LastState = false;

        // This Box Collider Gizmo
        private BoxColliderGizmo boxColliderGizmo;
        // Box Collider Editor Color
        [SerializeField] private Color color = Color.magenta;
        
        protected void Start()
        {
            RegisterMarkerOnGameManager(GameManager.MARKER_TYPE.AREA);

            // Create Box Colliders
            CreateBoxCollider();

            // Create Box Collider Gizmo
            CreateBoxColliderGizmo();
        }

        /// <summary>
        /// Create Box Collider Gizmo
        /// </summary>
        private void CreateBoxColliderGizmo()
        {
            gameObject.AddComponent<BoxColliderGizmo>();
            boxColliderGizmo = GetComponent<BoxColliderGizmo>();
            boxColliderGizmo.LineColor = color;
            boxColliderGizmo.HandlesColor = color;
            boxColliderGizmo.Target = null;
        }

        /// <summary>
        /// Create Box Collider
        /// </summary>
        private void CreateBoxCollider()
        {
            if (!GetComponent<BoxCollider>())
                gameObject.AddComponent<BoxCollider>();

            boxCollider = GetComponent<BoxCollider>();
            boxCollider.isTrigger = true;
        }

        private void OnDisable()
        {
            GameManager.instance.UnregisterMarker(gameObject);
        }
    }
}