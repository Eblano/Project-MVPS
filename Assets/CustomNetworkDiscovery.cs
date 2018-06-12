using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CustomNetworkDiscovery : NetworkDiscovery
{
    public static CustomNetworkDiscovery instance;
    private float timeTilDisconnect = 1.0f;
    private Dictionary<LanConnectionInfo, float> lanAddresses = new Dictionary<LanConnectionInfo, float>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("NO U");
            Destroy(this);
        }

        base.Initialize();
        base.StartAsClient();
        StartCoroutine(DeleteExpiredEntries());
    }

    public void ScanForGames()
    {
        StopBroadcast();
        base.Initialize();
        base.StartAsClient();
    }

    public void StartToBroadcast()
    {
        StopBroadcast();
        base.Initialize();
        base.StartAsServer();
        NetworkManager.singleton.StartHost();
    }

    private IEnumerator DeleteExpiredEntries()
    {
        while (true)
        {
            bool changed = false;
            List<LanConnectionInfo> keys = lanAddresses.Keys.ToList();

            foreach (LanConnectionInfo lanConnInfo in keys)
            {
                if (lanAddresses[lanConnInfo] <= Time.time)
                {
                    lanAddresses.Remove(lanConnInfo);
                    changed = true;
                }
            }

            if (changed)
            {
                UpdateDiscoveryList();
            }

            yield return new WaitForSeconds(timeTilDisconnect);
        }
    }

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        base.OnReceivedBroadcast(fromAddress, data);
        LanConnectionInfo info = new LanConnectionInfo(fromAddress, data);

        // Adds broadcast if it does not exists on network discovery list
        if (lanAddresses.ContainsKey(info) == false)
        {
            lanAddresses.Add(info, Time.time + timeTilDisconnect);
            UpdateDiscoveryList();
        }
        // Refresh time til disconnect if broadcast still exists
        else
        {
            lanAddresses[info] = Time.time + timeTilDisconnect;
        }
    }

    private void UpdateDiscoveryList()
    {
        // Adds to list of stuff
        AvailableLanList.HandleLanList(lanAddresses.Keys.ToList());
    }
}
