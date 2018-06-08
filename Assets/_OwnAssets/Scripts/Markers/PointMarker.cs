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

        [Header("Target Indicator Parameters")]
        private GameObject target;
        [SerializeField] private GameObject emptyGameObject_Prefab;
        [SerializeField] private Mesh targetMesh;
        [SerializeField] private Vector3 targetMeshScale = new Vector3(0.05f, 0.005f, 0.05f);
        private float directionLineLength = 0.1f;

        public Vector3 pointPosition;
        public Quaternion pointRotation;

        private bool setupMode = true;

        protected void Start()
        {
            gameObject.AddComponent<MeshFilter>();
            gameObject.AddComponent<MeshRenderer>();
            gameObject.AddComponent<LineRenderer>();

            meshFilter = gameObject.GetComponent<MeshFilter>();
            meshRenderer = gameObject.GetComponent<MeshRenderer>();
            lineRenderer = gameObject.GetComponent<LineRenderer>();

            meshFilter.mesh = markerMesh;
            meshRenderer.material = markerMat;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.material = markerMat;
            lineRenderer.widthMultiplier = lineRendererWidth;
            lineRenderer.positionCount = 3;

            target = Instantiate(emptyGameObject_Prefab, transform.position, transform.rotation);
            target.AddComponent<MeshFilter>();
            target.AddComponent<MeshRenderer>();

            target.GetComponent<MeshFilter>().mesh = targetMesh;
            target.GetComponent<MeshRenderer>().material = markerMat;
            target.transform.localScale = targetMeshScale;
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
                    target.SetActive(true);
                    target.transform.position = hitInfo.point;
                }
                else
                {
                    lineRenderer.SetPosition(1, transform.position);
                    lineRenderer.SetPosition(2, transform.position);
                    target.SetActive(false);
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