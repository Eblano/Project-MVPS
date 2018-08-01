using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHandle : MonoBehaviour
{
    private void Start()
    {
        DoorHandleSpawner.instance.RegisterHandle(this);
    }
}
