using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TransformationSync : NetworkBehaviour
{
    private Vector3 prevPos;
    private Vector3 lerpPos;

    private Quaternion prevRot;
    private Quaternion lerpRot;

    private void Start()
    {
        prevPos = transform.position;
        prevRot = transform.rotation;
    }

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
        lerpPos = Vector3.Lerp(prevPos, position, 0.1f);
        lerpRot = Quaternion.Lerp(prevRot, Quaternion.Euler(eulerAngle), 0.1f);

        // Set the updated transform
        transform.position = lerpPos;
        transform.rotation = lerpRot;

        // Update previous transform
        prevPos = transform.position;
        prevRot = transform.rotation;
    }
}