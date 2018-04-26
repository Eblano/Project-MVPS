using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using VRTK;

public class CombinedControllerInputs : MonoBehaviour
{
    private VRTK_ControllerReference lHandRef;
    private VRTK_ControllerReference rHandRef;
    private ServerSync ss;

    private void Start()
    {
        ss = GetComponent<ServerSync>();

        if (!ss.isLocalPlayer)
        {
            Destroy(this);
            return;
        }

        VRTK_ControllerEvents lHandEvents = VRTK_DeviceFinder.DeviceTransform(VRTK_DeviceFinder.Devices.LeftController).GetComponent<VRTK_ControllerEvents>();
        VRTK_ControllerEvents rHandEvents = VRTK_DeviceFinder.DeviceTransform(VRTK_DeviceFinder.Devices.RightController).GetComponent<VRTK_ControllerEvents>();

        lHandRef = VRTK_DeviceFinder.GetControllerReferenceLeftHand();
        rHandRef = VRTK_DeviceFinder.GetControllerReferenceRightHand();

        lHandEvents.GripClicked += LHandEvents_GripClicked;
        rHandEvents.GripClicked += RHandEvents_GripClicked;

        lHandEvents.GripUnclicked += LHandEvents_GripUnclicked;
        rHandEvents.GripUnclicked += RHandEvents_GripUnclicked;

        lHandEvents.TriggerClicked += LHandEvents_TriggerClicked;
        rHandEvents.TriggerClicked += RHandEvents_TriggerClicked;
    }

    private void RHandEvents_TriggerClicked(object sender, ControllerInteractionEventArgs e)
    {

    }

    private void LHandEvents_TriggerClicked(object sender, ControllerInteractionEventArgs e)
    {

    }

    private void RHandEvents_GripUnclicked(object sender, ControllerInteractionEventArgs e)
    {
        ss.CmdCallUngrab(VRTK_DeviceFinder.Devices.RightController, VRTK_DeviceFinder.GetControllerVelocity(rHandRef), VRTK_DeviceFinder.GetControllerAngularVelocity(rHandRef));
    }

    private void LHandEvents_GripUnclicked(object sender, ControllerInteractionEventArgs e)
    {
        ss.CmdCallUngrab(VRTK_DeviceFinder.Devices.LeftController, VRTK_DeviceFinder.GetControllerVelocity(lHandRef), VRTK_DeviceFinder.GetControllerAngularVelocity(lHandRef));
    }

    private void RHandEvents_GripClicked(object sender, ControllerInteractionEventArgs e)
    {
        ss.CmdCallGrab(VRTK_DeviceFinder.Devices.RightController, 1.0f);
    }

    private void LHandEvents_GripClicked(object sender, ControllerInteractionEventArgs e)
    {
        ss.CmdCallGrab(VRTK_DeviceFinder.Devices.LeftController, 1.0f);
    }
}
