using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

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
            if (Input.GetKeyDown(KeyCode.Mouse0) && (ReplaySystemCameraScript.instance.MouseActive))
            {
                if (selectedObject)
                {
                    selectedObject.GetComponent<Renderer>().material.shader = Shader.Find("Standard");
                }
                SelectGameObject();
                if (!selectedObject)
                {
                    selectedObjectName.text = "";
                }
                else
                {
                    selectedObjectName.text = selectedObject.name;
                    rend = selectedObject.GetComponent<Renderer>();
                    rend.material.shader = outlineShader;
                    rend.material.SetColor("_OutlineColor", Color.yellow);
                    rend.material.SetFloat("_OutlineWidth", 0.1f);
                }
                
                
            }
        }
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
            // Raycasts to find object for selection
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out hit);
            if (hit.transform)
            {
                selectedObject = hit.transform.gameObject;
            }
            else
            {
                selectedObject = null;
            }

            UpdateActions();
        }

        private void UpdateActions()
        {
            if (selectedObject)
            {
                foreach (GameObject b in actionBtnList)
                {
                    Destroy(b);
                }

                actionBtnList.Clear();
                if (selectedObject.GetComponents<IActions>().Length != 0)
                {
                    actionList = selectedObject.GetComponent<IActions>().GetActions();
                }


                foreach (string action in actionList)
                {
                    GameObject go = Instantiate(actionBtn, actionBtnContainer);

                    go.GetComponentInChildren<Text>().text = action;
                    Debug.Log("0");
                    go.GetComponent<Button>().onClick.AddListener(delegate
                    {
                        OnBtnClick(go.GetComponent<Button>());
                    });
                    Debug.Log("1");

                    actionBtnList.Add(go);
                }
            }

        }

        public void OnBtnClick(Button btn)
        {
            string txt = btn.GetComponentInChildren<Text>().text;
            selectedObject.GetComponent<IActions>().SetActions(txt);
            Debug.Log("BtnClk");

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
