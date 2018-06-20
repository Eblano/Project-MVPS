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

        [Command]
        public void CmdSyncHaps(NetworkInstanceId networkInstanceId, ControllerHapticsManager.HapticType hapticType, VRTK.VRTK_DeviceFinder.Devices devices)
        {
            TargetSyncHaps(NetworkServer.objects[networkInstanceId].connectionToClient, hapticType, devices);
        }

        [TargetRpc]
        public void TargetSyncHaps(NetworkConnection networkConnection, ControllerHapticsManager.HapticType hapticType, VRTK.VRTK_DeviceFinder.Devices devices)
        {
            ControllerHapticsManager.instance.PlayHaptic(hapticType, devices);
        }
    }
}
