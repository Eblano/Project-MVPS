using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using VRTK;

public class ServerSync : NetworkBehaviour
{
    [SerializeField] private Transform headset, lControl, rControl;
    [SerializeField] private GameObject leftHandObj;    // DEBUG
    [SerializeField] private GameObject rightHandObj;   // DEBUG
    private Vector3AndQuaternion head, lHand, rHand;
    private Vector3 headPos, lHandPos, rHandPos;
    private Quaternion headRot, lHandRot, rHandRot;

    [Command]
    public void CmdSyncVRTransform(Vector3AndQuaternion head, Vector3AndQuaternion lHand, Vector3AndQuaternion rHand)
    {
        RpcSyncVRTransform(head, lHand, rHand);
    }

    [Command]
    public void CmdCallGrab(VRTK_DeviceFinder.Devices control, float grabRadius)
    {
        RpcCallGrab(control, grabRadius);
    }

    [Command]
    public void CmdCallUngrab(VRTK_DeviceFinder.Devices control, Vector3 velo, Vector3 anguVelo)
    {
        RpcCallUngrab(control, velo, anguVelo);
    }

    [Command]
    public void CmdTriggerClick(VRTK_DeviceFinder.Devices control)
    {
        RpcCallTriggerClick(control);
    }

    [Command]
    public void CmdTriggerUnlick(VRTK_DeviceFinder.Devices control)
    {
        RpcCallTriggerUnclick(control);
    }

    [Command]
    private void CmdTransferObject(VRTK_DeviceFinder.Devices control, GameObject obj)
    {
        RpcTransferObject(control, obj);
    }

    [ClientRpc]
    public void RpcSyncVRTransform(Vector3AndQuaternion head, Vector3AndQuaternion lHand, Vector3AndQuaternion rHand)
    {
        headset.position = head.pos;
        headset.rotation = head.rot;

        lControl.position = lHand.pos;
        lControl.rotation = lHand.rot;

        rControl.position = rHand.pos;
        rControl.rotation = rHand.rot;
    }

    [ClientRpc]
    public void RpcCallGrab(VRTK_DeviceFinder.Devices control, float grabRadius)
    {
        // If the controller is grabbing something
        if (ControllerIsGrabbingSomething(control))
        {
            return;
        }

        Transform snapTarget = null;

        switch (control)
        {
            case VRTK_DeviceFinder.Devices.LeftController:
                snapTarget = lControl;
                break;
            case VRTK_DeviceFinder.Devices.RightController:
                snapTarget = rControl;
                break;
        }

        GameObject currGrabbedObj;

        // Set current grabbed object as nearest game object within radius
        currGrabbedObj = GetNearestGameObjectWithinGrabRadius(grabRadius, snapTarget.position);

        // If there is no grabbable object, stop running this method
        if (currGrabbedObj == null)
        {
            return;
        }

        if (AttemptingSameObjectGrab(control, currGrabbedObj))
        {
            CmdTransferObject(control, currGrabbedObj);
        }

        // Store grabbed object on the correct hand
        switch (control)
        {
            case VRTK_DeviceFinder.Devices.LeftController:
                leftHandObj = currGrabbedObj;
                break;
            case VRTK_DeviceFinder.Devices.RightController:
                rightHandObj = currGrabbedObj;
                break;
        }

        SnapObjectToController(currGrabbedObj, snapTarget);
    }

    [ClientRpc]
    public void RpcCallUngrab(VRTK_DeviceFinder.Devices control, Vector3 velo, Vector3 anguVelo)
    {
        // If the controller is not grabbing something
        if (!ControllerIsGrabbingSomething(control))
        {
            return;
        }

        GameObject currGrabbedObj = null;

        switch (control)
        {
            case VRTK_DeviceFinder.Devices.LeftController:
                currGrabbedObj = leftHandObj;
                leftHandObj = null;
                break;
            case VRTK_DeviceFinder.Devices.RightController:
                currGrabbedObj = rightHandObj;
                rightHandObj = null;
                break;
        }

        currGrabbedObj.transform.SetParent(null);
        ApplyControllerPhysics(currGrabbedObj.GetComponent<Rigidbody>(), velo, anguVelo);
    }

    [ClientRpc]
    public void RpcCallTriggerClick(VRTK_DeviceFinder.Devices control)
    {
        // If the controller is not grabbing something
        if (!ControllerIsGrabbingSomething(control))
        {
            return;
        }

        GameObject currGrabbedObj = null;

        switch (control)
        {
            case VRTK_DeviceFinder.Devices.LeftController:
                currGrabbedObj = leftHandObj;
                break;
            case VRTK_DeviceFinder.Devices.RightController:
                currGrabbedObj = rightHandObj;
                break;
        }

        // Return if object grabbed is not usable
        if (currGrabbedObj.GetComponent<NetworkUsableObject>() == null)
        {
            return;
        }

        NetworkUsableObject nuObj = currGrabbedObj.GetComponent<NetworkUsableObject>();
        nuObj.owner = this.gameObject;
        nuObj.use = true;
    }

    [ClientRpc]
    public void RpcCallTriggerUnclick(VRTK_DeviceFinder.Devices control)
    {
        // If the controller is not grabbing something
        if (!ControllerIsGrabbingSomething(control))
        {
            return;
        }

        GameObject currGrabbedObj = null;

        switch (control)
        {
            case VRTK_DeviceFinder.Devices.LeftController:
                currGrabbedObj = leftHandObj;
                break;
            case VRTK_DeviceFinder.Devices.RightController:
                currGrabbedObj = rightHandObj;
                break;
        }

        // Return if object grabbed is not usable
        if (currGrabbedObj.GetComponent<NetworkUsableObject>() == null)
        {
            return;
        }

        //currGrabbedObj.GetComponent<NetworkUsableObject>().use = false;
    }

    // METHOD TO UPDATE CLIENT SIDE FIRST

    [ClientRpc]
    private void RpcTransferObject(VRTK_DeviceFinder.Devices control, GameObject obj)
    {
        Transform snapTarget = null;

        switch (control)
        {
            case VRTK_DeviceFinder.Devices.LeftController:
                snapTarget = lControl;
                leftHandObj = obj;
                rightHandObj = null;
                break;
            case VRTK_DeviceFinder.Devices.RightController:
                snapTarget = rControl;
                rightHandObj = obj;
                leftHandObj = null;
                break;
        }

        SnapObjectToController(obj, snapTarget);
    }

    private bool AttemptingSameObjectGrab(VRTK_DeviceFinder.Devices control, GameObject attemptObj)
    {
        switch (control)
        {
            case VRTK_DeviceFinder.Devices.LeftController:
                if (rightHandObj == attemptObj)
                {
                    return true;
                }
                break;
            case VRTK_DeviceFinder.Devices.RightController:
                if (leftHandObj == attemptObj)
                {
                    return true;
                }
                break;
        }
        return false;
    }

    /// <summary>
    /// Returns true is control passed is grabbing something
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
    private bool ControllerIsGrabbingSomething(VRTK_DeviceFinder.Devices control)
    {
        switch (control)
        {
            case VRTK_DeviceFinder.Devices.LeftController:
                if (leftHandObj != null)
                {
                    return true;
                }
                break;
            case VRTK_DeviceFinder.Devices.RightController:
                if (rightHandObj != null)
                {
                    return true;
                }
                break;
        }
        return false;
    }

    /// <summary>
    /// Return the nerest grabbable game object
    /// </summary>
    /// <param name="grabRadius"></param>
    /// <returns></returns>
    private GameObject GetNearestGameObjectWithinGrabRadius(float grabRadius, Vector3 centerPos)
    {
        // Get all grabbable objects within grab radius
        Collider[] grabbablesWithinRadius = Physics.OverlapSphere(centerPos, grabRadius, ~LayerMask.NameToLayer("GrabLayer"), QueryTriggerInteraction.Collide);
        // If there is no grabbable within the radius, stop running this method
        if (grabbablesWithinRadius.Length == 0)
        {
            return null;
        }
        // Return first grabable object within radius
        return grabbablesWithinRadius[0].gameObject;
    }

    /// <summary>
    /// Applies current rotation and position of controller to object and set controller as parent
    /// </summary>
    /// <param name="objToSnap"></param>
    /// <param name="controllerTransform"></param>
    private void SnapObjectToController(GameObject objToSnap, Transform controllerTransform)
    {
        // Set controller as object's parent
        objToSnap.transform.SetParent(controllerTransform);
        // Zero out the local transformation
        objToSnap.transform.localPosition = Vector3.zero;
        objToSnap.transform.localRotation = Quaternion.identity;
        // Disable object's physics
        objToSnap.GetComponent<Rigidbody>().isKinematic = true;
    }

    /// <summary>
    /// Applies controller physics to given rigidbody
    /// </summary>
    /// <param name="rb"></param>
    private void ApplyControllerPhysics(Rigidbody rb, Vector3 velo, Vector3 anguVelo)
    {
        // Disable object's physics
        rb.isKinematic = false;
        // Transfer all controller velocities to object
        rb.angularVelocity = anguVelo;
        rb.velocity = velo;
    }
}

public class Vector3AndQuaternion
{
    public Vector3 pos;
    public Quaternion rot;

    public void SetPosAndRot(Transform t)
    {
        pos = t.position;
        rot = t.rotation;
    }
}

