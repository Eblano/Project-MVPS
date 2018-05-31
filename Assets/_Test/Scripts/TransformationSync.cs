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
        // Smooth the transform
        transform.position = Vector3.Lerp(transform.position, position, 0.1f);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(eulerAngle), 0.1f);
    }
}