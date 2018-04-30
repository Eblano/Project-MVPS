using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkSetup : NetworkBehaviour
{
    [SerializeField] Behaviour[] disable;

    private void Start()
    {
        if (isServer || !isLocalPlayer)
        {
            DisableComponents();
        }
        
        gameObject.name = "Player " + GetComponent<NetworkIdentity>().netId;
    }

    private void DisableComponents()
    {
        foreach (Behaviour b in disable)
        {
            b.enabled = false;
        }
    }
}
