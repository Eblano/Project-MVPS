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
        private LineRenderer lineRenderer;

        [SerializeField] private Material markerMat;

        [Header("Line Renderer Parameters")]
        [SerializeField] private float lineRendererWidth = 0.03f;
        private float directionLineLength = 0.1f;

        [Header("Read Only")]
        public Vector3 pointPosition;
        public Quaternion pointRotation;
        public bool validPoint;

        private bool setupMode = true;

        protected new void Start()
        {
            base.Start();

            if (!GetComponent<LineRenderer>())
                lineRenderer = gameObject.AddComponent<LineRenderer>();
            else
                lineRenderer = GetComponent<LineRenderer>();

            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.material = markerMat;
            lineRenderer.widthMultiplier = lineRendererWidth;
            lineRenderer.positionCount = 2;
        }

        public new virtual void Update()
        {
            base.Update();

            if(setupMode && lineRenderer)
            {
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

                RaycastHit hitInfo;
                lineRenderer.SetPosition(0, transform.position);

                int layer = 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Furniture");

                if (Physics.Raycast(transform.position, -transform.up, out hitInfo, 100, layer))
                {
                    lineRenderer.SetPosition(1, hitInfo.point);
                    pointPosition = hitInfo.point;
                    pointRotation = transform.rotation;
                    validPoint = true;
                }
                else
                {
                    lineRenderer.SetPosition(1, transform.position);

                    pointPosition = Vector3.zero;
                    pointRotation = Quaternion.identity;
                    validPoint = false;
                }
            }
        }

        public Vector3 GetMarkerPoint()
        {
            return pointPosition;
        }

        public override void CleanUpForSimulationStart()
        {
            base.CleanUpForSimulationStart();
            Destroy(GetComponent<MeshFilter>());
            Destroy(GetComponent<MeshRenderer>());
            Destroy(GetComponent<LineRenderer>());
            setupMode = false;
        }
    }
}