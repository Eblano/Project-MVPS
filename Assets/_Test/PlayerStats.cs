using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SealTeam4;

public class PlayerStats : MonoBehaviour
{
    private int hitPoints = 100;
    private NetworkInstanceId netID;

    private void Start()
    {
        if (GameManagerAssistant.instance.isServer)
        {
            Destroy(this);
        }

        netID = GetComponent<NetworkIdentity>().netId;
    }

    public void TakeDamage(int damage)
    {
        hitPoints -= damage;
        UpdateOverlay();
        CheckHealth();
    }

    private void UpdateOverlay()
    {
        GameManagerAssistant.instance.TargetUpdatePanelTransparency(NetworkServer.objects[netID].connectionToClient, hitPoints);
    }

    private void CheckHealth()
    {
        if (hitPoints <= 0)
        {
            // Change overlay transparency with gma

        }
    }
}