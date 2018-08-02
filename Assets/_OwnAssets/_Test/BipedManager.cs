using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SealTeam4;

public class BipedManager : MonoBehaviour
{
    public static BipedManager instance;
    public List<BipedGrabNode> bipedGrabNodes = new List<BipedGrabNode>();

    private void Start()
    {
        if (instance == null)
            instance = this;
    }

    public void RegisterNode(BipedGrabNode node)
    {
        bipedGrabNodes.Add(node);
    }

    public void SendOnGrab(BipedGrabNode node, bool isLeft)
    {
        GameManagerAssistant.instance.RelaySenderCmdSnapBiped(bipedGrabNodes.IndexOf(node), isLeft);
    }

    public void SendUngrab(BipedGrabNode node)
    {
        GameManagerAssistant.instance.RelaySenderCmdUnSnapBiped(bipedGrabNodes.IndexOf(node));
    }

    public void CallOnGrab(int index, Transform grabParent)
    {
        bipedGrabNodes[index].OnGrabbedSync(grabParent);
    }

    public void CallUnGrab(int index)
    {
        bipedGrabNodes[index].OnUngrabbedSync();
    }
}
