using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeirdGameManagerAssistant : NetworkBehaviour
{
    public static WeirdGameManagerAssistant instance;
    public List<IActions> allActions;

    private void Update()
    {
        if (isLocalPlayer)
            instance = this;
        else
            return;
    }

    //[Command]
    //public void CmdSnapToParentGameObj(NetworkInstanceId childID, NetworkInstanceId parentID)
    //{
    //    RpcSnapToParentGameObj(childID, parentID);
    //}

    [Command]
    public void CmdSnapToParentGameObj(NetworkInstanceId childID, NetworkInstanceId parentID, Vector3 offset)
    {
        RpcSnapToParentGameObj(childID, parentID, offset);
    }

    [Command]
    public void CmdSnapToController(NetworkInstanceId childID, NetworkInstanceId playerID, bool isLeftController)
    {
        RpcSnapToController(childID, playerID, isLeftController);
    }

    [Command]
    public void CmdUnSnapFromController(NetworkInstanceId playerID, bool isLeftController, Vector3 velo, Vector3 anguVelo)
    {
        RpcUnSnapFromController(playerID, isLeftController, velo, anguVelo);
    }

    //[ClientRpc]
    //public void RpcSnapToParentGameObj(NetworkInstanceId childID, NetworkInstanceId parentID)
    //{
    //    SnapTo(NetworkServer.objects[childID].gameObject, NetworkServer.objects[parentID].gameObject);
    //}

    [ClientRpc]
    public void RpcSnapToParentGameObj(NetworkInstanceId childID, NetworkInstanceId parentID, Vector3 offset)
    {
        SnapTo(ClientScene.objects[childID].gameObject, ClientScene.objects[parentID].gameObject, offset);
    }

    [ClientRpc]
    public void RpcSnapToController(NetworkInstanceId childID, NetworkInstanceId playerID, bool isLeftController)
    {
        ClientScene.objects[playerID].GetComponent<WeirdPlayerInteractionSync>().SyncControllerSnap(isLeftController, ClientScene.objects[childID].gameObject);
    }

    [ClientRpc]
    public void RpcUnSnapFromController(NetworkInstanceId playerID, bool isLeftController, Vector3 velo, Vector3 anguVelo)
    {
        ClientScene.objects[playerID].GetComponent<WeirdPlayerInteractionSync>().SyncControllerUnSnap(isLeftController, velo, anguVelo);
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
