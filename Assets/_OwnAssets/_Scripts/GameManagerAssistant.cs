using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace SealTeam4
{
    public class GameManagerAssistant : NetworkBehaviour
    {
        public static GameManagerAssistant instance;

        private void Update()
        {
            if (isLocalPlayer)
                instance = this;
            else
                return;
        }

        [Command]
        public void CmdSetBool(bool state)
        {
            RpcSetBool(state);
        }

        [ClientRpc]
        public void RpcSetBool(bool state)
        {
            GameManager.instance.UpdateNetworkTestBool(state);
        }

        [Command]
        public void CmdNetworkSpawnObject(GameObject GO)
        {
            NetworkServer.Spawn(GO);
        }
    }
}
