using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SealTeam4;

public class DoorGrabNode : InteractableObject
{
    [SerializeField] private bool isBeingGrabbed = false;
    private bool grabStateChanged = false;
    [SerializeField] private List<Door> doors = new List<Door>();

    private void Update()
    {
        CheckGrabState();
    }

    private void CheckGrabState()
    {
        if (GetOwner() != null)
        {
            isBeingGrabbed = true;
        }
        else
        {
            isBeingGrabbed = false;
        }

        if (grabStateChanged != isBeingGrabbed)
        {
            if (isBeingGrabbed)
            {
                foreach(Door door in doors)
                {
                    door.EnableDoorRot();
                }
            }

            grabStateChanged = isBeingGrabbed;
        }
    }

    public void Initialise(ref List<Door> doors)
    {
        this.doors = doors;
    }
}
