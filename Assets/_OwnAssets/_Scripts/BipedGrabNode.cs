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
    
    private void Start()
    {
        BipedManager.instance.RegisterNode(this);
    }

    public void OnGrabbed(NetworkInstanceId playerId, bool isLeft)
    {
        BipedManager.instance.SendOnGrab(this, isLeft);
        OnGrabbedSync(ClientScene.objects[playerId].GetComponent<PlayerInteractionSync>().GetControllerTransform(isLeft));
    }

    public void OnGrabbedSync(Transform parent)
    {
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
        BC.SetBiped(bipPos, null, 0);
        aIController.SetGrabModeTransform(null);
    }
}
