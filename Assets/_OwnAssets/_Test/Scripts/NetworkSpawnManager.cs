using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public static class NetworkSpawnManager
{
    [ClientRpc]
    public static Vector3 SyncRotation(Vector3 syncRotation)
    {
        return syncRotation;
    }
}
