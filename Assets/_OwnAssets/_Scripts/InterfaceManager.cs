using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        public static InterfaceManager instance;

        [SerializeField] private GameObject camPrefab;
        [SerializeField] private Camera cam;

        [Header("Selected Game Object Panel variables")]
        [SerializeField] private GameObject currSelectedGO;
        [SerializeField] private TextMeshProUGUI selectedGOTxt;
        private RaycastHit hit;
        private Ray ray;
        private Renderer rend;

        [Header("Action List spawning variables")]
        [SerializeField] private Transform actionBtnContainer;
        [SerializeField] private GameObject actionBtn_Prefab;
        private List<string> actionList = new List<string>();
        private List<Button> actionBtnList = new List<Button>();

        [Header("Selection Marker Variable")]
        [SerializeField] GameObject markerPrefab;
        private GameObject marker;
        private bool markerActive;
        private Vector3 pos;
        private Vector3 screenPos;
        [SerializeField] private GameObject borderUiPrefab;
        private GameObject borderUI;
        private List<GameObject> markerList = new List<GameObject>();
        private bool inView;
        private Plane[] planes;
        private Collider col;

        [Header("MarkerUI Camera Properties")]
        private Camera markerUICamera;
        private GameObject camToFollow;

        [Header("Buttons")]
        [SerializeField] private Button startButton;
        [SerializeField] private Button spawnNPCAccessoriesButton;
        [SerializeField] private Button showHideWallsCeillingBtn;
        [SerializeField] private Button showHideMarkersBtn;
        [SerializeField] private Button backToEditorBtn;
        private bool hideWallsCeillings = false;
        private bool hideMarkers = false;

        [Header("More Info Panel Properties")]
        [SerializeField] private GameObject objectInfoPanel;
        [SerializeField] private GameObject objectInfoSlot_Prefab;
        [SerializeField] private GameObject objectInfoScrollPlane;
        [SerializeField] private List<GameObject> currActiveObjectInfoSlots = new List<GameObject>();
        private IObjectInfo currSelObjInfo;
        private readonly float objectInfoRefRate = 1f;
        private float objectInfoTimeToRefresh = 0;

        [Header("Player Info Properties")]
        [SerializeField] private GameObject playersPanel;
        [SerializeField] private GameObject playerContainer_Prefab;
        private List<PlayerContainer> currActivePlayerContainers = new List<PlayerContainer>();

        [Space(10)]

        [SerializeField] private TextMeshProUGUI gameTime;
        private float currGameTime = 0;

        private bool spawnedNPCAndAcc = false;
        private bool gameStarted = false;

        // Use this for initialization
        void Start()
        {
            if (!instance)
                instance = this;
            else
            {
                Debug.Log("Interface Manager already exists");
            }
            
            cam = Instantiate(camPrefab).GetComponentInChildren<Camera>();

            // Selection Setup
            borderUI = Instantiate(borderUiPrefab, this.GetComponent<Canvas>().transform);
            borderUI.SetActive(false);

            GameObject.Find("AdminCam").transform.rotation = Quaternion.identity;

            // Setup MarkerUICamera
            markerUICamera = GameObject.Find("MarkerUICamera(Clone)").GetComponent<Camera>();
            markerUICamera.transform.SetParent(cam.gameObject.transform);
            markerUICamera.transform.localPosition = new Vector3(0, 0, 0);
            markerUICamera.transform.rotation = Quaternion.identity;

            //Suscribe buttons to listerners
            startButton.onClick.AddListener(delegate { OnStartGameButtonClick(); });
            spawnNPCAccessoriesButton.onClick.AddListener(delegate { OnSpawnNPCsAccButtonClick(); });
            showHideWallsCeillingBtn.onClick.AddListener(delegate { OnShowHideWallsCeillingBtn(); });
            showHideMarkersBtn.onClick.AddListener(delegate { OnShowHideMarkersBtn(); });
            backToEditorBtn.onClick.AddListener(delegate { OnBackToEditorBtn(); });
        }

        // Update is called once per frame
        private void Update()
        {
            if (
                Input.GetKeyDown(KeyCode.Mouse0) && 
                (ReplaySystemCameraScript.instance.MouseActive) && 
                !EventSystem.current.IsPointerOverGameObject()
                )
            {
                TryToSelectGameObject();
            }

            if (gameStarted)
                UpdateGameTime();

            if (currSelectedGO)
            {
                selectedGOTxt.text = currSelectedGO.GetComponent<IActions>().GetName();
                currSelObjInfo = currSelectedGO.GetComponent<IObjectInfo>();

                if (currSelObjInfo != null)
                {
                    if(Input.GetKeyDown(KeyCode.LeftShift))
                    {
                        objectInfoPanel.SetActive(true);
                        RefreshObjectInfoPanels(currSelObjInfo.GetObjectInfos());
                    }

                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        if (objectInfoTimeToRefresh < 0)
                        {
                            objectInfoPanel.SetActive(true);
                            RefreshObjectInfoPanels(currSelObjInfo.GetObjectInfos());
                            objectInfoTimeToRefresh = objectInfoRefRate;
                        }
                        else
                            objectInfoTimeToRefresh -= Time.deltaTime;
                    }
                    else
                    {
                        objectInfoPanel.SetActive(false);
                    }
                }
                else
                {
                    currSelObjInfo = null;
                    objectInfoPanel.SetActive(false);
                }
            }
            else
                selectedGOTxt.text = "Nothing Selected";

            UpdateMarker();
            UpdateActionList();
            ListenForKeys();
        }

        private void UpdateGameTime()
        {
            currGameTime += Time.deltaTime;
            gameTime.text = string.Format("{0:00}:{1:00}", (currGameTime / 60) % 60, currGameTime % 60);
        }

        public void UnToggleAllPlayerContainerVIPFollowTarget(PlayerContainer exclude)
        {
            foreach(PlayerContainer player in currActivePlayerContainers)
            {
                if (player != exclude)
                    player.SetVIPFollowButtonState(false);
            }
        }

        private void RefreshObjectInfoPanels(List<ObjectInfo> objInfos)
        {
            foreach (GameObject slot in currActiveObjectInfoSlots)
            {
                Destroy(slot);
            }
            currActiveObjectInfoSlots.Clear();

            foreach (ObjectInfo objInfo in objInfos)
            {
                ObjectInfoSlot objectInfoSlot = 
                    Instantiate(objectInfoSlot_Prefab, objectInfoScrollPlane.transform).GetComponent<ObjectInfoSlot>();

                string content = "";
                for (int i = 0; i < objInfo.content.Count; i++)
                {
                    if (i != objInfo.contentIndexToHighlight)
                        content += objInfo.content[i] + "\n";
                    else
                        content += "<#90CAF9>" + objInfo.content[i] + "</color>\n";
                }

                objectInfoSlot.UpdateContent(objInfo.title, content);
                currActiveObjectInfoSlots.Add(objectInfoSlot.gameObject);
            }

            Canvas.ForceUpdateCanvases();
        }

        private void TryToSelectGameObject()
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                // Raycasts to find object for selection
                ray = cam.ScreenPointToRay(Input.mousePosition);

                int ignoreLayer = ~(1 << LayerMask.NameToLayer("UI"));

                Physics.Raycast(ray, out hit, Mathf.Infinity, ignoreLayer);

                if (hit.transform)
                {
                    if(hit.transform.GetComponents<IActions>().Length != 0)
                    {
                        currSelectedGO = hit.transform.gameObject;
                        col = currSelectedGO.GetComponent<IActions>().GetCollider();
                        DrawObjectMarker();
                    }
                }
                else
                {
                    currSelectedGO = null;
                }
            }
        }

        public void SelectGameObject(GameObject go)
        {
            currSelectedGO = go;

            if(currSelectedGO)
            {
                col = currSelectedGO.GetComponent<IActions>().GetCollider();
                DrawObjectMarker();
            }
        }

        private void DrawObjectMarker()
        {
            pos = currSelectedGO.GetComponentInChildren<IActions>().GetHighestPointPos();
            screenPos = cam.WorldToScreenPoint(pos);
            if (currSelectedGO)
            {
                if (markerList.Count != 0)
                {
                    foreach (GameObject g in markerList)
                    {
                        Destroy(g.gameObject);
                    }
                    markerList.Clear();
                }
                borderUI.SetActive(false);
                GameObject m = Instantiate(markerPrefab, screenPos, Quaternion.identity, this.GetComponent<Canvas>().transform);
                markerList.Add(m);
            }
        }

        private void UpdateMarker()
        {
            if (markerList.Count == 0)
            {
                return;
            }

            if (currSelectedGO)
            {
                planes = GeometryUtility.CalculateFrustumPlanes(cam);
                inView = GeometryUtility.TestPlanesAABB(planes, col.bounds);

                if (!inView /*screenPos.z < 0*/)
                {
                    borderUI.SetActive(true);
                    markerList.First().SetActive(false);
                }
                else
                {
                    borderUI.SetActive(false);
                    markerList.First().gameObject.SetActive(true);
                    pos = currSelectedGO.GetComponentInChildren<IActions>().GetHighestPointPos();
                    screenPos = cam.WorldToScreenPoint(pos);
                    markerList.First().transform.position = screenPos;
                }
            }
            else
            {
                Destroy(markerList.First().gameObject);
                markerList.Clear();
            }
        }

        private void ListenForKeys()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && actionBtnList.Count > 0)
                actionBtnList[0].onClick.Invoke();

            if (Input.GetKeyDown(KeyCode.Alpha2) && actionBtnList.Count > 1)
                actionBtnList[1].onClick.Invoke();

            if (Input.GetKeyDown(KeyCode.Alpha3) && actionBtnList.Count > 2)
                actionBtnList[2].onClick.Invoke();

            if (Input.GetKeyDown(KeyCode.Alpha4) && actionBtnList.Count > 3)
                actionBtnList[3].onClick.Invoke();

            if (Input.GetKeyDown(KeyCode.Alpha5) && actionBtnList.Count > 4)
                actionBtnList[4].onClick.Invoke();

            if (Input.GetKeyDown(KeyCode.Alpha6) && actionBtnList.Count > 5)
                actionBtnList[5].onClick.Invoke();

            if (Input.GetKeyDown(KeyCode.Alpha7) && actionBtnList.Count > 6)
                actionBtnList[6].onClick.Invoke();

            if (Input.GetKeyDown(KeyCode.Alpha8) && actionBtnList.Count > 7)
                actionBtnList[7].onClick.Invoke();

            if (Input.GetKeyDown(KeyCode.Alpha9) && actionBtnList.Count > 8)
                actionBtnList[8].onClick.Invoke();

            if (Input.GetKeyDown(KeyCode.Alpha0) && actionBtnList.Count > 9)
                actionBtnList[9].onClick.Invoke();

            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.Z))
            {
                if (Input.GetKeyDown(KeyCode.E))
                    showHideMarkersBtn.onClick.Invoke();

                if (Input.GetKeyDown(KeyCode.R))
                    showHideWallsCeillingBtn.onClick.Invoke();
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
                selectedGOTxt.text = "Nothing Selected";
                actionList.Clear();
                UpdateActionListButtons();
            }
        }

        public void UpdateActionListButtons()
        {
            foreach (Button btn in actionBtnList)
            {
                Destroy(btn.gameObject);
            }
            actionBtnList.Clear();

            if (actionList != null && actionList.Count > 0)
            {
                for (int i = 0; i < actionList.Count; i++)
                {
                    Button actionBtn = Instantiate(actionBtn_Prefab, actionBtnContainer).GetComponent<Button>();

                    actionBtn.GetComponentInChildren<TextMeshProUGUI>().text = (i + 1) + " - " + actionList[i];

                    actionBtn.onClick.AddListener(delegate
                    {
                        OnBtnClick(actionBtn);
                    });

                    actionBtnList.Add(actionBtn);
                }
            }
        }

        public void OnBtnClick(Button btn)
        {
            string text = btn.GetComponentInChildren<TextMeshProUGUI>().text.Substring(4);
            SendAction(text);
        }

        private void SendAction(string text)
        {
            currSelectedGO.GetComponent<IActions>().SetAction(text);
        }
        
        public void AddNewPlayer(string playerName)
        {
            PlayerContainer newPlayerContainer = Instantiate(playerContainer_Prefab, playersPanel.transform).GetComponent<PlayerContainer>();
            newPlayerContainer.Setup(playerName);
            currActivePlayerContainers.Add(newPlayerContainer);

            if(currActivePlayerContainers.Count == 1)
            {
                currActivePlayerContainers[0].SetVIPFollowButtonState(true);
            }
        }

        public void RemovePlayer(string playerName)
        {
            Destroy(currActivePlayerContainers.Find(x => x.playerNameTxt.text == playerName).gameObject);
            
            if(currActivePlayerContainers.Count > 0)
                currActivePlayerContainers[0].SetVIPFollowButtonState(true);
        }

        private void OnStartGameButtonClick()
        {
            if(!spawnedNPCAndAcc)
                OnSpawnNPCsAccButtonClick();

            GameManager.instance.GM_Host_SwitchToRun();
            Destroy(startButton.gameObject);

            gameStarted = true;
        }

        private void OnSpawnNPCsAccButtonClick()
        {
            GameManager.instance.SpawnAndSetupNPC();
            GameManager.instance.SpawnAccessories();
            Destroy(spawnNPCAccessoriesButton.gameObject);

            spawnedNPCAndAcc = true;
        }
        
        private void OnShowHideWallsCeillingBtn()
        {
            if(hideWallsCeillings)
                cam.cullingMask = cam.cullingMask | (1 << LayerMask.NameToLayer("Walls")) | (1 << LayerMask.NameToLayer("Ceilling"));
            else
                cam.cullingMask = cam.cullingMask & ~(1 << LayerMask.NameToLayer("Walls")) & ~(1 << LayerMask.NameToLayer("Ceilling"));

            hideWallsCeillings = !hideWallsCeillings;
        }

        private void OnShowHideMarkersBtn()
        {
            if (hideMarkers)
            {
                markerUICamera.cullingMask =
                    markerUICamera.cullingMask |
                    (1 << LayerMask.NameToLayer("Marker")) |
                    (1 << LayerMask.NameToLayer("FloatingUI"));

                cam.cullingMask = cam.cullingMask | (1 << LayerMask.NameToLayer("AreaMarker"));
            }
            else
            {
                markerUICamera.cullingMask =
                    markerUICamera.cullingMask &
                    ~(1 << LayerMask.NameToLayer("Marker")) &
                    ~(1 << LayerMask.NameToLayer("FloatingUI"));

                cam.cullingMask = cam.cullingMask & ~(1 << LayerMask.NameToLayer("AreaMarker"));
            }

            hideMarkers = !hideMarkers;
        }

        private void OnBackToEditorBtn()
        {
            GameManager.instance.RestartScene();
        }
    }
}