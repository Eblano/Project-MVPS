using System.Collections.Generic;
using UnityEngine;
using SealTeam4;
using Battlehub.RTSaveLoad;
using UnityEngine.AI;
using UnityEngine.UI;
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
        [SerializeField] private GameObject gameMasterUI_Prefab;

        public enum MARKER_TYPE { AREA, WAYPOINT, NPCSPAWN, SEAT, PLAYER_SPAWN_MARKER, EXIT, ACCESSORY };

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

        [Header("Accessory Item Prefabs")]
        [SerializeField] private GameObject pistol_Prefab;
        [SerializeField] private GameObject rifle_Prefab;
        [SerializeField] private GameObject bag_Prefab;
        [SerializeField] private GameObject magazine_Prefab;


        // NPC List
        private List<AIController> spawnedCivilianNPCs = new List<AIController>();
        private List<AIController> spawnedVIPNPC = new List<AIController>();
        private List<AIController> spawnedHostileNPCs = new List<AIController>();

        [Space(10)]
       
        public string localPlayerName;

        [SerializeField] private Image panelOverlay;

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

            // Spawn and Build NavMesh
            NavMeshSurface nmSurface = Instantiate(navMeshSurfaceInitator_Prefab, Vector3.zero, Quaternion.identity).GetComponent<NavMeshSurface>();
            nmSurface.BuildNavMesh();
            
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
            currGameManagerHostMode = GameManagerHostMode.RUN;
            ActivateNPCs();
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
                    if (!marker)
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
            return registeredMarkers.FindAll(x => x.gameObject.name == markerName).Count > 1;
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
                case MARKER_TYPE.EXIT:
                    baseMarkerName = "Exit Marker ";
                    break;
                case MARKER_TYPE.ACCESSORY:
                    baseMarkerName = "Accessory Marker ";
                    break;
                default:
                    baseMarkerName = "NOT SPECIFIED";
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

            if(MarkerNameExists(marker.name))
            {
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
                if (marker is AccessoryMarker)
                    return GetUniqueMarkerName(MARKER_TYPE.ACCESSORY);
                if (marker is SeatMarker)
                    return GetUniqueMarkerName(MARKER_TYPE.SEAT);
            }
            
            return marker.name;
        }

        /// <summary>
        /// Removes a registerd marker
        /// </summary>
        /// <param name="gameObject"></param>
        public void UnregisterMarker(GameObject gameObject)
        {
            registeredMarkers.Remove(registeredMarkers.Find(x => x.gameObject == gameObject));
        }

        public bool AllPointMarkersOnPoint()
        {
            foreach(BaseMarker marker in registeredMarkers)
            {
                if(marker is PointMarker)
                {
                    if (marker.GetComponent<PointMarker>().validPoint == false)
                        return false;
                }
            }
            return true;
        }
        #endregion


        // ************
        // General Methods
        // ************
        #region General Methods
        public void RestartScene()
        {
            PlayerPrefs.SetString("SceneToLoad", "_MainScene");
            SceneManager.LoadScene("_LoadingScene");
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

        public void SpawnAndSetupNPC()
        {
            if(!ScriptStorage.instance)
            {
                Debug.Log("Cant find Script Storage");
                return;
            }

            List<NpcSpawnData> npcSpawnDataList = ScriptStorage.instance.GetAllNPCSpawnData();

            if (npcSpawnDataList == null)
            {
                Debug.Log("Cant get npcSpawnDataList");
                return;
            }

            foreach (NpcSpawnData npcSpawnData in npcSpawnDataList)
            {
                // Get spawn marker
                NPCSpawnMarker npcSpawnMarker = GetSpawnMarkerByName(npcSpawnData.spawnMarkerName);

                // Get NPC type to spawn
                GameObject npcToSpawn = GetNPCPrefabByNPCType(npcSpawnData.npcOutfit);

                // Spawn NPC
                GameObject npc = Instantiate(npcToSpawn, npcSpawnMarker.pointPosition, npcSpawnMarker.pointRotation);
                // Set name
                npc.name = npcSpawnData.npcName;

                // Setting NPC configurations
                AIController npcGOAIController = npc.GetComponent<AIController>();
                npcGOAIController.Setup(npcSpawnData.npcName, npcSpawnData.aiStats, npcSpawnData.npcSchedules);

                // Adding NPC reference to list according to ai type
                AIStats aiStats = npcSpawnData.aiStats;
                switch (aiStats.npcType)
                {
                    case AIStats.NPCType.CIVILLIAN:
                        spawnedCivilianNPCs.Add(npcGOAIController);
                        break;

                    case AIStats.NPCType.TERRORIST:
                        spawnedHostileNPCs.Add(npcGOAIController);
                        break;

                    case AIStats.NPCType.VIP:
                        spawnedVIPNPC.Add(npcGOAIController);
                        break;
                }

                // Spawn NPC on all clients
                GameManagerAssistant.instance.CmdNetworkSpawnObject(npc);
            }
        }

        public void ActivateNPCs()
        {
            foreach(AIController npc in spawnedCivilianNPCs)
            {
                if (npc.IsActivateFromSpawn())
                    npc.AISetActive();
            }
            foreach (AIController npc in spawnedHostileNPCs)
            {
                if (npc.IsActivateFromSpawn())
                    npc.AISetActive();
            }
            foreach (AIController npc in spawnedVIPNPC)
            {
                if (npc.IsActivateFromSpawn())
                    npc.AISetActive();
            }
        }

        public void SpawnAccessories()
        {
            if (!ScriptStorage.instance)
            {
                Debug.Log("Cant find Script Storage");
                return;
            }

            List<AccessoryData_SStorage> accessoryList = ScriptStorage.instance.GetAllAccessoryData_SStorage();

            if (accessoryList == null)
            {
                Debug.Log("Cant get npcSpawnDataList");
                return;
            }

            foreach (AccessoryData_SStorage accessoryData in accessoryList)
            {
                // Get accessory spawn marker
                AccessoryMarker accessorySpawnMarker = GetAccessoryMarkerByName(accessoryData.accessoryMarker);

                GameObject accessoryItemPrefab = null;

                switch(accessoryData.accessoryItem)
                {
                    case "Pistol":
                        accessoryItemPrefab = pistol_Prefab;
                        break;
                    case "Rifle":
                        accessoryItemPrefab = pistol_Prefab;
                        break;
                    case "Bag":
                        accessoryItemPrefab = pistol_Prefab;
                        break;
                    case "Magazine":
                        accessoryItemPrefab = pistol_Prefab;
                        break;
                }

                if(!accessoryItemPrefab)
                {
                    Debug.Log("Cant find item to spawn");
                    return;
                }

                GameObject accessoryItem = Instantiate(accessoryItemPrefab, accessorySpawnMarker.pointPosition, accessorySpawnMarker.pointRotation);

                if (GameManagerAssistant.instance)
                    GameManagerAssistant.instance.CmdNetworkSpawnObject(accessoryItem);
                else
                    Debug.Log("GameManageAssistant not found");
            }
        }

        public AIController GetNearestAvailableCivilianNPCForConvo(AIController requester)
        {
            List<AIController> availableNPCs = new List<AIController>();
            AIController availableClosestNPC = null;
            float closestDist = Mathf.Infinity;

            Vector3 position = requester.transform.position;
            foreach (AIController civilianNpc in spawnedCivilianNPCs)
            {
                if (civilianNpc != requester && civilianNpc.AvailableForConversation())
                {
                    availableNPCs.Add(civilianNpc);
                }
            }

            foreach(AIController availableNPC in availableNPCs)
            {
                Vector3 diff = availableNPC.transform.position - position;
                float curDistance = diff.sqrMagnitude;

                if (curDistance < closestDist)
                {
                    availableClosestNPC = availableNPC;
                    closestDist = curDistance;
                }
            }

            return availableClosestNPC;
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
        
        private NPCSpawnMarker GetSpawnMarkerByName(string markeName)
        {
            return (NPCSpawnMarker)registeredMarkers.FindAll(x => x is NPCSpawnMarker).Find(x => x.name == markeName);
        }

        private AccessoryMarker GetAccessoryMarkerByName(string markeName)
        {
            return (AccessoryMarker)registeredMarkers.FindAll(x => x is AccessoryMarker).Find(x => x.name == markeName);
        }

        public AreaMarker GetAreaMarkerByName(string areaName)
        {
            return (AreaMarker)registeredMarkers.FindAll(x => x is AreaMarker).Find(x => x.name == areaName);
        }
        
        public bool CheckIfObjectIsRegisteredArea(GameObject gameObject)
        {
            return registeredMarkers.Exists(x => x.gameObject == gameObject);
        }

        public List<AccessoryMarker> GetAllAccessoryMarkers()
        {
            List<BaseMarker> markers = registeredMarkers.FindAll(x => x is AccessoryMarker);
            return markers.OfType<AccessoryMarker>().ToList();
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

        public bool IsHost()
        {
            if (currGameManagerMode == GameManagerMode.HOST)
                return true;
            else
                return false;
        }

        public string GetSceneName()
        {
            return sceneName;
        }

        public string GetSceneHash()
        {
            return sceneHash;
        }

        public void SetOverlayTransparency(int percent)
        {
            Color c = panelOverlay.color;
            c.a = percent / 100.0f;
            panelOverlay.color = c; 
        }
    }
}