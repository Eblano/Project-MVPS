using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    public class Ruler : MonoBehaviour
    {
        private LineRenderer lineR;
        [Battlehub.SerializeIgnore] [SerializeField] private Material lineRMaterial;
        private Ruler connectedRuler;

        [SerializeField] private string endName = "";
        private string prevEndName;

        [Space(10)]

        private bool isRulerHead;
        private bool isRulerEnd;
        [SerializeField] private bool isRulerHead_RO;
        [SerializeField] private bool isRulerEnd_RO;

        private float distToEnd;
        [SerializeField] private float distToEnd_RO;

        private void Start()
        {
            prevEndName = endName;
        }

        private void Update()
        {
            if (endName != prevEndName)
                UpdateConnection();

            UpdateLineRenderer();
            UpdateDistanceValue();
            EnforceReadOnlyValues();
        }

        private void AttemptToConnectToEndRuler()
        {
            if (endName != gameObject.name && GameObject.Find(endName) && GameObject.Find(endName).GetComponent<Ruler>())
                connectedRuler = GameObject.Find(endName).GetComponent<Ruler>();
            else
            {
                connectedRuler = null;
                return;
            }

            SetRulerHead(true);
            connectedRuler.SetRulerEnd(true);
        }

        private void AttemptToDisconnectFromEndRuler()
        {
            if(connectedRuler)
                connectedRuler.SetRulerEnd(false);

            SetRulerHead(false);
            connectedRuler = null;
        }

        private void UpdateConnection()
        {
            if (!isRulerEnd)
            {
                if (connectedRuler)
                    AttemptToDisconnectFromEndRuler();

                AttemptToConnectToEndRuler();
                prevEndName = endName;
            }
            else
                endName = prevEndName;
        }

        private void UpdateDistanceValue()
        {
            if (connectedRuler)
                distToEnd = float.Parse((connectedRuler.transform.position - transform.position).magnitude.ToString("n2"));
            else
                distToEnd = 0;
        }

        private void UpdateLineRenderer()
        {
            lineR.SetPosition(0, transform.position);

            if (connectedRuler && connectedRuler.gameObject.activeInHierarchy)
                lineR.SetPosition(1, connectedRuler.transform.position);
            else
                lineR.SetPosition(1, transform.position);
        }

        public void SetRulerHead(bool value)
        {
            isRulerHead = value;
        }

        public void SetRulerEnd(bool value)
        {
            isRulerEnd = value;
        }

        public void EnforceReadOnlyValues()
        {
            isRulerHead_RO = isRulerHead;
            isRulerEnd_RO = isRulerEnd;
            distToEnd_RO = distToEnd;
        }

        private void InitiateLineRenderer()
        {
            if (GetComponent<LineRenderer>())
                lineR = GetComponent<LineRenderer>();
            else
                lineR = gameObject.AddComponent<LineRenderer>();

            lineR.positionCount = 2;
            lineR.SetPosition(0, transform.position);
            lineR.SetPosition(1, transform.position);
            lineR.material = lineRMaterial;
            lineR.widthMultiplier = 0.02f;
        }

        private void OnDisable()
        {
            AttemptToDisconnectFromEndRuler();
            Destroy(lineR);
        }

        private void OnEnable()
        {
            UpdateConnection();
            InitiateLineRenderer();
        }
    }
}
