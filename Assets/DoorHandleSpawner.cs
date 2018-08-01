using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHandleSpawner : MonoBehaviour
{
    public static DoorHandleSpawner instance;
    [HideInInspector] public List<DoorHandle> doorHandles = new List<DoorHandle>();

    private void Start()
    {
        if (instance == null)
            instance = this;
    }

    public void SpawnHandles()
    {
        for(int i = 0; i < doorHandles.Count; i++)
        {
            doorHandles[i].InstantiateGrabNode(i);
        }
    }

    public void RegisterHandle(DoorHandle doorHandle)
    {
        doorHandles.Add(doorHandle);
    }
}
