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

        private void Start()
        {
            if (instance == null)
                instance = this;
        }

        private void Update()
        {
            if(!spawnOverNetwork && NetworkServer.active)
            {
                NetworkServer.Spawn(this.gameObject);
                spawnOverNetwork = true;
            }
        }

        public void NetworkSpawnObject(GameObject GO)
        {
            NetworkServer.Spawn(GO);
        }
    }
}
