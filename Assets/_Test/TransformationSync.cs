using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TransformationSync : NetworkBehaviour
{
    private void Update()
    {
        if (isServer)
        {
            RpcSyncTransform(transform.position, transform.eulerAngles);
        }
    }

    [ClientRpc]
    private void RpcSyncTransform(Vector3 position, Vector3 eulerAngle)
    {
        transform.position = position;
        transform.rotation = Quaternion.Euler(eulerAngle);
    }
}