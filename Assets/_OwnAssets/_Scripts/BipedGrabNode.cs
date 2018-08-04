using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SealTeam4;

public class BipedGrabNode : MonoBehaviour
{
    [SerializeField] private BipedController BC;
    [SerializeField] private AIController aIController;
    [SerializeField] private BipedController.BipedPosition bipPos;
    private Transform parent;
    private float originlRadOffSet;
    private float detachRadiusThreshold = 0.15f;
    private Transform grabber;
    
    private void Start()
    {
        BipedManager.instance.RegisterNode(this);
        parent = transform.parent;
        originlRadOffSet = Vector3.Distance(parent.position, transform.position);
    }

    private void Update()
    {
        if (grabber)
        {
            if (Vector3.Distance(grabber.position, parent.position) > originlRadOffSet + detachRadiusThreshold)
            {
                OnUngrabbed();
            }
        }
    }

    public void OnGrabbed(NetworkInstanceId playerId, bool isLeft)
    {
        BipedManager.instance.SendOnGrab(this, isLeft);
        OnGrabbedSync(ClientScene.objects[playerId].GetComponent<PlayerInteractionSync>().GetControllerTransform(isLeft));
    }

    public void OnGrabbedSync(Transform parent)
    {
        grabber = transform;
        BC.SetBiped(bipPos, parent, 1);
        aIController.SetGrabModeTransform(transform);
    }

    public void OnUngrabbed()
    {
        BipedManager.instance.SendUngrab(this);
        OnUngrabbedSync();
    }

    public void OnUngrabbedSync()
    {
        grabber = null;
        BC.SetBiped(bipPos, null, 0);
        aIController.SetGrabModeTransform(null);
    }
}
