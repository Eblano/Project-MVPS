using UnityEngine;
using UnityEngine.Networking;
using VRTK;
using System.Collections;

namespace SealTeam4
{
    public class PlayerInput : MonoBehaviour
    {
        private VRTK_ControllerEvents lHandEvents;
        private VRTK_ControllerEvents rHandEvents;
        public static VRTK_ControllerReference lHandRef;
        public static VRTK_ControllerReference rHandRef;
        private PlayerInteractionSync playerInteractionSync;

        private Transform headset, lHandCont, rHandCont;
        private Vector3AndQuaternion head, lHand, rHand;

        private NetworkIdentity networkIdentity;
        
        [SerializeField] private float grabRadius;
        [SerializeField] private float holdTouchPadTimer;

        private float leftTimer;
        private float rightTimer;

        private void Start()
        {
            networkIdentity = GetComponent<NetworkIdentity>();
            string playerName = "Player " + networkIdentity.netId;
            gameObject.name = playerName;

            playerInteractionSync = GetComponent<PlayerInteractionSync>();

            if (!playerInteractionSync.isLocalPlayer)
            {
                Destroy(this);
                return;
            }

            GameManager.instance.SetLocalPlayerName(playerName);

            head = new Vector3AndQuaternion();
            lHand = new Vector3AndQuaternion();
            rHand = new Vector3AndQuaternion();
            headset = VRTK_DeviceFinder.DeviceTransform(VRTK_DeviceFinder.Devices.Headset);
            lHandCont = VRTK_DeviceFinder.DeviceTransform(VRTK_DeviceFinder.Devices.LeftController);
            rHandCont = VRTK_DeviceFinder.DeviceTransform(VRTK_DeviceFinder.Devices.RightController);

            lHandEvents = lHandCont.GetComponent<VRTK_ControllerEvents>();
            rHandEvents = rHandCont.GetComponent<VRTK_ControllerEvents>();

            StartCoroutine(SetController());

            lHandEvents.GripClicked += LHandEvents_GripClicked;
            rHandEvents.GripClicked += RHandEvents_GripClicked;

            lHandEvents.GripUnclicked += LHandEvents_GripUnclicked;
            rHandEvents.GripUnclicked += RHandEvents_GripUnclicked;

            lHandEvents.TriggerClicked += LHandEvents_TriggerClicked;
            rHandEvents.TriggerClicked += RHandEvents_TriggerClicked;

            lHandEvents.TouchpadPressed += LHandEvents_TouchpadPressed;
            rHandEvents.TouchpadPressed += RHandEvents_TouchpadPressed;

            lHandEvents.TouchpadReleased += LHandEvents_TouchpadReleased;
            rHandEvents.TouchpadReleased += RHandEvents_TouchpadReleased;
        }

        private void Update()
        {
            head.SetPosAndRot(headset);
            lHand.SetPosAndRot(lHandCont);
            rHand.SetPosAndRot(rHandCont);
            playerInteractionSync.CmdSyncVRTransform(head, lHand, rHand);
        }

        IEnumerator SetController()
        {
            while (!VRTK_ControllerReference.IsValid(lHandRef) || !VRTK_ControllerReference.IsValid(rHandRef))
            {
                lHandRef = VRTK_DeviceFinder.GetControllerReferenceLeftHand();
                rHandRef = VRTK_DeviceFinder.GetControllerReferenceRightHand();

                yield return new WaitForSeconds(0.5f);
            }

            Debug.Log(lHandRef);
            Debug.Log(rHandRef);
        }

        private void RHandEvents_TriggerClicked(object sender, ControllerInteractionEventArgs e)
        {
            playerInteractionSync.CmdTriggerClick(VRTK_DeviceFinder.Devices.RightController, networkIdentity.netId);
        }

        private void LHandEvents_TriggerClicked(object sender, ControllerInteractionEventArgs e)
        {
            playerInteractionSync.CmdTriggerClick(VRTK_DeviceFinder.Devices.LeftController, networkIdentity.netId);
        }

        private void RHandEvents_GripUnclicked(object sender, ControllerInteractionEventArgs e)
        {
            playerInteractionSync.CmdCallUngrab(VRTK_DeviceFinder.Devices.RightController, VRTK_DeviceFinder.GetControllerVelocity(rHandRef), VRTK_DeviceFinder.GetControllerAngularVelocity(rHandRef));
        }

        private void LHandEvents_GripUnclicked(object sender, ControllerInteractionEventArgs e)
        {
            playerInteractionSync.CmdCallUngrab(VRTK_DeviceFinder.Devices.LeftController, VRTK_DeviceFinder.GetControllerVelocity(lHandRef), VRTK_DeviceFinder.GetControllerAngularVelocity(lHandRef));
        }

        private void RHandEvents_GripClicked(object sender, ControllerInteractionEventArgs e)
        {
            playerInteractionSync.CmdCallGrab(VRTK_DeviceFinder.Devices.RightController, grabRadius);
        }

        private void LHandEvents_GripClicked(object sender, ControllerInteractionEventArgs e)
        {
            playerInteractionSync.CmdCallGrab(VRTK_DeviceFinder.Devices.LeftController, grabRadius);
        }

        private void RHandEvents_TouchpadPressed(object sender, ControllerInteractionEventArgs e)
        {
            playerInteractionSync.CmdCallTouchpadButton(VRTK_DeviceFinder.Devices.RightController, rHandEvents.GetTouchpadAxis());
            TouchPadDown(VRTK_DeviceFinder.Devices.RightController);
        }

        private void LHandEvents_TouchpadPressed(object sender, ControllerInteractionEventArgs e)
        {
            playerInteractionSync.CmdCallTouchpadButton(VRTK_DeviceFinder.Devices.LeftController, lHandEvents.GetTouchpadAxis());
            TouchPadDown(VRTK_DeviceFinder.Devices.LeftController);
        }

        private void RHandEvents_TouchpadReleased(object sender, ControllerInteractionEventArgs e)
        {
            TouchPadUp(VRTK_DeviceFinder.Devices.RightController);
        }

        private void LHandEvents_TouchpadReleased(object sender, ControllerInteractionEventArgs e)
        {
            TouchPadUp(VRTK_DeviceFinder.Devices.LeftController);
        }

        private void TouchPadDown(VRTK_DeviceFinder.Devices control)
        {
            switch (control)
            {
                case VRTK_DeviceFinder.Devices.LeftController:
                    leftTimer = Time.time + holdTouchPadTimer;
                    break;
                case VRTK_DeviceFinder.Devices.RightController:
                    rightTimer = Time.time + holdTouchPadTimer;
                    break;
            }
        }

        private void TouchPadUp(VRTK_DeviceFinder.Devices control)
        {
            switch (control)
            {
                //case VRTK_DeviceFinder.Devices.LeftController:
                //    // Checks if user held down button for long enough
                //    if (Time.time > leftTimer)
                //    {
                //        Debug.Log("ZH DIED");
                //        //GameManager.instance.CmdAddCalibrationPoint(gameObject.name, lHandCont.position);
                //        NetworkPlayerPosManager.localInstance.CmdAddCalibrationPointToGameManager(gameObject.name, lHandCont.position);
                //    }
                //    break;
                //case VRTK_DeviceFinder.Devices.RightController:
                //    // Checks if user held down button for long enough
                //    if (Time.time > rightTimer)
                //    {
                //        Debug.Log("ZH MUM DIED");
                //        //GameManager.instance.CmdAddCalibrationPoint(gameObject.name, rHandCont.position);
                //        NetworkPlayerPosManager.localInstance.CmdAddCalibrationPointToGameManager(gameObject.name, rHandCont.position);
                //    }
                //    break;
            }
        }
    }
}