using System.Collections.Generic;
using UnityEngine;
using SealTeam4;
using Battlehub.RTSaveLoad;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using System.Linq;

namespace SealTeam4
{
    /// <summary>
    /// Game Manager
    /// Central control of everything
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        // Scene Info for loaded scene
        private string sceneName;
        private string sceneHash;

        // Instance of the Game Manager
        public static GameManager instance;

        // GameManager Modes
        private enum GameManagerMode { LEVELSETUP, HOST, CLIENT }
        [SerializeField] private GameManagerMode currGameManagerMode = GameManagerMode.LEVELSETUP;

        // GameManager Host Modes
        private enum GameManagerHostMode { IDLE, RUN }
        [SerializeField] private GameManagerHostMode currGameManagerHostMode = GameManagerHostMode.IDLE;

        // GameManager Client Modes
        private enum GameManagerClientMode { IDLE, RUN }
        [SerializeField] private GameManagerClientMode currGameManagerClientMode = GameManagerClientMode.IDLE;

        [Header("Essentials")]
        [SerializeField] private GameObject navMeshSurfaceInitator_Prefab;
        [SerializeField] private GameObject localPlayerController_Prefab;
        [SerializeField] private GameObject gameManagerAssistant_Prefab;

        [Header("Admin Components")]
        [SerializeField] private GameObject gameMasterCamera_Prefab;
        [SerializeField] private GameObject gameMasterUI_Prefab;

        public enum MARKER_TYPE { AREA, WAYPOINT, NPCSPAWN, SEAT, PLAYER_SPAWN_MARKER, EXIT };

        [Header("NPC Prefabs")]
        [SerializeField] private GameObject type0NPC_Prefab;
        [SerializeField] private GameObject type1NPC_Prefab;

        // List of markers GameManager keeps track of
        [Header("Registered Markers Counter")]
        [SerializeField] private int totalRegMarkers;
        private List<BaseMarker> registeredMarkers = new List<BaseMarker>();
        [SerializeField] private float refreshRate = 3.0f;
        private float currRefreshRate;

        [Header("Game Condition")]
        public bool areaUnderAttack;
        
        public bool isHost = false;

        [Space(10)]

        // NPC List
        [SerializeField] private List<NpcSpawnData> npcSpawnDataList = new List<NpcSpawnData>();
        private List<GameObject> spawnedCivilianNPCs = new List<GameObject>();
        private List<GameObject> spawnedVIPNPC = new List<GameObject>();
        private List<GameObject> spawnedHostileNPCs = new List<GameObject>();

        //// For calibration of in game position from physical position
        //public bool calibrationMode = false;

        [Space(10)]

        public string localPlayerName;

        [Space(10)]

        public bool networkTest = false;

        //public List<PlayerVectorCalibData> playerVectorCalibDataList = new List<PlayerVectorCalibData>();

        private void Start()
        {
            if (instance == null)
                instance = this;
        }

        private void OnDisable()
        {
            if (registeredMarkers.Count > 0) registeredMarkers.Clear();
        }

        // ************
        // Update Methods
        // ************
        #region Update Methods
        private void Update()
        {
            switch (currGameManagerMode)
            {
                case GameManagerMode.LEVELSETUP:
                    LevelSetup_Update();
                    break;
                case GameManagerMode.HOST:
                    Host_Update();
                    break;
                case GameManagerMode.CLIENT:
                    Client_Update();
                    break;
            }

            //if(Input.GetKeyDown(KeyCode.Z))
            //{
            //    GameManagerAssistant.instance.CmdSetBool(networkTest);
            //}
        }

        public void UpdateNetworkTestBool(bool value)
        {
            networkTest = value;
            Debug.Log(networkTest);
        }

        private void Host_Update()
        {
            if(currGameManagerHostMode == GameManagerHostMode.RUN)
            {
                Host_Run_Update();
            }
        }
        
        private void Client_Update()
        {

        }

        private void LevelSetup_Update()
        {
            UpdateRegisteredMarkers();
        }

        private void Host_Run_Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                //NetworkPlayerPosManager.localInstance.RpcCalibratePlayerVector();
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                areaUnderAttack = true;
            }
        }
        #endregion

        // ************
        // Switching Methods
        // ************
        #region Switching Methods
        public void GM_SwitchToHostMode()
        {
            currGameManagerMode = GameManagerMode.HOST;

            Destroy(Camera.main.gameObject);

            // Set NPCSpawnData
            SetNPCSpawnDataFromNPCScriptStorage();

            // Spawn and Build NavMesh
            NavMeshSurface nmSurface = Instantiate(navMeshSurfaceInitator_Prefab, Vector3.zero, Quaternion.identity).GetComponent<NavMeshSurface>();
            nmSurface.BuildNavMesh();

            Host_SetupMarkers();
            isHost = true;

            // Instantiate admin cam
            Instantiate(gameMasterCamera_Prefab, transform.position, transform.rotation);

            // Instantiate admin interface
            Instantiate(gameMasterUI_Prefab, Vector3.zero, Quaternion.identity);
        }

        public void GM_SwitchToClientMode()
        {
            currGameManagerMode = GameManagerMode.CLIENT;

            if(registeredMarkers.Count > 0)
            {
                // Find spawn marker
                PlayerSpawnMarker playerSpawnMarker =
                    (PlayerSpawnMarker)registeredMarkers.Find(x => x is PlayerSpawnMarker);

                // Spawn local player controller at spawn position
                Instantiate(localPlayerController_Prefab, playerSpawnMarker.pointPosition, playerSpawnMarker.pointRotation);
            }
        }

        public void GM_Host_SwitchToRun()
        {
            SpawnAndSetupNPC();
            currGameManagerHostMode = GameManagerHostMode.RUN;
        }
        #endregion
        
        // ************
        // Host Methods
        // ************
        #region Host Methods
        private void Host_SetupMarkers()
        {
            //foreach (Marker marker in registeredMarkers)
            //{
            //    if (marker.markerGO.GetComponent<BaseMarker>() is IMarkerBehaviours)
            //    {
            //        //marker.markerGO.GetComponent<IMarkerBehaviours>().CleanUpForSimulationStart();
            //    }
            //}
        }
        #endregion
        
        // ************
        // RuntimeEditor Methods
        // ************
        #region RuntimeEditor Methods
        private void UpdateRegisteredMarkers()
        {
            if (currRefreshRate <= 0)
            {
                totalRegMarkers = 0;

                foreach (BaseMarker marker in registeredMarkers)
                {
                    if (!marker.gameObject)
                        registeredMarkers.Remove(marker);
                    else
                        totalRegMarkers++;
                }
                currRefreshRate = refreshRate;
            }
            else
            {
                currRefreshRate -= Time.deltaTime;
            }
        }

        public bool MarkerNameExists(string markerName)
        {
            return registeredMarkers.Exists(x => x.gameObject.name == markerName);
        }

        public string GetUniqueMarkerName(MARKER_TYPE markerType)
        {
            string baseMarkerName;
            int nameModifier = 0;

            switch (markerType)
            {
                case MARKER_TYPE.AREA:
                    baseMarkerName = "Area Marker ";
                    break;
                case MARKER_TYPE.WAYPOINT:
                    baseMarkerName = "Waypoint Marker ";
                    break;
                case MARKER_TYPE.NPCSPAWN:
                    baseMarkerName = "NPC Spawn Marker ";
                    break;
                case MARKER_TYPE.SEAT:
                    baseMarkerName = "Seat Marker ";
                    break;
                case MARKER_TYPE.PLAYER_SPAWN_MARKER:
                    baseMarkerName = "Player Spawn Marker ";
                    break;
                default:
                    baseMarkerName = "";
                    break;
            }

            while (MarkerNameExists(baseMarkerName + nameModifier))
            {
                nameModifier++;
            }

            return baseMarkerName + nameModifier;
        }

        /// <summary>
        /// Adds markers to the Registered Markers list
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="markerType"></param>
        public string RegisterMarker(BaseMarker marker)
        {
            registeredMarkers.Add(marker);
            
            if (marker is NPCSpawnMarker)
                return GetUniqueMarkerName(MARKER_TYPE.NPCSPAWN);
            if (marker is AreaMarker)
                return GetUniqueMarkerName(MARKER_TYPE.AREA);
            if (marker is PlayerSpawnMarker)
                return GetUniqueMarkerName(MARKER_TYPE.PLAYER_SPAWN_MARKER);
            if (marker is ExitMarker)
                return GetUniqueMarkerName(MARKER_TYPE.EXIT);
            if (marker is WaypointMarker)
                return GetUniqueMarkerName(MARKER_TYPE.WAYPOINT);
            if (marker is SeatMarker)
                return GetUniqueMarkerName(MARKER_TYPE.SEAT);

            Debug.Log("??");
            return "error";
        }

        /// <summary>
        /// Removes a registerd marker
        /// </summary>
        /// <param name="gameObject"></param>
        public void UnregisterMarker(GameObject gameObject)
        {
            registeredMarkers.Remove(registeredMarkers.Find(x => x.gameObject == gameObject));
        }
        #endregion

        // ************
        // General Methods
        // ************
        #region General Methods
        public void RestartScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
        #endregion

        public void SetLocalPlayerName(string name)
        {
            localPlayerName = name;
        }

        private void SetNPCSpawnDataFromNPCScriptStorage()
        {
            if(NpcScriptStorage.instance)
                npcSpawnDataList = NpcScriptStorage.instance.GetAllNPCSpawnData();
        }

        private void SpawnAndSetupNPC()
        {
            foreach (NpcSpawnData npcSpawnData in npcSpawnDataList)
            {
                // Get spawn marker
                GameObject SpawnMarker = GetSpawnMarkerByName(npcSpawnData.spawnMarkerName);
                // Get NPC type to spawn
                GameObject npcToSpawn = GetNPCPrefabByNPCType(npcSpawnData.npcOutfit);

                NPCSpawnMarker targetSpawnMarker = SpawnMarker.GetComponent<NPCSpawnMarker>();
                
                // Spawn NPC
                GameObject npc = Instantiate(npcToSpawn, targetSpawnMarker.pointPosition, targetSpawnMarker.pointRotation);
                // Set name
                npc.name = npcSpawnData.npcName;

                // Spawn NPC on all clients
                GameManagerAssistant.instance.CmdNetworkSpawnObject(npc);

                // Setting NPC configurations
                AIController npcGOAIController = npc.GetComponent<AIController>();
                npcGOAIController.Setup(npcSpawnData.npcName, npcSpawnData.aiStats, npcSpawnData.npcSchedules);
                if (npcSpawnData.aiStats.activateOnStart)
                    npcGOAIController.AISetActive();

                // Adding NPC reference to list according to ai type
                AIStats aiStats = npcSpawnData.aiStats;
                switch(aiStats.npcType)
                {
                    case AIStats.NPCType.CIVILLIAN:
                        spawnedCivilianNPCs.Add(npc);
                        break;

                    case AIStats.NPCType.TERRORIST:
                        spawnedHostileNPCs.Add(npc);
                        break;

                    case AIStats.NPCType.VIP:
                        spawnedVIPNPC.Add(npc);
                        break;
                }
            }
        }

        public GameObject GetNearestCivilianNPC(GameObject npcGO)
        {
            GameObject closestNPC = null;

            float distance = Mathf.Infinity;
            Vector3 position = npcGO.transform.position;
            foreach (GameObject civilianNpc in spawnedCivilianNPCs)
            {
                if (civilianNpc != npcGO)
                {
                    Vector3 diff = civilianNpc.transform.position - position;
                    float curDistance = diff.sqrMagnitude;

                    if (curDistance < distance)
                    {
                        closestNPC = civilianNpc;
                        distance = curDistance;
                    }
                }
            }
            return closestNPC;
        }

        public Vector3 GetNearestExitMarkerVector(GameObject npcGO)
        {
            ExitMarker closetExitMarker = null;
            float smallestDist = Mathf.Infinity;

            Vector3 position = npcGO.transform.position;
            foreach (BaseMarker marker in registeredMarkers)
            {
                if (marker is ExitMarker)
                {
                    float markerDist = (marker.transform.position - position).magnitude;
                    if (markerDist < smallestDist)
                    {
                        closetExitMarker = (ExitMarker)marker;
                        smallestDist = markerDist;
                    }
                }
            }
            if (closetExitMarker != null)
                return closetExitMarker.pointPosition;
            else
                return npcGO.transform.position;
        }

        public Vector3 GetWaypointMarkerPosition(string targetName)
        {
            WaypointMarker waypointMarker = (WaypointMarker)registeredMarkers
                .FindAll(x => x is WaypointMarker)
                .Find(x => x.name == targetName);

            return waypointMarker.pointPosition;
        }

        public Quaternion GetWaypointMarkerRotation(string targetName)
        {
            WaypointMarker waypointMarker = (WaypointMarker)registeredMarkers
                .FindAll(x => x is WaypointMarker)
                .Find(x => x.name == targetName);

            return waypointMarker.pointRotation;
        }

        public GameObject GetNPCPrefabByNPCType(NpcSpawnData.NPCOutfit npcType)
        {
            switch (npcType)
            {
                case NpcSpawnData.NPCOutfit.TYPE0:
                    return type0NPC_Prefab;

                case NpcSpawnData.NPCOutfit.TYPE1:
                    return type1NPC_Prefab;
            }
            return null;
        }
        
        private GameObject GetSpawnMarkerByName(string markeName)
        {
            return registeredMarkers.FindAll(x => x is NPCSpawnMarker).Find(x => x.name == markeName).gameObject;
        }
        
        public AreaMarker GetAreaMarkerByName(string areaName)
        {
            return (AreaMarker)registeredMarkers.FindAll(x => x is AreaMarker).Find(x => x.name == areaName);
        }
        
        public bool CheckIfObjectIsRegisteredArea(GameObject gameObject)
        {
            return registeredMarkers.Exists(x => x.gameObject == gameObject);
        }

        public List<AreaMarker> GetAllAreaMarkers()
        {
            List<BaseMarker> markers = registeredMarkers.FindAll(x => x is AreaMarker);
            return markers.OfType<AreaMarker>().ToList();
        }

        public List<WaypointMarker> GetAllWaypointMarkers()
        {
            List<BaseMarker> markers = registeredMarkers.FindAll(x => x is WaypointMarker);
            return markers.OfType<WaypointMarker>().ToList();
        }

        public List<NPCSpawnMarker> GetAllNPCSpawnMarkers()
        {
            List<BaseMarker> markers = registeredMarkers.FindAll(x => x is NPCSpawnMarker);
            return markers.OfType<NPCSpawnMarker>().ToList();
        }

        public void SetSceneInfo(string sceneName, string sceneHash)
        {
            this.sceneName = sceneName;
            this.sceneHash = sceneHash;
        }

        public string GetSceneName()
        {
            return sceneName;
        }

        public string GetSceneHash()
        {
            return sceneHash;
        }
    }

    //[System.Serializable]
    //public class PlayerVectorCalibData
    //{
    //    public string playerName = string.Empty;
    //    public Vector3 point1 = Vector3.zero;
    //    public Vector3 point2 = Vector3.zero;

    //    public PlayerVectorCalibData() { }

    //    public PlayerVectorCalibData(string playerName)
    //    {
    //        this.playerName = playerName;
    //    }
}