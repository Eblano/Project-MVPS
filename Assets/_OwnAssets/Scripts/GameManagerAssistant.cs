using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace SealTeam4
{
    public class GameManagerAssistant : NetworkBehaviour
    {
        public static GameManagerAssistant instance;

        private void Start()
        {
            if (instance == null)
                instance = this;
        }

        public void NetworkSpawnObject(GameObject GO)
        {
            NetworkServer.Spawn(GO);
        }
    }
}
