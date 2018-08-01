using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SealTeam4;

[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllPublic)]
public class DoorHandle : MonoBehaviour
{
    [SerializeField] private Transform parentObj;
    [SerializeField] private GameObject grabNodePref;
    [SerializeField] Door[] doors;

    private void Start()
    {
        DoorHandleSpawner.instance.RegisterHandle(this);
    }

    public void InstantiateGrabNode(int index)
    {
        GameObject doorGrabNode = Instantiate(grabNodePref);
        GameManagerAssistant.instance.NetworkSpawnGameObj(doorGrabNode);
        GameManagerAssistant.instance.RpcSetUpDoorHandle(index, doorGrabNode.GetComponent<NetworkIdentity>().netId);
        SetUpKnob(doorGrabNode);
    }

    public void SetUpKnob(GameObject syncObj)
    {
        syncObj.transform.SetParent(parentObj);
        syncObj.transform.localPosition = transform.localPosition;
        syncObj.transform.localRotation = transform.localRotation;
        syncObj.GetComponent<DoorGrabNode>().Initialise(ref doors);
    }
}
