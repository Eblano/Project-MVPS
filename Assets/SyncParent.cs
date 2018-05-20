using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SyncParent : NetworkBehaviour
{
    [ClientRpc]
    public void RpcSetParent(GameObject parent)
    {
        transform.SetParent(parent.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
}
