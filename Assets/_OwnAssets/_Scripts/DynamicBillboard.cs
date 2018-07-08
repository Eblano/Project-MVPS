using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Battlehub.RTCommon;
using Battlehub.RTSaveLoad;

namespace SealTeam4
{
    [ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllPublic)]
    public class DynamicBillboard : MonoBehaviour
    {
        private WorldSpaceText markerFloatingText;

        [SerializeField] private Color markerColor;
        private Color oldMarkerColor;

        protected Camera camToTrack;
        [Battlehub.SerializeIgnore] [SerializeField] private Transform billboardTransform;

        [Header("Canvas Scaling Parameters")]
        //At what distance do we want to start scaling the UI?
        private readonly float uiScaleStartDist = 0.1f;
        //When do we want to stop scaling the UI?
        private readonly float uiScaleEndDist = 500f;
        //The smallest the UI element can get.
        private readonly float minUiScale = 0.05f;
        //The largest the UI element can get.
        private readonly float maxUiScale = 10f;
        //How slowly you want the UI element to grow. This may be a bit counter-intuitive, but higher is slower.
        private float scaleRate = 35f;
        // For Canvas scaling
        private float distanceFromCamera = 0;
        //The modifier of our UI scale.
        private float uiElementScaleModifier = 0;

        private bool setupDone;

        protected void Start()
        {
            if (GetComponents<IActions>().Length > 0)
            {
                billboardTransform = GetComponents<IActions>()[0].GetHighestPointTransform();
            }
            else
            {
                billboardTransform = transform;
            }

            oldMarkerColor = markerColor;
        }

        protected void LateUpdate()
        {
            if (markerFloatingText && camToTrack)
            {
                if (!setupDone)
                {
                    uiElementScaleModifier = markerFloatingText.transform.localScale.x / 1;
                    setupDone = true;
                }

                UpdateCanvasText();
                RotateCanvasToFaceCamera();
                ScaleCanvas();
                MoveCanvas();
            }
            else
            {
                camToTrack = GameObject.Find("MarkerUICamera(Clone)").GetComponent<Camera>();
            }

            if (RuntimeEditorUltiltes.instance)
            {
                if (RuntimeSelection.activeObject == gameObject)
                {
                    markerFloatingText.gameObject.SetActive(false);
                }
                else
                {
                    markerFloatingText.gameObject.SetActive(true);
                }
            }
            else
            {
                markerFloatingText.gameObject.SetActive(true);
            }

            markerFloatingText.SetBGColor(markerColor);
            if (oldMarkerColor != markerColor)
            {
                markerFloatingText.SetBGColor(markerColor);
                oldMarkerColor = markerColor;
            }
        }

        private void UpdateCanvasText()
        {
            markerFloatingText.SetText(gameObject.name);
        }

        private void ScaleCanvas()
        {
            distanceFromCamera = (markerFloatingText.transform.position - camToTrack.transform.position).magnitude;

            if (distanceFromCamera >= uiScaleStartDist)
            {
                float scaleModifier = (distanceFromCamera - uiScaleStartDist) * (uiElementScaleModifier / scaleRate);

                //Limit our scale so it doesn't continue growing infinitely.
                scaleModifier = Mathf.Clamp(scaleModifier, minUiScale, maxUiScale);

                markerFloatingText.transform.localScale = Vector3.one * scaleModifier;
            }
            else
            {
                //Reset our UI element size.
                markerFloatingText.transform.localScale = Vector3.one * uiElementScaleModifier;
            }
        }

        private void MoveCanvas()
        {
            markerFloatingText.transform.position = billboardTransform.position;
        }

        private void RotateCanvasToFaceCamera()
        {
            Vector3 lookPos = markerFloatingText.transform.position - camToTrack.transform.position;

            if(lookPos != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(lookPos);
                markerFloatingText.transform.rotation = rotation;
            }
        }

        private void OnDestroy()
        {
            //Destroy(markerFloatingText.gameObject);
        }

        private void OnDisable()
        {
            if (markerFloatingText)
                Destroy(markerFloatingText.gameObject);
        }

        private void OnEnable()
        {
            if(!markerFloatingText)
                markerFloatingText = 
                    Instantiate(RuntimeEditorUltiltes.instance.GetMarkerFloatingtTextPrefab(), transform.position, transform.rotation)
                    .GetComponent<WorldSpaceText>();

            markerFloatingText.SetParentDynamicBillboard(this);
        }

        public void SelectThis()
        {
            if(RuntimeEditorUltiltes.instance)
                RuntimeSelection.activeObject = gameObject;
            else if(GetComponents<IActions>().Length != 0)
                InterfaceManager.instance.SelectGameObject(gameObject);
            else
                InterfaceManager.instance.SelectGameObject(null);
        }
    }
}