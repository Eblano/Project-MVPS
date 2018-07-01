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

        private bool initializedMeshCollider = false;

        protected void Start()
        {
            //gameObject.name = RegisterMarkerOnGameManager(GameManager.MARKER_TYPE.AREA);
            RegisterMarkerOnGameManager(this);
            InitializeMeshAndMaterial();
        }

        public new virtual void Update()
        {
            base.Update();

            if (!initializedMeshCollider)
            {
                initializedMeshCollider = true;

                MeshCollider collider = GetComponent<MeshCollider>();
                if(!collider)
                    collider = gameObject.AddComponent<MeshCollider>();

                collider = GetComponent<MeshCollider>();
                collider.sharedMesh = mesh;
                collider.convex = true;
                collider.isTrigger = true;
            }
        }

        private void InitializeMeshAndMaterial()
        {
            if(!GetComponent<MeshFilter>())
                gameObject.AddComponent<MeshFilter>();
            GetComponent<MeshFilter>().mesh = mesh;

            if (!GetComponent<MeshRenderer>())
                gameObject.AddComponent<MeshRenderer>();
            GetComponent<MeshRenderer>().material = meshMat;
        }

        public override void CleanUpForSimulationStart()
        {
            base.CleanUpForSimulationStart();
            Destroy(GetComponent<MeshFilter>());
            Destroy(GetComponent<MeshRenderer>());
        }
    }
}