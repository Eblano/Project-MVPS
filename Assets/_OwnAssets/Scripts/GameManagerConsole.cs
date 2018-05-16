using Battlehub.RTSaveLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Interfaces with Game Manager, to allow user to change GameManager Configurations
/// </summary>
namespace SealTeam4
{
    public class GameManagerConsole : MonoBehaviour
    {
        [Battlehub.SerializeIgnore] public static GameManagerConsole instance;

        [SerializeField] private int numOfRegisteredMarkers = 0;

        [SerializeField] private List<NpcSpawnData> npcSpawnList = new List<NpcSpawnData>();

        private void Start()
        {
            if (Dependencies.ProjectManager != null && !GameManager.instance)
            {
                Debug.LogError("GameManager not found, GameManagerConsole cannot function");
            }

            if (instance == null)
                instance = this;
            else
            {
                Debug.LogWarning("Duplicate GameManagerConsole Detected");
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            if(Dependencies.ProjectManager != null)
            {
                UpdateRegisteredMarkerNumbers();
            }
            else
            {
                PackageAndSetConfigToGameManager();
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Update Registered Marker number indicator
        /// </summary>
        private void UpdateRegisteredMarkerNumbers()
        {
            numOfRegisteredMarkers = GameManager.instance.GetTotalRegMarkers();
        }

        /// <summary>
        /// Send Config to Game Manager
        /// </summary>
        private void PackageAndSetConfigToGameManager()
        {
            GameManager.instance.SetGameManagerConfig(npcSpawnList);
        }
    }
}
