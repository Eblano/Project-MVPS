using System.Collections.Generic;
using UnityEngine;
using SealTeam4;
using Battlehub.RTSaveLoad;
using UnityEngine.AI;

namespace SealTeam4
{
    /// <summary>
    /// Game Manager
    /// Central control of everything
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        // Instance of the Game Manager
        public static GameManager localInstance;

        private GameManagerAssistant ref_GMAssistance;

        // State of GameManager
        private enum GameManagerMode { LEVELSETUP, HOST, CLIENT }
        private GameManagerMode currGameManagerMode = GameManagerMode.LEVELSETUP;

        private enum GameManagerHostMode { WAITFORSTART, RUN }
        private GameManagerHostMode currGameManagerHostMode = GameManagerHostMode.WAITFORSTART;

        [SerializeField] private GameObject localPlayerController;
        [SerializeField] private GameObject gameManagerAssistant;

        public enum MARKER_TYPE { AREA, TARGET, NPC_SPAWN, SEAT, PLAYER_SPAWN_MARKER };

        [Header("NPC Prefabs")]
        [SerializeField] private GameObject type0NPC;
        [SerializeField] private GameObject type1NPC;

        // List of markers GameManager keeps track of
        private List<Marker> registeredMarkers = new List<Marker>();
        [Header("Registered Markers Counter")]
        [SerializeField] private int totalRegMarkers;
        [SerializeField] private float refreshRate = 3.0f;
        private float currRefreshRate;

        [Header("Game Condition")]
        public bool areaUnderAttack;

        //[SerializeField] private bool startGame;
        public bool isHost = false;
        //[SerializeField] private bool serverSetUpGame = false;

        // NPC List
        [SerializeField] private List<NpcSpawnData> npcSpawnList = new List<NpcSpawnData>();
        private List<GameObject> spawnedCivilianNPCs = new List<GameObject>();
        private List<GameObject> spawnedVIPNPC = new List<GameObject>();
        private List<GameObject> spawnedHostileNPCs = new List<GameObject>();

        //// For calibration of in game position from physical position
        //public bool calibrationMode = false;

        public string localPlayerName;

        //public List<PlayerVectorCalibData> playerVectorCalibDataList = new List<PlayerVectorCalibData>();

        private void Start()
        {
            if (localInstance == null)
                localInstance = this;

            SpawnAndSetupGMAssistant();
        }

        private void OnDisable()
        {
            if (registeredMarkers.Count > 0) registeredMarkers.Clear();
        }

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
                    break;
            }
        }

        private void Host_Update()
        {
            switch (currGameManagerHostMode)
            {
                case GameManagerHostMode.WAITFORSTART:
                    break;
                case GameManagerHostMode.RUN:
                    Host_Run_Update();
                    break;
            }
        }

        private void LevelSetup_Update()
        {
            UpdateRegisteredMarkers();
        }

        public void RunGame()
        {
            SpawnAndSetupNPC();
            currGameManagerHostMode = GameManagerHostMode.RUN;
        }

        private void SpawnAndSetupGMAssistant()
        {
            ref_GMAssistance = Instantiate(gameManagerAssistant, transform.position, transform.rotation).GetComponent<GameManagerAssistant>();
            ref_GMAssistance.SetGameManagerInstance(localInstance); 
        }

        private void SetupMarkers()
        {
            foreach (Marker marker in registeredMarkers)
            {
                if (
                    marker.markerType == MARKER_TYPE.NPC_SPAWN ||
                    marker.markerType == MARKER_TYPE.TARGET ||
                    marker.markerType == MARKER_TYPE.PLAYER_SPAWN_MARKER
                    )
                {
                    marker.markerGO.GetComponent<IMarkerBehaviours>().CleanUpForSimulationStart();
                }
            }
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

        private void SpawnAndMoveLocalPlayerController()
        {
            Vector3 localPlayerSpawnPos =
                registeredMarkers.Find(x => x.markerType == MARKER_TYPE.PLAYER_SPAWN_MARKER).markerGO.transform.position;

            Quaternion localPlayerSpawnRot =
                registeredMarkers.Find(x => x.markerType == MARKER_TYPE.PLAYER_SPAWN_MARKER).markerGO.transform.rotation;

            Instantiate(localPlayerController, localPlayerSpawnPos, localPlayerSpawnRot);
        }

        public void SetLocalPlayerName(string name)
        {
            localPlayerName = name;
        }

        private void SpawnAndSetupNPC()
        {
            foreach (NpcSpawnData npcSpawnData in npcSpawnList)
            {
                GameObject targetSpawn = GetSpawnMarkerByName(npcSpawnData.spawnLocation);
                GameObject npcToSpawn = GetNPCGameObjectByENUM(npcSpawnData.nPC_TYPE);
                List<Schedule> npcSchedule = npcSpawnData.nPC_Schedules;
                AIStats aiStats = npcSpawnData.aiStats;
                SpawnMarker targetSpawnMarker = targetSpawn.GetComponent<SpawnMarker>();
                
                GameObject npc = Instantiate(npcToSpawn, targetSpawnMarker.pointPosition, targetSpawnMarker.pointRotation);

                // Tell GameManager Assitant to do the below code
                ref_GMAssistance.NetworkSpawnObject(npc);

                AIController npcGOAIController = npc.GetComponent<AIController>();
                npcGOAIController.SetAIStats(aiStats);
                npcGOAIController.SetSchedule(npcSchedule);
                npcGOAIController.ActivateNPC();

                if (aiStats.isTerrorist)
                {
                    spawnedHostileNPCs.Add(npc);
                    continue;
                }
                if (aiStats.isVIP)
                {
                    spawnedVIPNPC.Add(npc);
                    continue;
                }
                else
                {
                    spawnedCivilianNPCs.Add(npc);
                    continue;
                }
            }
        }

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

        public void SwitchToHostMode()
        {
            currGameManagerMode = GameManagerMode.HOST;
            FindObjectOfType<NavMeshSurface>().BuildNavMesh();
            SetupMarkers();
            isHost = true;
            // Instantiate admin cam
            // Instantiate admin interface
        }

        public void SwitchToClientMode()
        {
            currGameManagerMode = GameManagerMode.CLIENT;
            // Instantiate local player controller
            SpawnAndMoveLocalPlayerController();
            // Instantiate player model
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

        /// <summary>
        /// Returns the closest NPC Object from the transform parameter
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Find and Returns Transform of a TargetMarker by name
        /// </summary>
        /// <param name="targetName"></param>
        /// <returns></returns>
        public Transform GetTargetMarkerTransform(string targetName)
        {
            return registeredMarkers
                .FindAll(x => x.markerType == MARKER_TYPE.TARGET)
                .Find(x => x.markerName == targetName)
                .markerGO.transform;
        }

        /// <summary>
        /// Find and Return GameObject of NPC by NPC_TYPE
        /// </summary>
        /// <param name="npcType"></param>
        /// <returns></returns>
        public GameObject GetNPCGameObjectByENUM(NpcSpawnData.NPC_TYPE npcType)
        {
            switch (npcType)
            {
                case NpcSpawnData.NPC_TYPE.TYPE0:
                    return type0NPC;

                case NpcSpawnData.NPC_TYPE.TYPE1:
                    return type1NPC;
            }
            return null;
        }

        /// <summary>
        /// Find and return GameObject of spawnmarker by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private GameObject GetSpawnMarkerByName(string name)
        {
            return registeredMarkers
                .FindAll(x => x.markerType == MARKER_TYPE.NPC_SPAWN)
                .Find(x => x.markerName == name)
                .markerGO;
        }

        /// <summary>
        /// Returns Area Marker by Area Name
        /// </summary>
        /// <param name="areaName"></param>
        /// <returns></returns>
        public AreaMarker GetAreaMarkerByName(string areaName)
        {
            return registeredMarkers
                .FindAll(x => x.markerType == MARKER_TYPE.AREA)
                .Find(x => x.markerName == areaName)
                .markerGO.GetComponent<AreaMarker>();
        }

        /// <summary>
        /// Returns true/false depending if gameobject in 
        /// param is a registered area
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public bool CheckIfObjectIsRegisteredArea(GameObject go)
        {
            return registeredMarkers.Exists(x => x.markerGO == go);
        }

        /// <summary>
        /// Returns total registered markers
        /// </summary>
        /// <returns></returns>
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