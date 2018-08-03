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
        public NetworkInstanceId playerID;
        [SerializeField] private List<PlayerCalibrationInfo> playerCalibrationInfos = new List<PlayerCalibrationInfo>();

        private void Update()
        {
            if (isServer)
            {
                Debug.Log("Is server in");
                if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.C) && Input.GetKeyDown(KeyCode.B))
                {
                    Debug.Log("Is key pressed");
                    if (IsReadyForCalibration())
                    {
                        CalibratePlayers();
                    }
                }
            }

            if (isLocalPlayer)
                instance = this;
            else
                return;
        }

        [ClientRpc]
        public void RpcGameManagerSendCommand(string goName, string msg)
        {
            GameManager.instance.RecieveNetCmdObjMsg(goName, msg);
        }

        [TargetRpc]
        public void TargetOnPlayerDeath(NetworkConnection networkConnection)
        {
            GameManager.instance.ActivatedOnDeath();
        }

        [TargetRpc]
        public void TargetOnPlayerDamaged(NetworkConnection networkConnection, int hpLeft)
        {
            GameManager.instance.ActivatedOnDamaged(hpLeft);
        }

        public void NetworkSpawnGameObj(GameObject GO)
        {
            NetworkServer.Spawn(GO);
        }

        [Command]
        public void CmdRegisterClient(string clientName)
        {
            GameManager.instance.AddNewPlayer(clientName);
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

        [Command]
        public void CmdSyncScale(NetworkInstanceId networkId, Vector3 yScale, Vector3 handScale)
        {
            RpcSyncScale(networkId, yScale, handScale);
        }

        [ClientRpc]
        public void RpcSyncScale(NetworkInstanceId networkId, Vector3 yScale, Vector3 handScale)
        {
            Debug.Log("NetworkID: " + networkId);
            Debug.Log("NetworkObject: " + ClientScene.objects[networkId]);
            Debug.Log("GO Name: " + ClientScene.objects[networkId].gameObject.name);

            if (!ClientScene.objects.ContainsKey(networkId))
            {
                return;
            }

            ClientScene.objects[networkId].gameObject.GetComponent<PlayerSizeCalibration>().ApplyScale(yScale, handScale);
        }

        #region PlayerInteractions
        // Player interaction syncs
        public override void OnStartLocalPlayer()
        {
            playerID = GetComponent<NetworkIdentity>().netId;
        }

        public void RelaySenderCmdSnapToController(NetworkInstanceId childID, bool isLeftController)
        {
            CmdSnapToController(childID, playerID, isLeftController);
        }

        public void RelaySenderCmdUnSnapFromController(bool isLeftController, Vector3 velo, Vector3 anguVelo)
        {
            CmdUnSnapFromController(playerID, isLeftController, velo, anguVelo);
        }

        public void RelaySenderCmdGunShellSync(NetworkInstanceId gunNetID, Vector3 force)
        {
            CmdGunShellSync(playerID, gunNetID, force);
        }

        public void RelaySenderCmdGunEffectSync(NetworkInstanceId gunNetID)
        {
            CmdGunEffectSync(playerID, gunNetID);
        }

        public void RelaySenderCmdDropMagazine(NetworkInstanceId gunNetID)
        {
            CmdDropMagazine(playerID, gunNetID);
        }

        public void RelaySenderCmdTorchlightState(NetworkInstanceId torchNetID, bool lightState)
        {
            CmdTorchlightState(playerID, torchNetID, lightState);
        }

        [Command]
        public void CmdSnapToParentGameObj(NetworkInstanceId childID, NetworkInstanceId parentID, Vector3 offset)
        {
            RpcSnapToParentGameObj(childID, parentID, offset);
        }

        [Command]
        private void CmdSnapToController(NetworkInstanceId childID, NetworkInstanceId senderPlayerID, bool isLeftController)
        {
            RpcSnapToController(childID, senderPlayerID, isLeftController);
        }

        [Command]
        private void CmdUnSnapFromController(NetworkInstanceId senderPlayerID, bool isLeftController, Vector3 velo, Vector3 anguVelo)
        {
            RpcUnSnapFromController(senderPlayerID, isLeftController, velo, anguVelo);
        }

        [Command]
        private void CmdGunShellSync(NetworkInstanceId senderPlayerID, NetworkInstanceId gunNetID, Vector3 force)
        {
            RpcGunShellSync(senderPlayerID, gunNetID, force);
        }

        [Command]
        private void CmdGunEffectSync(NetworkInstanceId senderPlayerID, NetworkInstanceId gunNetID)
        {
            RpcGunEffectSync(senderPlayerID, gunNetID);
        }

        [Command]
        private void CmdDropMagazine(NetworkInstanceId senderPlayerID, NetworkInstanceId gunNetID)
        {
            RpcDropMagazine(senderPlayerID, gunNetID);
        }

        [Command]
        private void CmdTorchlightState(NetworkInstanceId senderPlayerID, NetworkInstanceId torchNetID, bool lightState)
        {
            RpcTorchlightState(senderPlayerID, torchNetID, lightState);
        }

        [ClientRpc]
        private void RpcSnapToParentGameObj(NetworkInstanceId childID, NetworkInstanceId parentID, Vector3 offset)
        {
            SnapTo(ClientScene.objects[childID].gameObject, ClientScene.objects[parentID].gameObject, offset);
        }

        [ClientRpc]
        private void RpcSnapToController(NetworkInstanceId childID, NetworkInstanceId senderPlayerID, bool isLeftController)
        {
            ClientScene.objects[senderPlayerID].GetComponent<PlayerInteractionSync>().SyncControllerSnap(isLeftController, ClientScene.objects[childID].gameObject);
        }

        [ClientRpc]
        private void RpcUnSnapFromController(NetworkInstanceId senderPlayerID, bool isLeftController, Vector3 velo, Vector3 anguVelo)
        {
            ClientScene.objects[senderPlayerID].GetComponent<PlayerInteractionSync>().SyncControllerUnSnap(isLeftController, velo, anguVelo);
        }

        [ClientRpc]
        private void RpcGunShellSync(NetworkInstanceId senderPlayerID, NetworkInstanceId gunNetID, Vector3 force)
        {
            if (senderPlayerID == playerID)
            {
                return;
            }

            ClientScene.objects[gunNetID].GetComponent<Gun>().BulletShellSpawn(force);
        }

        [ClientRpc]
        private void RpcGunEffectSync(NetworkInstanceId senderPlayerID, NetworkInstanceId gunNetID)
        {
            if (senderPlayerID == playerID)
            {
                return;
            }

            ClientScene.objects[gunNetID].GetComponent<Gun>().ActivateGunEffects();
        }

        [ClientRpc]
        private void RpcDropMagazine(NetworkInstanceId senderPlayerID, NetworkInstanceId gunNetID)
        {
            if (senderPlayerID == playerID)
            {
                return;
            }

            ClientScene.objects[gunNetID].GetComponent<Gun>().UnloadMagazine();
        }

        [ClientRpc]
        private void RpcTorchlightState(NetworkInstanceId senderPlayerID, NetworkInstanceId torchNetID, bool lightState)
        {
            if (senderPlayerID == playerID)
            {
                return;
            }

            ClientScene.objects[torchNetID].GetComponent<Torchlight>().SetLightState(lightState);
        }

        [Command]
        public void CmdGunFire(NetworkInstanceId gunNetID)
        {
            NetworkServer.objects[gunNetID].GetComponent<Gun>().FireBullet();
        }

        [ClientRpc]
        public void RpcSyncGunEffects(NetworkInstanceId aiGunID)
        {
            ClientScene.objects[aiGunID].GetComponent<NPCGun>().SyncAIGunEffects();
        }

        //[ClientRpc]
        //public void RpcSetUpDoorHandle(int doorIndex, NetworkInstanceId doorKnobID)
        //{
        //    DoorHandleSpawner.instance.doorHandles[doorIndex].SetUpKnob(ClientScene.objects[doorKnobID].gameObject);
        //}

        public void RelaySenderCmdSpawnBloodPlayer(NetworkInstanceId gunNetID, Vector3 hitPos, Vector3 normal, Vector3 faceAngle)
        {
            CmdSpawnBloodPlayer(playerID, gunNetID, hitPos, normal, faceAngle);
        }

        [Command]
        private void CmdSpawnBloodPlayer(NetworkInstanceId senderPlayerID, NetworkInstanceId gunNetID, Vector3 hitPos, Vector3 normal, Vector3 faceAngle)
        {
            RpcSpawnBloodPlayer(senderPlayerID, gunNetID, hitPos, normal, faceAngle);
        }

        [ClientRpc]
        private void RpcSpawnBloodPlayer(NetworkInstanceId senderPlayerID, NetworkInstanceId gunNetID, Vector3 hitPos, Vector3 normal, Vector3 faceAngle)
        {
            if (senderPlayerID == playerID)
            {
                return;
            }

            ClientScene.objects[gunNetID].GetComponent<Gun>().SpawnBlood(hitPos, normal, faceAngle);
        }

        public void RelaySenderCmdSpawnBulletHolePlayer(NetworkInstanceId gunNetID, Vector3 hitPos, Vector3 normal, Vector3 faceAngle)
        {
            CmdSpawnBulletHolePlayer(playerID, gunNetID, hitPos, normal, faceAngle);
        }

        [Command]
        private void CmdSpawnBulletHolePlayer(NetworkInstanceId senderPlayerID, NetworkInstanceId gunNetID, Vector3 hitPos, Vector3 normal, Vector3 faceAngle)
        {
            RpcSpawnBulletHolePlayer(senderPlayerID, gunNetID, hitPos, normal, faceAngle);
        }

        [ClientRpc]
        private void RpcSpawnBulletHolePlayer(NetworkInstanceId senderPlayerID, NetworkInstanceId gunNetID, Vector3 hitPos, Vector3 normal, Vector3 faceAngle)
        {
            if (senderPlayerID == playerID)
            {
                return;
            }

            ClientScene.objects[gunNetID].GetComponent<Gun>().SpawnBulletHole(hitPos, normal, faceAngle);
        }

        [ClientRpc]
        public void RpcSpawnBloodServer(NetworkInstanceId gunNetID, Vector3 hitPos, Vector3 normal, Vector3 faceAngle)
        {
            ClientScene.objects[gunNetID].GetComponent<NPCGun>().SpawnBlood(hitPos, normal, faceAngle);
        }

        [ClientRpc]
        public void RpcSpawnBulletHoleServer(NetworkInstanceId gunNetID, Vector3 hitPos, Vector3 normal, Vector3 faceAngle)
        {
            ClientScene.objects[gunNetID].GetComponent<NPCGun>().SpawnBulletHole(hitPos, normal, faceAngle);
        }

        public void RelaySenderCmdOpenDoor(int doorIndex, bool state)
        {
            if (state)
            {
                CmdOpenDoor(playerID, doorIndex);
            }
            else
            {
                CmdCloseDoor(playerID, doorIndex);
            }
        }

        [Command]
        private void CmdOpenDoor(NetworkInstanceId senderPlayerID, int doorIndex)
        {
            RpcOpenDoor(senderPlayerID, doorIndex);
        }

        [Command]
        private void CmdCloseDoor(NetworkInstanceId senderPlayerID, int doorIndex)
        {
            RpcCloseDoor(senderPlayerID, doorIndex);
        }

        [ClientRpc]
        private void RpcOpenDoor(NetworkInstanceId senderPlayerID, int doorIndex)
        {
            if (playerID == senderPlayerID)
            {
                return;
            }

            DoorKnobHandler.instance.SyncKnob(doorIndex, true);
        }

        [ClientRpc]
        private void RpcCloseDoor(NetworkInstanceId senderPlayerID, int doorIndex)
        {
            if (playerID == senderPlayerID)
            {
                return;
            }

            DoorKnobHandler.instance.SyncKnob(doorIndex, false);
        }

        //[Command]
        //public void CmdApplyRBPhysics(NetworkInstanceId objApplyRB, Vector3 velo, Vector3 anguVelo)
        //{
        //    RpcApplyRBPhysics(objApplyRB, velo, anguVelo);
        //}

        //[ClientRpc]
        //private void RpcApplyRBPhysics(NetworkInstanceId objApplyRB, Vector3 velo, Vector3 anguVelo)
        //{
        //    Rigidbody rb = ClientScene.objects[objApplyRB].GetComponent<Rigidbody>();

        //    if (rb)
        //    {
        //        ApplyRigidbodyPhysics(rb, velo, anguVelo);
        //    }
        //}

        //private void ApplyRigidbodyPhysics(Rigidbody rb, Vector3 velo, Vector3 anguVelo)
        //{
        //    rb.velocity = velo;
        //    rb.angularVelocity = anguVelo;
        //}

        public void RelaySenderCmdSnapBiped(int childID, bool isLeftController)
        {
            CmdSnapBiped(childID, playerID, isLeftController);
        }

        public void RelaySenderCmdUnSnapBiped(int childID)
        {
            CmdUnSnapBiped(playerID, childID);
        }

        [Command]
        private void CmdSnapBiped(int childID, NetworkInstanceId senderPlayerID, bool isLeftController)
        {
            RpcSnapBiped(childID, senderPlayerID, isLeftController);
        }

        [Command]
        private void CmdUnSnapBiped(NetworkInstanceId senderPlayerID, int childID)
        {
            RpcUnSnapBiped(senderPlayerID, childID);
        }

        [ClientRpc]
        private void RpcSnapBiped(int childID, NetworkInstanceId senderPlayerID, bool isLeftController)
        {
            if (playerID == senderPlayerID)
            {
                return;
            }

            BipedManager.instance.CallOnGrab(childID, ClientScene.objects[senderPlayerID].GetComponent<PlayerInteractionSync>().GetControllerTransform(isLeftController));
        }

        [ClientRpc]
        private void RpcUnSnapBiped(NetworkInstanceId senderPlayerID, int childID)
        {
            if (playerID == senderPlayerID)
            {
                return;
            }

            BipedManager.instance.CallUnGrab(childID);
        }

        public void RelaySenderCmdAddCalibrateInfo(Vector3 pos, bool isLeft)
        {
            CmdAddCalibrateInfo(playerID, pos, isLeft);
        }

        [Command]
        private void CmdAddCalibrateInfo(NetworkInstanceId senderPlayerId, Vector3 pos, bool isLeft)
        {
            CalibrateInfo(senderPlayerId, pos, isLeft);
        }

        [TargetRpc]
        public void TargetCalibrateLPC(NetworkConnection conn, Vector3 posCorrection, float rotCorrection)
        {
            GameManager.instance.ApplyCorrection(posCorrection, rotCorrection);
        }

        #region Calibration
        public void CalibrateInfo(NetworkInstanceId senderPlayerId, Vector3 pos, bool isLeft)
        {
            Debug.Log("Registered" + senderPlayerId);
            if (IsUniquePlayerID(senderPlayerId))
            {
                Debug.Log("New entry" + senderPlayerId);
                PlayerCalibrationInfo pci = new PlayerCalibrationInfo();
                pci.NetworkInstanceIdPlayer = senderPlayerId;
                playerCalibrationInfos.Add(pci);
                AddCalibrationPos(senderPlayerId, pos, isLeft);
            }
            else
            {
                Debug.Log("Existing entry" + senderPlayerId);
                AddCalibrationPos(senderPlayerId, pos, isLeft);
            }
        }

        private void CalibratePlayers()
        {
            // Using first registered player as calibration point
            for (int counter = 1; counter < playerCalibrationInfos.Count; counter++)
            {
                TargetCalibrateLPC
                    (
                    NetworkServer.objects[playerCalibrationInfos[counter].NetworkInstanceIdPlayer].connectionToClient,
                    playerCalibrationInfos[0].LeftControllerPos - playerCalibrationInfos[counter].LeftControllerPos,
                    Vector3.Angle(playerCalibrationInfos[counter].LeftControllerPos - playerCalibrationInfos[counter].RightControllerPos, playerCalibrationInfos[0].LeftControllerPos - playerCalibrationInfos[0].RightControllerPos)
                    );
            }
        }

        private bool IsReadyForCalibration()
        {
            if (playerCalibrationInfos.Count < 2)
            {
                Debug.Log("Not enough data to calibrate");
                return false;
            }

            foreach(PlayerCalibrationInfo pci in playerCalibrationInfos)
            {
                if (pci.LeftControllerPos == Vector3.zero || pci.RightControllerPos == Vector3.zero)
                {
                    Debug.Log("Empty positional data in left or right controller");
                    return false;
                }
            }

            return true;
        }

        private void AddCalibrationPos(NetworkInstanceId playerId, Vector3 pos, bool isLeft)
        {
            if (isLeft)
            {
                foreach (PlayerCalibrationInfo pci in playerCalibrationInfos)
                {
                    if (pci.NetworkInstanceIdPlayer == playerId)
                    {
                        pci.LeftControllerPos = pos;
                        return;
                    }
                }
            }
            else
            {
                foreach (PlayerCalibrationInfo pci in playerCalibrationInfos)
                {
                    if (pci.NetworkInstanceIdPlayer == playerId)
                    {
                        pci.RightControllerPos = pos;
                        return;
                    }
                }
            }
        }

        private bool IsUniquePlayerID(NetworkInstanceId playerId)
        {
            foreach (PlayerCalibrationInfo pci in playerCalibrationInfos)
            {
                if (pci.NetworkInstanceIdPlayer == playerId)
                {
                    return false;
                }
            }

            return true;
        }
        #endregion Calibration

        private void SnapTo(GameObject child, GameObject parent)
        {
            if (child.GetComponent<Rigidbody>())
            {
                child.GetComponent<Rigidbody>().isKinematic = true;
            }

            child.transform.SetParent(parent.transform);
            child.transform.localPosition = Vector3.zero;
            child.transform.localRotation = Quaternion.identity;
        }

        private void SnapTo(GameObject child, GameObject parent, Vector3 offset)
        {
            if (child.GetComponent<Rigidbody>())
            {
                child.GetComponent<Rigidbody>().isKinematic = true;
            }

            child.transform.SetParent(parent.transform);
            child.transform.localPosition = -offset;
            child.transform.localRotation = Quaternion.identity;
        }
        #endregion PlayerInteractions
    }

    [System.Serializable]
    public class PlayerCalibrationInfo
    {
        public NetworkInstanceId NetworkInstanceIdPlayer { get; set; }
        public Vector3 LeftControllerPos { get; set; }
        public Vector3 RightControllerPos { get; set; }
    }
}
