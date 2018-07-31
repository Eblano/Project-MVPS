using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using SealTeam4;

public static class ControllerHapticsManager
{
    public enum HapticType { GUNFIRE, GUNRELOAD }

    public static void PlayHaptic(HapticType hapticType, VRTK_DeviceFinder.Devices devices)
    {
        Debug.Log("PlayHaptic Attempt" + hapticType + " " + devices);

        VRTK_ControllerReference reference = null;

        switch (devices)
        {
            case VRTK_DeviceFinder.Devices.LeftController:
                reference = WeirdPlayerInput.lHandRef;
                break;
            case VRTK_DeviceFinder.Devices.RightController:
                reference = WeirdPlayerInput.rHandRef;
                break;
        }

        switch (hapticType)
        {
            case HapticType.GUNFIRE:
                VRTK_ControllerHaptics.TriggerHapticPulse(reference, 1);
                Debug.Log("Fire " + reference);
                break;
            case HapticType.GUNRELOAD:
                //VRTK_ControllerHaptics.TriggerHapticPulse(reference, 0.8f);
                VRTK_ControllerHaptics.TriggerHapticPulse(reference, 1);
                Debug.Log("Reload " + reference);
                break;
            default:
                break;
        }
    }
}
