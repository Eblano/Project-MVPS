using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using SealTeam4;

public class ControllerHapticsManager : MonoBehaviour
{
    public enum HapticType { GUNFIRE, GUNRELOAD }
    public static ControllerHapticsManager instance;

    private void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        //lHandRef = VRTK_DeviceFinder.GetControllerReferenceLeftHand();
        //rHandRef = VRTK_DeviceFinder.GetControllerReferenceRightHand();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            PlayHaptic(HapticType.GUNFIRE, VRTK_DeviceFinder.Devices.LeftController);
            PlayHaptic(HapticType.GUNRELOAD, VRTK_DeviceFinder.Devices.RightController);
        }
    }

    public void PlayHaptic(HapticType hapticType, VRTK_DeviceFinder.Devices devices)
    {
        Debug.Log("PlayHaptic Attempt" + hapticType + " " + devices);

        VRTK_ControllerReference reference = null;

        switch (devices)
        {
            case VRTK_DeviceFinder.Devices.LeftController:
                reference = PlayerInput.lHandRef;
                break;
            case VRTK_DeviceFinder.Devices.RightController:
                reference = PlayerInput.rHandRef;
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
