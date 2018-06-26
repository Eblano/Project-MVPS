using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SealTeam4
{
    public class DynamicBillboard : MonoBehaviour
    {
        private GameObject canvas;
        private TextMeshProUGUI canvasText;

        protected Transform camToTrack;

        [Header("Canvas Scaling Parameters")]
        //At what distance do we want to start scaling the UI?
        private float uiScaleStartDist = 0.01f;
        //When do we want to stop scaling the UI?
        private float uiScaleEndDist = 200;
        //The smallest the UI element can get.
        private float minUiScale = 0.1f;
        //The largest the UI element can get.
        private float maxUiScale = 0.3f;
        //How slowly you want the UI element to grow. This may be a bit counter-intuitive, but higher is slower.
        private float scaleRate = 10;
        // For Canvas scaling
        private float distanceFromCamera = 0;
        //The modifier of our UI scale.
        private float uiElementScaleModifier = 0;

        private bool setupDone;

        protected void Start()
        {
            camToTrack = GameObject.Find("Editor Camera").transform;
        }

        protected void LateUpdate()
        {
            if(canvas && camToTrack)
            {
                if(!setupDone)
                {
                    uiElementScaleModifier = canvas.transform.localScale.x / 1;
                    setupDone = true;
                }
                
                if (!canvasText)
                    canvasText = canvas.GetComponentInChildren<TextMeshProUGUI>();
                else
                    UpdateCanvasText();

                RotateCanvasToFaceCamera();
                ScaleCanvas();
                MoveCanvas();
            }
        }

        private void UpdateCanvasText()
        {
            canvasText.text = gameObject.name;
        }

        private void ScaleCanvas()
        {
            distanceFromCamera = (canvas.transform.position - camToTrack.position).magnitude;

            if (distanceFromCamera >= uiScaleStartDist)
            {
                float scaleModifier = (distanceFromCamera - uiScaleStartDist) * (uiElementScaleModifier / scaleRate);

                //Limit our scale so it doesn't continue growing infinitely.
                scaleModifier = Mathf.Clamp(scaleModifier, minUiScale, maxUiScale);

                canvas.transform.localScale = Vector3.one * scaleModifier;
            }
            else
            {
                //Reset our UI element size.
                canvas.transform.localScale = Vector3.one * uiElementScaleModifier;
            }
        }

        private void MoveCanvas()
        {
            canvas.transform.position = transform.position;
        }

        private void RotateCanvasToFaceCamera()
        {
            Vector3 lookPos = canvas.transform.position - camToTrack.position;
            Quaternion rotation = Quaternion.LookRotation(lookPos);

            canvas.transform.rotation = rotation;
        }

        public GameObject GetCanvas()
        {
            return canvas;
        }

        private void OnDestroy()
        {
            Destroy(canvas.gameObject);
        }

        private void OnDisable()
        {
            if (canvas)
                Destroy(canvas.gameObject);
        }

        private void OnEnable()
        {
            if(!canvas)
                canvas = Instantiate(RuntimeEditorUltiltes.instance.GetMarkerFloatingtTextPrefab(), transform.position, transform.rotation);
        }
    }
}