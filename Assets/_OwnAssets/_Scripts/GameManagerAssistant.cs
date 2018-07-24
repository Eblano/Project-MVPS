using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace SealTeam4
{
    public class GameManagerAssistant : NetworkBehaviour
    {
        public static GameManagerAssistant instance;
        public List<IActions> allActions;

        private void Update()
        {
            if (isLocalPlayer)
                instance = this;
            else
                return;
        }

        // Example of sending data
        //[Command]
        //public void CmdSetBool(bool state)
        //{
        //    RpcSetBool(state);
        //}

        //[ClientRpc]
        //public void RpcSetBool(bool state)
        //{
        //    GameManager.instance.UpdateNetworkTestBool(state);
        //}

        [TargetRpc]
        public void TargetUpdatePanelTransparency(NetworkConnection networkConnection, int percent)
        {
            GameManager.instance.SetOverlayTransparency(percent);
        }
        
        public void NetworkSpawnGameObj(GameObject GO)
        {
            NetworkServer.Spawn(GO);
        }

        [Command]
        public void CmdSyncHaps(NetworkInstanceId networkInstanceId, ControllerHapticsManager.HapticType hapticType, VRTK.VRTK_DeviceFinder.Devices devices)
        {
            TargetSyncHaps(NetworkServer.objects[networkInstanceId].connectionToClient, hapticType, devices);
        }

        [Command]
        public void CmdRegisterClient(string clientName)
        {
            GameManager.instance.AddNewPlayer(clientName);
        }

        [TargetRpc]
        public void TargetSyncHaps(NetworkConnection networkConnection, ControllerHapticsManager.HapticType hapticType, VRTK.VRTK_DeviceFinder.Devices devices)
        {
            ControllerHapticsManager.instance.PlayHaptic(hapticType, devices);
        }

        // Experimental Feature
        [ClientRpc]
        public void RpcSyncActions(int actionNum,string actionCommand)
        {
            // maybe just use game find all IActions and add to a list then send over the list number
            allActions[actionNum].SetAction(actionCommand);
        }

        [ClientRpc]
        public void RpcGameManagerSendCommand(string goName, string msg)
        {
            GameManager.instance.RecieveNetCmdObjMsg(goName, msg);
        }

        [Command]
        public void CmdSendAudioSourceInt(int audioSourceNum)
        {
            RpcSendAudioSourceInt(audioSourceNum);
        }

        [ClientRpc]
        public void RpcSendAudioSourceInt(int audioSourceNum)
        {
            NetworkASManager.instance.PlayAudioSource(audioSourceNum);
        }
    }
}
