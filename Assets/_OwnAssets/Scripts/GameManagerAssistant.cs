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
            if (isLocalPlayer)
                instance = this;

            if (NetworkServer.active)
                NetworkSpawnObject(this.gameObject);
        }

        public void NetworkSpawnObject(GameObject GO)
        {
            NetworkServer.Spawn(GO);
        }

        [Command]
        public void CmdUpdateNetworkTestBool(bool newValue)
        {
            RpcUpdateNetworkTestBool(newValue);
        }

        [ClientRpc]
        public void RpcUpdateNetworkTestBool(bool newValue)
        {
            GameManager.instance.networkTest = newValue;
        }
    }
}
