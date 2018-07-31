using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeirdGameManagerAssistant : NetworkBehaviour
{
    public static WeirdGameManagerAssistant instance;
    public List<IActions> allActions;
    public NetworkInstanceId playerID;

    private void Update()
    {
        if (isLocalPlayer)
            instance = this;
        else
            return;
    }

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
        ClientScene.objects[senderPlayerID].GetComponent<WeirdPlayerInteractionSync>().SyncControllerSnap(isLeftController, ClientScene.objects[childID].gameObject);
    }

    [ClientRpc]
    private void RpcUnSnapFromController(NetworkInstanceId senderPlayerID, bool isLeftController, Vector3 velo, Vector3 anguVelo)
    {
        ClientScene.objects[senderPlayerID].GetComponent<WeirdPlayerInteractionSync>().SyncControllerUnSnap(isLeftController, velo, anguVelo);
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
}