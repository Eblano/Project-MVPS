using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class VRHapticsSync : NetworkBehaviour
{
    public enum HapticType { GUNFIRE, GUNRELOAD }

    [Command]
    public void CmdSyncHaps(NetworkInstanceId networkInstanceId, HapticType hapticType)
    {
        TargetSyncHaps(NetworkServer.objects[networkInstanceId].connectionToClient, hapticType);
    }

    [TargetRpc]
    public void TargetSyncHaps(NetworkConnection networkConnection, HapticType hapticType)
    {
        PlayHaptic(hapticType);
    }

    private void PlayHaptic(HapticType hapticType)
    {
        switch (hapticType)
        {
            case HapticType.GUNFIRE:
                break;
            case HapticType.GUNRELOAD:
                break;
            default:
                break;
        }
    }
}
