using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Linq;
using SealTeam4;

namespace SealTeam4
{
    /// <summary>
    /// 
    /// Author: Clement Leow, SealTeam4
    /// Project: St Michael
    /// 
    /// This script handles the UI interactions for the Game Master Interface
    /// </summary>
    public class InterfaceManager : MonoBehaviour
    {
        #region Variables

        #region Calibration Button variables
        private bool calibrationModeOn;
        private Image calibrationBtnColor;
        #endregion

        #region Selected Game Object Panel variables
        [SerializeField] private GameObject selectedObject;
        private Text selectedObjectName;
        private RaycastHit hit;
        private Ray ray;
        private Shader outlineShader;
        private Renderer rend;

        #endregion

        #region Player container list spawning variables
        private int count;
        [SerializeField] private GameObject prefab;
        [SerializeField] private GameObject playerListUI;
        private List<GameObject> playerContainerList = new List<GameObject>();
        [SerializeField] private RectTransform rt;
        #endregion

        #region Action List spawning variables
        private int btnCount;
        [SerializeField] private Transform actionBtnContainer;
        [SerializeField] private GameObject actionBtn;
        [SerializeField] private List<string> actionList = new List<string>();
        [SerializeField] private List<GameObject> actionBtnList = new List<GameObject>();
        #endregion

        #region Selection Marker Variable
        [SerializeField] GameObject markerPrefab;
        private GameObject marker;
        private bool markerActive;
        #endregion

        #endregion

        // Use this for initialization
        void Start()
        {
            // Calibration Button Setup
            calibrationModeOn = false;
            calibrationBtnColor = GameObject.Find("PlayerPosCalibrationBtn").GetComponent<Image>();
            calibrationBtnColor.color = Color.grey;

            // Selection Setup
            selectedObjectName = GameObject.Find("Selected Object Name").GetComponent<Text>();
            outlineShader = Shader.Find("Outlined/Uniform");
        }

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && (ReplaySystemCameraScript.instance.MouseActive) && !EventSystem.current.IsPointerOverGameObject())
            {
                SelectGameObject();

                if (selectedObject)
                {
                    selectedObjectName.text = selectedObject.name;


                    //Renderer selectedObjRenderer = selectedObject.GetComponent<Renderer>();

                    //if(selectedObjRenderer)
                    //{
                    //    selectedObjRenderer.material.shader = Shader.Find("Standard");

                    //    selectedObjectName.text = selectedObject.name;
                    //    rend = selectedObject.GetComponent<Renderer>();
                    //    rend.material.shader = outlineShader;
                    //    rend.material.SetColor("_OutlineColor", Color.yellow);
                    //    rend.material.SetFloat("_OutlineWidth", 0.1f);
                    //}
                }
                else
                {
                    selectedObjectName.text = "";
                }
            }
            UpdateActionList();
            //if(selectedObject) UpdateMarker();
        }


        private void MarkSelectedObject()
        {
            if (!markerActive)
            {
                marker = Instantiate(markerPrefab, selectedObject.transform);
                markerActive = true;
            }
            

            if (selectedObject)
            {
                marker.transform.position = selectedObject.transform.position + new Vector3(0, 10, 0);
            }
            else if(markerActive)
            {
                Destroy(marker.gameObject);
            }
        }


        //private void UpdateMarker()
        //{
        //    marker.transform.position = selectedObject.transform.position + new Vector3(0, 10, 0);
        //}
    
        // Toggles the position Buttons on and off
        public void ToggleCalibration()
        {
            if (!calibrationModeOn)
            {
                calibrationModeOn = true;
                calibrationBtnColor.color = Color.cyan;

            }
            else
            {
                calibrationModeOn = false;
                calibrationBtnColor.color = Color.grey;
            }

            foreach (GameObject playerContainer in playerContainerList)
            {
                playerContainer.GetComponent<playerUIContainer>().SetButtonStates(calibrationModeOn);
            }
        }

        // Gets an object and puts it in focus
        private void SelectGameObject()
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                // Raycasts to find object for selection
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Physics.Raycast(ray, out hit);
                if (hit.transform)
                {
                    selectedObject = hit.transform.gameObject;
                }
            }
        }

        private void UpdateActionList()
        {
            if (selectedObject)
            {
                if (selectedObject.GetComponents<IActions>().Length != 0)
                {
                    List<string> newActionList = selectedObject.GetComponent<IActions>().GetActions();
                    if (actionList.SequenceEqual(newActionList))
                    {
                        return;
                    }
                    else
                    {
                        actionList = new List<string>(newActionList);
                        UpdateActionListButtons();
                    }
                }
            }
        }

        public void UpdateActionListButtons()
        {
            foreach (GameObject b in actionBtnList)
            {
                Destroy(b);
            }

            foreach (GameObject btn in actionBtnList)
            {
                Destroy(btn.gameObject);
            }
            actionBtnList.Clear();

            if (actionList != null && actionList.Count > 0)
            {
                foreach (string action in actionList)
                {
                    GameObject go = Instantiate(actionBtn, actionBtnContainer);

                    go.GetComponentInChildren<Text>().text = action;

                    go.GetComponent<Button>().onClick.AddListener(delegate
                    {
                        OnBtnClick(go.GetComponent<Button>());
                    });

                    actionBtnList.Add(go);
                }
            }
        }

        public void OnBtnClick(Button btn)
        {
            string txt = btn.GetComponentInChildren<Text>().text;
            Debug.Log("BtnClk " + btn.name + " on object " + selectedObject.name + " set action " + txt);

            selectedObject.GetComponent<IActions>().SetAction(txt);
        }

        // Adds a new ui prefab to the player list
        public void AddNewPlayer()
        {
            // increases the viewport to account for the new player stats
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, rt.sizeDelta.y + 60);

            // Instantiate a new player UI prefab
            GameObject go = Instantiate(
                prefab,
                playerListUI.transform.position,
                Quaternion.identity,
                playerListUI.transform);

            // Checks if calibration mode is on, then sets visibility accordingly
            go.GetComponent<playerUIContainer>().SetButtonStates(calibrationModeOn);
            playerContainerList.Add(go);
        }

        // Removes a specific ui prefab to the player list
        public void RemoveExistingPlayer()
        {
            // decreases the viewport to account for removal of one row of stats
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, rt.sizeDelta.y - 60);
        }
    }

}