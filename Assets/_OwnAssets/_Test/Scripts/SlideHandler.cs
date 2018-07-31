using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SealTeam4;


public class SlideHandler : MonoBehaviour
{
    [SerializeField] private Transform slideObject;
    [SerializeField] private float xMin, xMax, yMin, yMax, zMin, zMax;

    private enum ActiveAxis { X, Y, Z };

    [SerializeField] private ActiveAxis currActiveAxis;
    [SerializeField] private bool isMaxActive;
    [SerializeField] private float activeThreshold;

    [SerializeField] private Gun gun;

    private Vector3 clampLocPos;
    private Vector3 initialSlidePos;
    private Vector3 initialPos;
    private Vector3 initialRot;

    [SerializeField] private bool activeState = false;
    private bool activeStateChanged = false;

    [SerializeField] private bool follow = false;

    private NetworkInstanceId ownerNetID;

    private void Start()
    {
        initialSlidePos = slideObject.localPosition;
        initialPos = transform.localPosition;
        initialRot = transform.eulerAngles;
    }

    public void OnUngrabbed(NetworkInstanceId playerNetID)
    {
        ownerNetID = NetworkInstanceId.Invalid;
        follow = false;
        slideObject.localPosition = initialSlidePos;
    }

    public void OnGrabbed(NetworkInstanceId playerNetID)
    {
        ownerNetID = playerNetID;
        follow = true;
    }

    public void ResetLocalPos()
    {
        transform.localPosition = initialPos;
        transform.eulerAngles = initialRot;
    }

    private void Update()
    {
        if (follow)
        {
            FollowPosition();
        }
    }

    private void FollowPosition()
    {
        Vector3 newPos = transform.position - slideObject.position;
        Vector3 slideLocPos = slideObject.localPosition;

        slideObject.Translate(newPos, Space.Self);

        Vector3 clampPos = slideObject.localPosition;
        clampPos.x = Mathf.Clamp(clampPos.x, xMin, xMax);
        clampPos.y = Mathf.Clamp(clampPos.y, yMin, yMax);
        clampPos.z = Mathf.Clamp(clampPos.z, zMin, zMax);
        
        activeState = IsSlideActive();

        if (activeStateChanged != activeState)
        {
            if (activeState && GameManagerAssistant.instance.playerID == ownerNetID)
            {
                gun.LoadChamber();
            }

            activeStateChanged = activeState;
        }

        slideObject.localPosition = clampPos;
    }

    private bool IsSlideActive()
    {
        switch (currActiveAxis)
        {
            case ActiveAxis.X:
                if (slideObject.localPosition.x < xMin + activeThreshold)
                {
                    return true;
                }
                break;
            case ActiveAxis.Y:
                if (slideObject.localPosition.y < yMin + activeThreshold)
                {
                    return true;
                }
                break;
            case ActiveAxis.Z:
                if (slideObject.localPosition.z < zMin + activeThreshold)
                {
                    return true;
                }
                break;
        }
        return false;
    }
}