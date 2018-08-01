using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TransformationSync : NetworkBehaviour
{
    [SerializeField] private float sendRatePerSecond = 45;
    [SerializeField] private float moveThreshold = 0.005f;
    [SerializeField] private float angularThreshold = 0.01f;
    private float counter = float.MinValue;
    private Vector3 prevPos = Vector3.positiveInfinity;
    private Quaternion prevRot = Quaternion.identity;

    private void FixedUpdate()
    {
        if (isServer)
        {
            //if (Vector3.Distance(prevPos, transform.position) > moveThreshold || Quaternion.Angle(prevRot, transform.rotation) > angularThreshold)
            //{
            if (IsCounterReady(ref counter, 1 / sendRatePerSecond))
            {
                RpcSyncTransform(transform.position, transform.eulerAngles);
                prevPos = transform.position;
                prevRot = transform.rotation;
            }
            //}
        }
    }

    private bool IsCounterReady(ref float timeCounter, float downTime)
    {
        if (Time.time > timeCounter)
        {
            timeCounter = Time.time + downTime;
            return true;
        }

        return false;
    }

    [ClientRpc]
    private void RpcSyncTransform(Vector3 position, Vector3 eulerAngle)
    {
        // Smooth the transform
        transform.position = Vector3.Lerp(transform.position, position, 0.5f);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(eulerAngle), 0.5f);
    }
}