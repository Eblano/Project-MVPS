using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorKnob : MonoBehaviour
{
    [SerializeField] private Door[] doors;

    public void ActivateDoor()
    {
        foreach (Door door in doors)
        {
            door.EnableDoorRot();
        }
    }
}
