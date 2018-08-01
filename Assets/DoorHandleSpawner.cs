using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHandleSpawner : MonoBehaviour
{
    public static DoorHandleSpawner instance;
    private List<DoorHandle> doorHandles = new List<DoorHandle>();

    private void Start()
    {
        if (instance == null)
            instance = this;
    }

    public void SpawnHandles()
    {
        foreach(DoorHandle dH in doorHandles)
        {
            
        }
    }

    public void RegisterHandle(DoorHandle doorHandle)
    {
        doorHandles.Add(doorHandle);
    }
}
