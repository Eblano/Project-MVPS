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

        protected void Start()
        {
            RegisterMarkerOnGameManager(GameManager.MARKER_TYPE.AREA);
            InitializeMeshAndMaterial();
        }

        private void InitializeMeshAndMaterial()
        {
            gameObject.AddComponent<MeshFilter>().mesh = mesh;
            gameObject.AddComponent<MeshRenderer>().material = meshMat;
        }

        public override void CleanUpForSimulationStart()
        {
            Destroy(gameObject.GetComponent<MeshFilter>());
            Destroy(gameObject.GetComponent<MeshRenderer>());
        }
    }
}