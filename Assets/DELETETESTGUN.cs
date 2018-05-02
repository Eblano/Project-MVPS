using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class DELETETESTGUN : MonoBehaviour
{
    private Transform firstHand;
    private Transform secondHand;
    //[SerializeField] private bool noU;

    private void Start()
    {
        firstHand = VRTK_DeviceFinder.DeviceTransform(VRTK_DeviceFinder.Devices.LeftController);
        secondHand = VRTK_DeviceFinder.DeviceTransform(VRTK_DeviceFinder.Devices.RightController);
    }

    private void Update()
    {
        transform.position = firstHand.position;
        CalculateGunRotation();
    }

    private void CalculateGunRotation()
    {
        
    }
}
