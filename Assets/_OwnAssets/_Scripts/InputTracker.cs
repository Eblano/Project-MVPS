using UnityEngine;
using UnityEngine.Networking;
using VRTK;

public class InputTracker : MonoBehaviour
{
    private Transform headset, lHandCont, rHandCont;
    private Vector3AndQuaternion head, lHand, rHand;
    private ServerSync ss;

    private void OnEnable()
    {
        ss = GetComponent<ServerSync>();
        head = new Vector3AndQuaternion();
        lHand = new Vector3AndQuaternion();
        rHand = new Vector3AndQuaternion();

        headset = VRTK_DeviceFinder.DeviceTransform(VRTK_DeviceFinder.Devices.Headset);
        lHandCont = VRTK_DeviceFinder.DeviceTransform(VRTK_DeviceFinder.Devices.LeftController);
        rHandCont = VRTK_DeviceFinder.DeviceTransform(VRTK_DeviceFinder.Devices.RightController);
    }

    private void Update()
    {
        head.SetPosAndRot(headset);
        lHand.SetPosAndRot(lHandCont);
        rHand.SetPosAndRot(rHandCont);
        ss.CmdSyncVRTransform(head, lHand, rHand);
    }
}
