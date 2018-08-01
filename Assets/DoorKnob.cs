using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SealTeam4;

public class DoorKnob : MonoBehaviour
{
    [SerializeField] private Door[] doors;

    private void Start()
    {
        DoorKnobHandler.instance.AddKnob(this);
    }

    public void ActivateDoor()
    {
        DoorKnobHandler.instance.SendKnobState(this);
        OpenDoors();
    }

    public void OpenDoors()
    {
        foreach (Door door in doors)
        {
            door.EnableDoorRot();
        }
    }
}
