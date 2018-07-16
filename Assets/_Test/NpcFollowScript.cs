using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcFollowScript : MonoBehaviour
{
    [SerializeField] private float followThreshholdRadius;
    private Transform grabbedNode;
    private bool isNodeGrabbed;

    private bool WithinThreshold()
    {
        return Vector3.Distance(grabbedNode.position, transform.position) > followThreshholdRadius;
    }

    public void SetNodeGrabState(bool state)
    {
        isNodeGrabbed = state;
    }

    private void Update()
    {
        if (isNodeGrabbed && WithinThreshold())
        {
            // This will be the position for npc to follow 
            // grabbedNode.position;

        }
    }
}