using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SealTeam4;

public class BipedGrabNode : InteractableObject
{
    [SerializeField] private BipedController BC;
    [SerializeField] private AIController aIController;
    [SerializeField] private BipedController.BipedPosition bipPos;
    [SerializeField] private float snapBackRadius;

    private Transform bipedTransform;

    [SerializeField] private bool isBeingGrabbed = false;
    private bool grabStateChanged = false;

    private void Start()
    {
        bipedTransform = BC.GetBipedPos(bipPos);
    }

    private void Update()
    {
        CheckGrabState();
    }

    private void CheckGrabState()
    {
        if (GetOwner() != null && WithinRadius())
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
                aIController.SetGrabModeTransform(transform);
            }
            else
            {
                BC.SetBiped(bipPos, null, 0);
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
                aIController.SetGrabModeTransform(null);
            }

            grabStateChanged = isBeingGrabbed;
        }
    }

    private bool WithinRadius()
    {
        return Vector3.Distance(bipedTransform.position, transform.position) < snapBackRadius;
    }
}
