using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace SealTeam4
{
    public class GameManagerAssistant : NetworkBehaviour
    {
        public static GameManagerAssistant instance;
        [SerializeField] private bool spawnOverNetwork = false;

        [SerializeField] private GameObject gameManagerAssistant_Prefab;

        private void Start()
        {
            if (instance == null)
                instance = this;
        }

        private void Update()
        {
            if(!spawnOverNetwork && NetworkManager.singleton.isNetworkActive)
            {
                NetworkServer.Spawn(gameManagerAssistant_Prefab);
                spawnOverNetwork = true;
            }
        }

        public void NetworkSpawnObject(GameObject GO)
        {
            NetworkServer.Spawn(GO);
        }
    }
}
