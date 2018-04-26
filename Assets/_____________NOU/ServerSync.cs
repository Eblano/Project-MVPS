using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using VRTK;

public class ServerSync : NetworkBehaviour
{
    private GameObject currGrabbedObj; // add left AND right support: rn can only do either one
    [SerializeField] private Transform headset, lControl, rControl;
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
        Transform snapTarget = null;
        
        switch (control)
        {
            case VRTK_DeviceFinder.Devices.LeftController:
                snapTarget = lControl;
                break;
            case VRTK_DeviceFinder.Devices.RightController:
                snapTarget = rControl;
                break;
            default:
                break;
        }
        // Set current grabbed object as nearest game object within radius
        currGrabbedObj = GetNearestGameObjectWithinGrabRadius(grabRadius, snapTarget.position);

        // If there is no grabbable object, stop running this method
        if (currGrabbedObj == null)
        {
            return;
        }

        SnapObjectToController(currGrabbedObj, snapTarget);
    }

    [ClientRpc]
    public void RpcCallUngrab(VRTK_DeviceFinder.Devices control, Vector3 velo, Vector3 anguVelo)
    {
        // If there is no grabbable object, stop running this method
        if (currGrabbedObj == null)
        {
            return;
        }

        currGrabbedObj.transform.SetParent(null);
        ApplyControllerPhysics(currGrabbedObj.GetComponent<Rigidbody>(), velo, anguVelo);
        currGrabbedObj = null;
    }
    
    /// <summary>
    /// Return the nerest grabbable game object
    /// </summary>
    /// <param name="grabRadius"></param>
    /// <returns></returns>
    private GameObject GetNearestGameObjectWithinGrabRadius(float grabRadius, Vector3 centerPos)
    {
        // Get all grabbable objects within grab radius
        Collider[] grabbablesWithinRadius = Physics.OverlapSphere(centerPos, grabRadius, ~LayerMask.NameToLayer("GrabLayer"));
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

