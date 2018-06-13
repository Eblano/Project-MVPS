using System.Collections.Generic;
using UnityEngine;
using SealTeam4;
using Battlehub.RTSaveLoad;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace SealTeam4
{
    /// <summary>
    /// Game Manager
    /// Central control of everything
    /// </summary>
    public class GameManager : MonoBehaviour
    {
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
        [SerializeField] private GameObject playerModel_Prefab;

        [Header("Admin Components")]
        [SerializeField] private GameObject gameMasterCamera_Prefab;
        [SerializeField] private GameObject gameMasterUI_Prefab;

        public enum MARKER_TYPE { AREA, TARGET, NPC_SPAWN, SEAT, PLAYER_SPAWN_MARKER };

        [Header("NPC Prefabs")]
        [SerializeField] private GameObject type0NPC_Prefab;
        [SerializeField] private GameObject type1NPC_Prefab;

        // List of markers GameManager keeps track of
        private List<Marker> registeredMarkers = new List<Marker>();
        [Header("Registered Markers Counter")]
        [SerializeField] private int totalRegMarkers;
        [SerializeField] private float refreshRate = 3.0f;
        private float currRefreshRate;

        [Header("Game Condition")]
        public bool areaUnderAttack;
        
        public bool isHost = false;

        [Space(10)]

        // NPC List
        [SerializeField] private List<NpcSpawnData> npcSpawnList = new List<NpcSpawnData>();
        private List<GameObject> spawnedCivilianNPCs = new List<GameObject>();
        private List<GameObject> spawnedVIPNPC = new List<GameObject>();
        private List<GameObject> spawnedHostileNPCs = new List<GameObject>();

        //// For calibration of in game position from physical position
        //public bool calibrationMode = false;

        [Space(10)]

        public string localPlayerName;

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

            // Spawn and Build NavMesh
            Instantiate(navMeshSurfaceInitator_Prefab, Vector3.zero, Quaternion.identity)
                .GetComponent<NavMeshSurface>()
                .BuildNavMesh();

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

            // Find spawn marker
            PlayerSpawnMarker playerSpawnMarker =
                registeredMarkers.Find(x => x.markerType == MARKER_TYPE.PLAYER_SPAWN_MARKER)
                .markerGO
                .GetComponent<PlayerSpawnMarker>();

            // Spawn local player controller at spawn position
            Instantiate(localPlayerController_Prefab, playerSpawnMarker.pointPosition, playerSpawnMarker.pointRotation);

            // Spawn player model at spawn position
            Instantiate(playerModel_Prefab, playerSpawnMarker.pointPosition, playerSpawnMarker.pointRotation);
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
            foreach (Marker marker in registeredMarkers)
            {
                if(marker is IMarkerBehaviours)
                {
                    marker.markerGO.GetComponent<IMarkerBehaviours>().CleanUpForSimulationStart();
                }
            }
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

                foreach (Marker marker in registeredMarkers)
                {
                    if (!marker.markerGO)
                    {
                        registeredMarkers.Remove(marker);
                    }
                    else
                    {
                        marker.markerName = marker.markerGO.name;
                        totalRegMarkers++;
                    }
                }
                currRefreshRate = refreshRate;
            }
            else
            {
                currRefreshRate -= Time.deltaTime;
            }
        }

        /// <summary>
        /// Adds markers to the Registered Markers list
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="markerType"></param>
        public void RegisterMarker(GameObject gameObject, MARKER_TYPE markerType)
        {
            registeredMarkers.Add(new Marker(gameObject, markerType));
        }

        /// <summary>
        /// Removes a registerd marker
        /// </summary>
        /// <param name="gameObject"></param>
        public void UnregisterMarker(GameObject gameObject)
        {
            registeredMarkers.Remove(registeredMarkers.Find(x => x.markerGO == gameObject));
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

        private void SpawnAndSetupNPC()
        {
            foreach (NpcSpawnData npcSpawnData in npcSpawnList)
            {
                // Get spawn marker
                GameObject SpawnMarker = GetSpawnMarkerByName(npcSpawnData.spawnMarkerName);
                // Get NPC type to spawn
                GameObject npcToSpawn = GetNPCPrefabByNPCType(npcSpawnData.nPC_TYPE);

                SpawnMarker targetSpawnMarker = SpawnMarker.GetComponent<SpawnMarker>();
                
                // Spawn NPC
                GameObject npc = Instantiate(npcToSpawn, targetSpawnMarker.pointPosition, targetSpawnMarker.pointRotation);
                
                // Spawn NPC on all clients
                GameManagerAssistant.instance.NetworkSpawnObject(npc);

                // Setting NPC configurations
                AIController npcGOAIController = npc.GetComponent<AIController>();
                npcGOAIController.SetAIStats(npcSpawnData.aiStats);
                npcGOAIController.SetSchedule(npcSpawnData.npcSchedules);

                // Activate NPC
                npcGOAIController.ActivateNPC();

                // Adding NPC reference to list according to ai type
                AIStats aiStats = npcSpawnData.aiStats;
                switch(aiStats.aiType)
                {
                    case AIStats.AiType.CIVILLIAN:
                        spawnedCivilianNPCs.Add(npc);
                        break;

                    case AIStats.AiType.TERRORIST:
                        spawnedHostileNPCs.Add(npc);
                        break;

                    case AIStats.AiType.VIP:
                        spawnedVIPNPC.Add(npc);
                        break;
                }
            }
        }

        public GameObject GetNearestCivilianNPC(Transform transform, GameObject ownGameObject)
        {
            GameObject closestNPC = null;

            float distance = Mathf.Infinity;
            Vector3 position = transform.position;
            foreach (GameObject civilianNpc in spawnedCivilianNPCs)
            {
                if (civilianNpc != ownGameObject)
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

        public bool LineOfSightAgainstHostileNPC(Transform npcT)
        {
            foreach (GameObject hostileNPC in spawnedHostileNPCs)
            {
                RaycastHit hitinfo;
                if (Physics.Raycast(npcT.position, hostileNPC.transform.position - npcT.position, out hitinfo))
                {
                    if (hitinfo.transform.tag == "NPC")
                    {
                        return true;
                    }
                    else
                    {
                    }
                    Debug.DrawLine(npcT.position, hitinfo.point);
                }

            }
            return false;
        }
        
        public Transform GetTargetMarkerTransform(string targetName)
        {
            return registeredMarkers
                .FindAll(x => x.markerType == MARKER_TYPE.TARGET)
                .Find(x => x.markerName == targetName)
                .markerGO.transform;
        }
        
        public GameObject GetNPCPrefabByNPCType(NpcSpawnData.NPC_TYPE npcType)
        {
            switch (npcType)
            {
                case NpcSpawnData.NPC_TYPE.TYPE0:
                    return type0NPC_Prefab;

                case NpcSpawnData.NPC_TYPE.TYPE1:
                    return type1NPC_Prefab;
            }
            return null;
        }
        
        private GameObject GetSpawnMarkerByName(string name)
        {
            return registeredMarkers
                .FindAll(x => x.markerType == MARKER_TYPE.NPC_SPAWN)
                .Find(x => x.markerName == name)
                .markerGO;
        }
        
        public AreaMarker GetAreaMarkerByName(string areaName)
        {
            return registeredMarkers
                .FindAll(x => x.markerType == MARKER_TYPE.AREA)
                .Find(x => x.markerName == areaName)
                .markerGO.GetComponent<AreaMarker>();
        }
        
        public bool CheckIfObjectIsRegisteredArea(GameObject go)
        {
            return registeredMarkers.Exists(x => x.markerGO == go);
        }
        
        public int GetTotalRegMarkers()
        {
            return totalRegMarkers;
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