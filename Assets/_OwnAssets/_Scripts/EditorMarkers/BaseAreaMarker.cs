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

        protected Transform highestPoint;
        protected MeshCollider mCollider;

        private bool initializedMeshCollider = false;

        protected new void Start()
        {
            base.Start();
            RegisterMarkerOnGameManager(this);
            InitializeMeshAndMaterial();

            highestPoint = new GameObject().transform;
            highestPoint.SetParent(gameObject.transform);
            highestPoint.localPosition = new Vector3(0, 0.5f, 0);
        }

        public new virtual void Update()
        {
            base.Update();

            if (!initializedMeshCollider)
            {
                initializedMeshCollider = true;

                mCollider = GetComponent<MeshCollider>();
                if(!mCollider)
                    mCollider = gameObject.AddComponent<MeshCollider>();

                mCollider = GetComponent<MeshCollider>();
                mCollider.sharedMesh = mesh;
                mCollider.convex = true;
                mCollider.isTrigger = true;
            }
        }

        private void InitializeMeshAndMaterial()
        {
            if(!GetComponent<MeshFilter>())
                gameObject.AddComponent<MeshFilter>();
            GetComponent<MeshFilter>().mesh = mesh;

            if (!GetComponent<MeshRenderer>())
                gameObject.AddComponent<MeshRenderer>();

            MeshRenderer mr = GetComponent<MeshRenderer>();
            mr.material = meshMat;
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        public override void CleanUpForSimulationStart()
        {
            base.CleanUpForSimulationStart();
            Destroy(GetComponent<MeshFilter>());
            Destroy(GetComponent<MeshRenderer>());
        }
    }
}