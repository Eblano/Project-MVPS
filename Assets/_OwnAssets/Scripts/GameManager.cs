using System.Collections.Generic;
using UnityEngine;
using SealTeam4;
using Battlehub.RTSaveLoad;
using UnityEngine.AI;
//using UnityEngine.Networking;

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

        private bool gameStartInitCodeExecuted = false;

        public enum MARKER_TYPE { AREA, TARGET, NPC_SPAWN, SEAT, PLAYER_SPAWN_MARKER };

        // List of markers GameManager keeps track of
        private List<Marker> markers;

        // NPC Prefab
        [SerializeField] private GameObject type0NPC;
        [SerializeField] private GameObject type1NPC;

        // (Work in progress)
        [SerializeField] private int totalRegMarkers;
        [SerializeField] private float refreshRate = 3.0f;
        private float currRefreshRate;

        public bool areaUnderAttack;

        [SerializeField] private bool startGame;
        public bool isServerObj = false;

        [SerializeField] private List<NpcSpawnData> npcSpawnList = new List<NpcSpawnData>();
        private List<GameObject> spawnedCivilianNPCs = new List<GameObject>();
        private List<GameObject> spawnedVIPNPC = new List<GameObject>();
        private List<GameObject> spawnedHostileNPCs = new List<GameObject>();

        // For calibration of in game position from physical position
        public bool calibrationMode = false;

        public string localPlayerName;

        public List<PlayerVectorCalibData> playerVectorCalibDataList = new List<PlayerVectorCalibData>();

        private void Start()
        {
            if (instance == null)
                instance = this;
            else
            {
                Debug.LogWarning("Duplicate GameManager Detected");
                Destroy(gameObject);
            }

            markers = new List<Marker>();
        }

        private void OnDisable()
        {
            if (markers != null && markers.Count > 0)
                markers.Clear();
        }

        private void Update()
        {
            // If Runtime Editor is still running
            if (Dependencies.ProjectManager != null)
            {
                RTERunning_Update();
            }
            else if (!gameStartInitCodeExecuted && startGame && isServerObj)
            {
                InitCodeAfterGameStart();
                gameStartInitCodeExecuted = true;
            }

            if (gameStartInitCodeExecuted && startGame && isServerObj)
            {
                GameRunning_Update();
            }
        }

        private void RTERunning_Update()
        {
            UpdateRegisteredMarkers();
        }

        private void GameRunning_Update()
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                NetworkPlayerPosManager.localInstance.RpcCalibratePlayerVector(playerVectorCalibDataList);
            }
        }

        private void InitCodeAfterGameStart()
        {
            foreach (Marker marker in markers)
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
            FindObjectOfType<NavMeshSurface>().BuildNavMesh();
            SpawnAndSetupNPC();
            TeleportLocalPlayerControllerToPlayerSpawnPos();
        }

        private void TeleportLocalPlayerControllerToPlayerSpawnPos()
        {
            Vector3 localPlayerSpawnPos =
                markers.Find(x => x.markerType == MARKER_TYPE.PLAYER_SPAWN_MARKER).markerGO.transform.position;

            GameObject localPlayerController = GameObject.Find("LocalPlayerController(Clone)");
            localPlayerController.transform.position = localPlayerSpawnPos;
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

                GameObject spawnedNPC = targetSpawnMarker.SpawnNPC(npcToSpawn, npcSchedule, aiStats);

                if (aiStats.isTerrorist)
                {
                    spawnedHostileNPCs.Add(spawnedNPC);
                    continue;
                }
                if (aiStats.isVIP)
                {
                    spawnedVIPNPC.Add(spawnedNPC);
                    continue;
                }
                else
                {
                    spawnedCivilianNPCs.Add(spawnedNPC);
                    continue;
                }
            }
        }

        private void UpdateRegisteredMarkers()
        {
            if (currRefreshRate <= 0)
            {
                totalRegMarkers = 0;

                foreach (Marker marker in markers)
                {
                    if (!marker.markerGO)
                    {
                        markers.Remove(marker);
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
            markers.Add(new Marker(gameObject, markerType));
        }

        /// <summary>
        /// Removes a registerd marker
        /// </summary>
        /// <param name="gameObject"></param>
        public void UnregisterMarker(GameObject gameObject)
        {
            markers.Remove(markers.Find(x => x.markerGO == gameObject));
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
            return markers
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
            return markers
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
            return markers
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
            return markers.Exists(x => x.markerGO == go);
        }

        /// <summary>
        /// Set GameManager's Schedule, for GameManagerConsole
        /// </summary>
        /// <param name="npcSpawnList"></param>
        public void SetGameManagerConfig(List<NpcSpawnData> npcSpawnList)
        {
            this.npcSpawnList = npcSpawnList;
        }

        /// <summary>
        /// Returns total registered markers
        /// </summary>
        /// <returns></returns>
        public int GetTotalRegMarkers()
        {
            return totalRegMarkers;
        }

        private void OnDestroy()
        {
            instance = null;
        }
    }
}

[System.Serializable]
public class PlayerVectorCalibData
{
    public string playerName = string.Empty;
    public Vector3 point1 = Vector3.zero;
    public Vector3 point2 = Vector3.zero;

    public PlayerVectorCalibData() { }

    public PlayerVectorCalibData(string playerName)
    {
        this.playerName = playerName;
    }
}
