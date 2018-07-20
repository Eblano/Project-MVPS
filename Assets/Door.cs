using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private bool isLocked;
    [SerializeField] private float doorCloseAngle;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetDoorFreeRot(bool doorState)
    {
        if (isLocked && doorState)
        {
            rb.isKinematic = false;
        }
    }

    private void CheckDoorClosingAngle()
    {

    }

    private void Update()
    {

    }
}