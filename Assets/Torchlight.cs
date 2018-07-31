using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SealTeam4;

public class Torchlight : MonoBehaviour, IUsableObject
{
    [SerializeField] private Light lightSource;
    private NetworkInstanceId torchNetID;

    private void Start()
    {
        torchNetID = GetComponent<NetworkIdentity>().netId;
    }

    public void UseObject(NetworkInstanceId networkInstanceId)
    {
        GameManagerAssistant.instance.RelaySenderCmdTorchlightState(torchNetID, !lightSource.enabled);
        SetLightState(!lightSource.enabled);
    }

    public void SetLightState(bool state)
    {
        lightSource.enabled = state;
    }
}
