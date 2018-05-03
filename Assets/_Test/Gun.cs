using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SealTeam4;

public class Gun : NetworkBehaviour, IUsableObject, ITwoHandedObject
{
    [SerializeField] private GameObject spawnPref;
    [SerializeField] private Transform firingPoint;
    [SerializeField] private Transform secondHandTransform;
    private Collider secondHandGrabCollider;
    private InteractableObject interactableObject;
    private bool isTwoHandedGrab = false;
    private bool isBeingGrabbed = false;
    private bool grabStateChanged = false;

    private void Start()
    {
        interactableObject = GetComponent<InteractableObject>();
        if (secondHandTransform)
        {
            secondHandGrabCollider = secondHandTransform.GetComponent<Collider>();
        }
    }

    private void Update()
    {
        if (isTwoHandedGrab)
        {
            CmdCalculateGunRotation();
        }

        if (secondHandTransform)
        {
            CheckGrabState();
        }
    }

    private void CheckGrabState()
    {
        if (interactableObject.GetOwner() != null)
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
                secondHandGrabCollider.enabled = true;
            }
            else
            {
                secondHandGrabCollider.enabled = false;
            }

            grabStateChanged = isBeingGrabbed;
        }
    }

    public void UseObject()
    {
        CmdFireGun();
    }

    [Command]
    private void CmdFireGun()
    {
        RpcFireGun();
    }

    [Command]
    private void CmdCalculateGunRotation()
    {
        RpcCalculateGunRotation();
    }

    [Command]
    private void CmdResetGunRotation()
    {
        RpcResetGunRotation();
    }

    [ClientRpc]
    private void RpcFireGun()
    {
        RaycastHit hit;
        if (Physics.Raycast(firingPoint.position, firingPoint.forward, out hit, Mathf.Infinity, ~LayerMask.NameToLayer("Shootable")))
        {
            Instantiate(spawnPref, hit.point, firingPoint.rotation);
        }
    }

    [ClientRpc]
    private void RpcCalculateGunRotation()
    {
        transform.rotation = Quaternion.FromToRotation(Vector3.forward, secondHandTransform.position - transform.position);
    }

    [ClientRpc]
    private void RpcResetGunRotation()
    {
        transform.rotation = Quaternion.identity;
    }

    public void SecondHandActive()
    {
        isTwoHandedGrab = true;
    }

    public void SecondHandInactive()
    {
        isTwoHandedGrab = false;
        CmdResetGunRotation();
    }
}