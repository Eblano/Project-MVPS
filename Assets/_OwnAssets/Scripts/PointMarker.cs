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
        [SerializeField] private int segments = 20;
        [SerializeField] private float xradius = 0.3f;
        [SerializeField] private float zradius = 0.3f;
        [SerializeField] private float thickness = 0.2f;
        private LineRenderer line;
        [SerializeField] protected Material lineRendererMaterial;

        protected void Start()
        {
            if(!GetComponent<LineRenderer>())
                gameObject.AddComponent<LineRenderer>();

            line = gameObject.GetComponent<LineRenderer>();
            CreateCircularIndicator();
        }

        /// <summary>
        /// Creates a circle around the transform of the object using line renderer
        /// </summary>
        private void CreateCircularIndicator()
        {
            line.positionCount = segments + 3;
            line.useWorldSpace = false;
            line.widthMultiplier = thickness;
            line.material = lineRendererMaterial;

            float x;
            float y = 0f;
            float z;

            float angle = 20f;

            for (int i = 0; i < (segments + 1); i++)
            {
                x = Mathf.Sin(Mathf.Deg2Rad * angle) * xradius;
                z = Mathf.Cos(Mathf.Deg2Rad * angle) * zradius;

                line.SetPosition(i, new Vector3(x, y, z));

                angle += (360f / segments) + 2;
            }
            x = Mathf.Sin(Mathf.Deg2Rad * 90) * xradius;
            z = Mathf.Cos(Mathf.Deg2Rad * 90) * zradius;

            line.SetPosition(segments + 1, new Vector3(x, y, z));
        }
    }
}