using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Linq;
using TMPro;

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
        
        // Calibration Button variables
        private bool calibrationModeOn;
        private Image calibrationBtnColor;
        
        [Header("Selected Game Object Panel variables")]
        [SerializeField] private GameObject currSelectedGO;
        [SerializeField] private TextMeshProUGUI selectedGOTxt;
        private RaycastHit hit;
        private Ray ray;
        private Shader outlineShader;
        private Renderer rend;
        
        [Header("Player container list spawning variables")]
        [SerializeField] private GameObject prefab;
        [SerializeField] private GameObject playerList_GO;
        [SerializeField] private RectTransform rt;
        private int count;
        private List<GameObject> playerContainerList = new List<GameObject>();

        [Header("Action List spawning variables")]
        [SerializeField] private Transform actionBtnContainer;
        [SerializeField] private GameObject actionBtn_Prefab;
        private List<string> actionList = new List<string>();
        private List<GameObject> actionBtnList = new List<GameObject>();
        
        [Header("Selection Marker Variable")]
        [SerializeField] GameObject markerPrefab;
        private GameObject marker;
        private bool markerActive;

        #endregion

        // Use this for initialization
        void Start()
        {
            // Calibration Button Setup
            calibrationModeOn = false;
            calibrationBtnColor = GameObject.Find("PlayerPosCalibrationBtn").GetComponent<Image>();
            calibrationBtnColor.color = Color.grey;

            // Selection Setup
            outlineShader = Shader.Find("Outlined/Uniform");
        }

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && (ReplaySystemCameraScript.instance.MouseActive) && !EventSystem.current.IsPointerOverGameObject())
            {
                SelectGameObject();

                if (currSelectedGO)
                {
                    selectedGOTxt.text = currSelectedGO.GetComponent<IActions>().GetName();
                    
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
                    selectedGOTxt.text = "Nothing Selected";
                }
            }
            UpdateActionList();
            //if(selectedObject) UpdateMarker();
        }


        private void MarkSelectedObject()
        {
            if (!markerActive)
            {
                marker = Instantiate(markerPrefab, currSelectedGO.transform);
                markerActive = true;
            }

            if (currSelectedGO)
            {
                marker.transform.position = currSelectedGO.transform.position + new Vector3(0, 10, 0);
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
                if (hit.transform && hit.transform.GetComponents<IActions>().Length != 0)
                {
                    currSelectedGO = hit.transform.gameObject;
                }
                else
                {
                    currSelectedGO = null;
                }
            }
        }

        private void UpdateActionList()
        {
            if (currSelectedGO)
            {
                if (currSelectedGO.GetComponents<IActions>().Length != 0)
                {
                    List<string> newActionList = currSelectedGO.GetComponent<IActions>().GetActions();
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
            else
            {
                actionList.Clear();
                UpdateActionListButtons();
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
                    GameObject go = Instantiate(actionBtn_Prefab, actionBtnContainer);

                    go.GetComponentInChildren<TextMeshProUGUI>().text = action;

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
            string txt = btn.GetComponentInChildren<TextMeshProUGUI>().text;

            currSelectedGO.GetComponent<IActions>().SetAction(txt);
        }

        // Adds a new ui prefab to the player list
        public void AddNewPlayer()
        {
            // increases the viewport to account for the new player stats
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, rt.sizeDelta.y + 60);

            // Instantiate a new player UI prefab
            GameObject go = Instantiate(
                prefab,
                playerList_GO.transform.position,
                Quaternion.identity,
                playerList_GO.transform);

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

        public void OnStartButtonClick()
        {
            GameManager.instance.GM_Host_SwitchToRun();
        }
    }

}