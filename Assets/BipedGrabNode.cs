using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SealTeam4;

public class BipedGrabNode : InteractableObject
{
    [SerializeField] private BipedController BC;
    [SerializeField] private BipedController.BipedPosition bipPos;

    [SerializeField] private bool isBeingGrabbed = false;
    private bool grabStateChanged = false;

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
                BC.SetBiped(bipPos, this.transform, 1);
            }
            else
            {
                BC.SetBiped(bipPos, null, 0);
            }

            grabStateChanged = isBeingGrabbed;
        }
    }
}
