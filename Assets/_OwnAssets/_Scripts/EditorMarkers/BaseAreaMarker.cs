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
    public class BaseAreaMarker : BaseMarker, IActions
    {
        [SerializeField] private Mesh mesh;
        [SerializeField] private Material meshMat;

        private Transform highestPoint;

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

        public List<string> GetActions()
        {
            throw new NotImplementedException();
        }

        public void SetAction(string action)
        {
            throw new NotImplementedException();
        }

        public string GetName()
        {
            return gameObject.name;
        }

        public Vector3 GetHighestPointPos()
        {
            return highestPoint.position;
        }

        public Transform GetHighestPointTransform()
        {
            return highestPoint;
        }

        public Collider GetCollider()
        {
            throw new NotImplementedException();
        }
    }
}