using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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
        WeirdGameManagerAssistant.instance.RelaySenderCmdTorchlightState(torchNetID, !lightSource.enabled);
        SetLightState(!lightSource.enabled);
    }

    public void SetLightState(bool state)
    {
        lightSource.enabled = state;
    }
}
