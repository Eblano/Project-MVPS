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
        if (doors[0].isClosed)
        {
            DoorKnobHandler.instance.SendKnobState(this, true);
            OpenDoors();
        }
        else
        {
            DoorKnobHandler.instance.SendKnobState(this, false);
            CloseDoors();
        }
    }

    public void OpenDoors()
    {
        foreach (Door door in doors)
        {
            door.EnableDoorRot();
        }
    }

    public void CloseDoors()
    {
        foreach (Door door in doors)
        {
            door.CloseDoor();
        }
    }
}
