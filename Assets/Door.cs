using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private bool isLocked;
    [SerializeField] private float doorCloseAngle;
    [SerializeField] private Vector3 openingTorque;

    private HingeJoint doorHinge;
    private Quaternion closedRot;
    private Rigidbody rb;
    private float doorTimer;

    private void Start()
    {
        closedRot = transform.localRotation;
        rb = GetComponent<Rigidbody>();
        doorHinge = GetComponent<HingeJoint>();
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

    private void CheckDoorClosingAngle()
    {
        if (doorHinge.angle < doorCloseAngle && Time.time > doorTimer)
        {
            rb.isKinematic = true;
            transform.localRotation = closedRot;
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