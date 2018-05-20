using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    /// <summary>
    /// Base class for Point Based Marker
    /// </summary>
    public class PointMarker : BaseMarker
    {
        [SerializeField] private Mesh sphere;
        [SerializeField] private Material sphereMat;

        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;

        protected void Start()
        {
            gameObject.AddComponent<MeshFilter>();
            gameObject.AddComponent<MeshRenderer>();

            meshFilter = gameObject.GetComponent<MeshFilter>();
            meshRenderer = gameObject.GetComponent<MeshRenderer>();

            meshFilter.mesh = sphere;
            meshRenderer.material = sphereMat;
        }

        public override void CleanUpForSimulationStart()
        {
            Destroy(meshFilter);
            Destroy(meshRenderer);
            Destroy(GetComponent<MeshCollider>());
            Destroy(GetComponentInChildren<Light>());
        }
    }
}