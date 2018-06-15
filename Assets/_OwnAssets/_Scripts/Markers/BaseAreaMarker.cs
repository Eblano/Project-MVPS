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
        [SerializeField] private Mesh mesh;
        [SerializeField] private Material meshMat;

        [SerializeField] private bool initializedMeshCollider = false;

        protected void Start()
        {
            RegisterMarkerOnGameManager(GameManager.MARKER_TYPE.AREA);
            InitializeMeshAndMaterial();
        }

        protected void Update()
        {
            if (!initializedMeshCollider && GetComponent<MeshCollider>())
            {
                initializedMeshCollider = true;

                MeshCollider collider = GetComponent<MeshCollider>();
                collider.convex = true;
                collider.isTrigger = true;
            }
        }

        private void InitializeMeshAndMaterial()
        {
            gameObject.AddComponent<MeshFilter>().mesh = mesh;
            gameObject.AddComponent<MeshRenderer>().material = meshMat;
        }

        public override void CleanUpForSimulationStart()
        {
            Destroy(GetComponent<MeshFilter>());
            Destroy(GetComponent<MeshRenderer>());
        }
    }
}