using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace SealTeam4
{
    public class GameManagerAssistant : NetworkBehaviour
    {
        private GameManager localInstace;

        public void SetGameManagerInstance(GameManager instance)
        {
            localInstace = instance;
        }

        public void NetworkSpawnObject(GameObject gameObject)
        {
            NetworkServer.Spawn(gameObject);
        }
    }
}
