using Battlehub.RTCommon;
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
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private LineRenderer lineRenderer;

        [SerializeField] private Material markerMat;

        [Header("Marker Indicator Parameters")]
        [SerializeField] private Mesh markerMesh;
        [SerializeField] private float markerScale = 2;

        [Header("Line Renderer Parameters")]
        [SerializeField] private float lineRendererWidth = 0.03f;
        private float directionLineLength = 0.1f;

        //[Header("Target Indicator Parameters")]
        //private GameObject target;
        //[SerializeField] private Mesh targetMesh;
        //[SerializeField] private Vector3 targetMeshScale = new Vector3(0.05f, 0.005f, 0.05f);

        [Header("Read Only")]
        public Vector3 pointPosition;
        public Quaternion pointRotation;

        private bool setupMode = true;

        protected void Start()
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            lineRenderer = gameObject.AddComponent<LineRenderer>();

            meshFilter.mesh = markerMesh;
            transform.localScale = new Vector3(markerScale, markerScale, markerScale);
            meshRenderer.material = markerMat;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.material = markerMat;
            lineRenderer.widthMultiplier = lineRendererWidth;
            lineRenderer.positionCount = 3;

            //target = new GameObject();
            //target.transform.position = transform.position + transform.up * targetMeshScale.y;
            //target.transform.rotation = transform.rotation;
            //target.transform.SetParent(transform);
            //target.AddComponent<MeshFilter>();
            //target.AddComponent<MeshRenderer>();

            //target.GetComponent<MeshFilter>().mesh = targetMesh;
            //target.GetComponent<MeshRenderer>().material = markerMat;
            //target.transform.localScale = targetMeshScale;
        }

        private void Update()
        {
            if(setupMode)
            {
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

                RaycastHit hitInfo;
                lineRenderer.SetPosition(0, transform.position);

                if (Physics.Raycast(transform.position, -transform.up, out hitInfo, 100))
                {
                    lineRenderer.SetPosition(1, hitInfo.point);
                    lineRenderer.SetPosition(2, hitInfo.point + transform.forward * directionLineLength);
                    pointPosition = hitInfo.point;
                    pointRotation = transform.rotation;
                    //target.SetActive(true);
                    //target.transform.position = hitInfo.point;
                }
                else
                {
                    lineRenderer.SetPosition(1, transform.position);
                    lineRenderer.SetPosition(2, transform.position);
                    //target.SetActive(false);
                    pointPosition = Vector3.zero;
                    pointRotation = Quaternion.identity;
                }
            }
        }

        public override void CleanUpForSimulationStart()
        {
            Destroy(meshFilter);
            Destroy(meshRenderer);
            Destroy(lineRenderer);
            setupMode = false;
        }
    }
}