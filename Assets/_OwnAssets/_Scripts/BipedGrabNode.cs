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
    [SerializeField] private float snapBackRadius;

    private Transform parent;
    private Vector3 originalPos;
    private Quaternion originalRot;

    private Transform bipedTransform;

    private void Start()
    {
        bipedTransform = BC.GetBipedPos(bipPos);
        parent = transform.parent;
        originalPos = transform.localPosition;
        originalRot = transform.localRotation;
    }

    public void OnGrabbed(Transform grabParent)
    {
        transform.SetParent(grabParent);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        BC.SetBiped(bipPos, this.transform, 1);
        aIController.SetGrabModeTransform(transform);
    }

    public void OnUngrabbed()
    {
        transform.SetParent(parent);
        BC.SetBiped(bipPos, null, 0);
        transform.localPosition = originalPos;
        transform.localRotation = originalRot;
        aIController.SetGrabModeTransform(null);
    }

    private bool WithinRadius()
    {
        return Vector3.Distance(bipedTransform.position, transform.position) < snapBackRadius;
    }
}
