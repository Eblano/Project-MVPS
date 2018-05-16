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
        public static GameManager instance;

        private bool gameStartInitCodeExecuted = false;

        public enum MARKER_TYPE { AREA, TARGET, NPC_SPAWN, SEAT };

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

        [SerializeField] private List<NpcSpawnData> npcSpawnList = new List<NpcSpawnData>();
        private List<GameObject> spawnedCivilianNPCs = new List<GameObject>();
        private List<GameObject> spawnedVIPNPC = new List<GameObject>();
        private List<GameObject> spawnedHostileNPCs = new List<GameObject>();

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
            markers.Clear();
        }

        private void Update()
        {
            // If Runtime Editor is still running
            if (Dependencies.ProjectManager != null)
            {
                UpdateRegisteredMarkers();
            }
            else if (!gameStartInitCodeExecuted)
            {
                InitCodeAfterGameStart();
                gameStartInitCodeExecuted = true;
            }
        }

        /// <summary>
        /// Prepares all registered markers for the switch to game mode
        /// </summary>
        private void InitCodeAfterGameStart()
        {
            foreach (Marker marker in markers)
            {
                switch (marker.markerType)
                {
                    case MARKER_TYPE.TARGET:
                        marker.markerGO.GetComponent<TargetMarker>().RemoveVisualMarkersAndMeshCollider();
                        break;

                    case MARKER_TYPE.NPC_SPAWN:
                        marker.markerGO.GetComponent<SpawnMarker>().RemoveVisualMarkersAndMeshCollider();
                        break;
                }
            }
            FindObjectOfType<NavMeshSurface>().BuildNavMesh();
            SpawnAndSetupNPC();
        }

        /// <summary>
        /// Spawn and Setup NPC's
        /// Schedule and Stats
        /// </summary>
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

                if(aiStats.isTerrorist)
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

        /// <summary>
        /// Updates registered markers
        /// </summary>
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
            foreach(GameObject hostileNPC in spawnedHostileNPCs)
            {
                RaycastHit hitinfo;
                if(Physics.Raycast(npcT.position, hostileNPC.transform.position - npcT.position, out hitinfo))
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
    }
}


