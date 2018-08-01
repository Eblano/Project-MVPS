using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllPublic)]
public class Door : MonoBehaviour
{
    [SerializeField] private bool isLocked;
    [SerializeField] private float doorCloseAngle;
    [SerializeField] private Vector3 openingTorque;

    [SerializeField] private bool isMaxClosedState;

    private HingeJoint doorHinge;
    private Quaternion closedRot;
    private Rigidbody rb;
    private float doorTimer;
    private float minAngle;
    private float maxAngle;

    private void Start()
    {
        closedRot = transform.localRotation;
        rb = GetComponent<Rigidbody>();
        doorHinge = GetComponent<HingeJoint>();
        minAngle = doorHinge.limits.min;
        maxAngle = doorHinge.limits.max;
    }

    public void EnableDoorRot()
    {
        if (!isLocked)
        {
            rb.isKinematic = false;
            rb.AddTorque(openingTorque);
            Debug.Log("Helll");
            StartTimer();
        }
        else
        {
            //Door locked sound
        }
    }

    public void CloseDoor()
    {
        rb.AddTorque(-openingTorque);
    }

    private void CheckDoorClosingAngle()
    {
        if (isMaxClosedState)
        {
            if (doorHinge.angle > maxAngle - doorCloseAngle && Time.time > doorTimer)
            {
                rb.isKinematic = true;
                transform.localRotation = closedRot;
            }
        }
        else
        {
            if (doorHinge.angle < minAngle + doorCloseAngle && Time.time > doorTimer)
            {
                rb.isKinematic = true;
                transform.localRotation = closedRot;
            }
        }
    }

    private void StartTimer()
    {
        doorTimer = Time.time + 2;
    }

    private void Update()
    {
        CheckDoorClosingAngle();
    }
}