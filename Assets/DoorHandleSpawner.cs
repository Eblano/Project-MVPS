using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SealTeam4;

public class DoorHandleSpawner : MonoBehaviour
{
    public static DoorHandleSpawner instance;
    public List<DoorHandle> doorHandles = new List<DoorHandle>();
    private bool done = false;

    private void Start()
    {
        if (instance == null)
            instance = this;
    }

    private void Update()
    {
        if (GameManagerAssistant.instance && GameManagerAssistant.instance.isServer && !done)
        {
            SpawnHandles();
            done = true;
        }
    }

    public void SpawnHandles()
    {
        for(int i = 0; i < doorHandles.Count; i++)
        {
            if(doorHandles[i] == null)
            {
                continue;
            }
            doorHandles[i].InstantiateGrabNode(i);
        }

    }

    public void RegisterHandle(DoorHandle doorHandle)
    {
        doorHandles.Add(doorHandle);
    }
}
