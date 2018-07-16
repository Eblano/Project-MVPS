using System.Collections;
using System.Collections.Generic;
using SealTeam4;
using UnityEngine;
using UnityEngine.Networking;
using VRTK;

public class PlayerInteractionSync : NetworkBehaviour
{
    [SerializeField] private Transform headset, lControl, rControl;
    [SerializeField] private GameObject leftHandObj;    // DEBUG
    [SerializeField] private GameObject rightHandObj;   // DEBUG
    private Vector3AndQuaternion head, lHand, rHand;
    private Vector3 headPos, lHandPos, rHandPos;
    private Quaternion headRot, lHandRot, rHandRot;

    private GameManager gameManager;

    [SerializeField] private List<HolsterItem> holsterItems;

    public override void OnStartServer()
    {
        gameManager = GameManager.instance;
    }

    public Vector3 GetHeadPos()
    {
        return headPos;
    }

    public Vector3 GetLHandPos()
    {
        return lHandPos;
    }

    #region ServerMethods
    /// <summary>
    /// Updates the player's postition and rotation.
    /// </summary>
    /// <param name="head"></param>
    /// <param name="lHand"></param>
    /// <param name="rHand"></param>
    [Command]
    public void CmdSyncVRTransform(Vector3AndQuaternion head, Vector3AndQuaternion lHand, Vector3AndQuaternion rHand)
    {
        RpcSyncVRTransform(head, lHand, rHand);
    }

    /// <summary>
    /// Handles objects when player grabs.
    /// </summary>
    /// <param name="control"></param>
    /// <param name="grabRadius"></param>
    [Command]
    public void CmdCallGrab(VRTK_DeviceFinder.Devices control, float grabRadius)
    {
        RpcCallGrab(control, grabRadius);
    }

    /// <summary>
    /// Handles objects when player ungrabs.
    /// </summary>
    /// <param name="control"></param>
    /// <param name="velo"></param>
    /// <param name="anguVelo"></param>
    [Command]
    public void CmdCallUngrab(VRTK_DeviceFinder.Devices control, Vector3 velo, Vector3 anguVelo)
    {
        RpcCallUngrab(control, velo, anguVelo);
    }

    /// <summary>
    /// Handles object when trigger button is clicked.
    /// </summary>
    /// <param name="control"></param>
    [Command]
    public void CmdTriggerClick(VRTK_DeviceFinder.Devices control, NetworkInstanceId networkInstanceId)
    {
        RpcCallTriggerClick(control, networkInstanceId);
    }

    /// <summary>
    /// Handles object when trigger button is unclicked.
    /// </summary>
    /// <param name="control"></param>
    [Command]
    public void CmdTriggerUnlick(VRTK_DeviceFinder.Devices control)
    {
        RpcCallTriggerUnclick(control);
    }

    /// <summary>
    /// Handles object when touchpad button is pressed
    /// </summary>
    /// <param name="control"></param>
    /// <param name="touchpadAxis"></param>
    [Command]
    public void CmdCallTouchpadButton(VRTK_DeviceFinder.Devices control, Vector2 touchpadAxis)
    {
        RpcCallTouchpadButton(control, touchpadAxis);
    }

    /// <summary>
    /// Swap the ownership of this hand's grabbed object to the the other hand.
    /// </summary>
    /// <param name="control"></param>
    /// <param name="obj"></param>
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

        // If grabbing the same object
        if (IsSameObjectGrab(control, currGrabbedObj))
        {
            CmdTransferObject(control, currGrabbedObj);
            return;
        }

        if (IsSecondaryGrab(control, currGrabbedObj))
        {
            Transform objectParent = currGrabbedObj.transform.parent;
            ITwoHandedObject twoHandedObject;
            twoHandedObject = objectParent.GetComponent(typeof(ITwoHandedObject)) as ITwoHandedObject;
            if (objectParent.GetComponent<Gun>() && currGrabbedObj.CompareTag("SecondGrabPoint"))
            {
                twoHandedObject.SecondHandActive();
            }
            
            if (currGrabbedObj.GetComponent<SlideHandler>())
            {
                SlideHandler slide = currGrabbedObj.GetComponent<SlideHandler>();
                slide.OnGrabbed();
            }
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

        InteractableObject interactableObject = currGrabbedObj.GetComponent<InteractableObject>();
        interactableObject.SetOwner(gameObject);
        SnapObjectToController(currGrabbedObj, snapTarget, interactableObject.GetGrabPosition());
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

        Transform grabbedObjParent;
        grabbedObjParent = currGrabbedObj.GetComponent<InteractableObject>().GetParent();
        currGrabbedObj.GetComponent<InteractableObject>().SetOwner(null);
        if (grabbedObjParent)
        {
            ITwoHandedObject twoHandedObject;
            twoHandedObject = grabbedObjParent.GetComponent(typeof(ITwoHandedObject)) as ITwoHandedObject;

            if(twoHandedObject != null)
            {
                twoHandedObject.SecondHandInactive();
            }

            currGrabbedObj.transform.SetParent(grabbedObjParent);

            if (currGrabbedObj.GetComponent<SlideHandler>())
            {
                SlideHandler slide = currGrabbedObj.GetComponent<SlideHandler>();
                slide.OnUngrabbed();
                slide.ResetLocalPos();
            }
        }
        else
        {
            currGrabbedObj.transform.SetParent(null);
        }

        // Check if object is snappable
        if (currGrabbedObj.GetComponent<SnappableObject>())
        {
            // Check for nearby snappable spots
            if (currGrabbedObj.GetComponent<SnappableObject>().IsNearSnappables())
            {
                currGrabbedObj.GetComponent<SnappableObject>().CmdCheckSnappable();
            }
            else
            {
                ApplyControllerPhysics(currGrabbedObj.GetComponent<Rigidbody>(), velo, anguVelo);
            }
        }
        else
        {
            ApplyControllerPhysics(currGrabbedObj.GetComponent<Rigidbody>(), velo, anguVelo);
        }
    }

    [ClientRpc]
    public void RpcCallTriggerClick(VRTK_DeviceFinder.Devices control, NetworkInstanceId networkInstanceId)
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
        if (currGrabbedObj.GetComponent<UsableObject>() == null)
        {
            return;
        }

        UsableObject usableObject = currGrabbedObj.GetComponent<UsableObject>();
        usableObject.Use(networkInstanceId);
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
        if (currGrabbedObj.GetComponent<UsableObject>() == null)
        {
            return;
        }

        //currGrabbedObj.GetComponent<NetworkUsableObject>().use = false;
    }

    [ClientRpc]
    public void RpcCallTouchpadButton(VRTK_DeviceFinder.Devices control, Vector2 touchpadAxis)
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
        if (currGrabbedObj.GetComponent<UsableObject>() == null)
        {
            return;
        }

        UsableObject usableObject = currGrabbedObj.GetComponent<UsableObject>();

        if (touchpadAxis.y > 0)
        {
            usableObject.UseUp();
        }
        else if (touchpadAxis.y < 0)
        {
            usableObject.UseDown();
        }
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

        SnapObjectToController(obj, snapTarget, obj.GetComponent<InteractableObject>().GetGrabPosition());
    }
    #endregion ServerMethods

    #region HelperMethods
    /// <summary>
    /// Returns true when attempting to grab same object as the other hand.
    /// </summary>
    /// <param name="control"></param>
    /// <param name="attemptObj"></param>
    /// <returns></returns>
    private bool IsSameObjectGrab(VRTK_DeviceFinder.Devices control, GameObject attemptObj)
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
    /// Returns true when attempting to grab an interactable object that is the child of the other hand's object.
    /// </summary>
    /// <param name="control"></param>
    /// <param name="attemptObj"></param>
    /// <returns></returns>
    private bool IsSecondaryGrab(VRTK_DeviceFinder.Devices control, GameObject attemptObj)
    {
        if (attemptObj.transform.parent != null)
        {
            switch (control)
            {
                case VRTK_DeviceFinder.Devices.LeftController:
                    if (rightHandObj == attemptObj.transform.parent.gameObject)
                    {
                        return true;
                    }
                    break;
                case VRTK_DeviceFinder.Devices.RightController:
                    if (leftHandObj == attemptObj.transform.parent.gameObject)
                    {
                        return true;
                    }
                    break;
            }
        }
        return false;
    }

    /// <summary>
    /// Returns true when hand is already grabbing something.
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
    /// Returns the nearest grabbable game object within specified radius.
    /// </summary>
    /// <param name="grabRadius"></param>
    /// <returns></returns>
    private GameObject GetNearestGameObjectWithinGrabRadius(float grabRadius, Vector3 centerPos)
    {
        // Get all grabbable objects within grab radius
        Collider[] grabbablesWithinRadius = Physics.OverlapSphere(centerPos, grabRadius, 1 << LayerMask.NameToLayer("GrabLayer"), QueryTriggerInteraction.Collide);
        // If there is no grabbable within the radius, stop running this method
        if (grabbablesWithinRadius.Length == 0)
        {
            return null;
        }

        float nearestDist = float.MaxValue;
        float distance;
        Collider nearestColl = null;
        // Return nearest grabbable object within radius
        foreach (Collider c in grabbablesWithinRadius)
        {
            distance = Vector3.SqrMagnitude(centerPos - c.transform.position);
            if (distance < nearestDist)
            {
                nearestDist = distance;
                nearestColl = c;
            }
        }
        // Return nearest grabable object within radius
        return nearestColl.gameObject;
    }

    /// <summary>
    /// Applies current rotation and position of controller to object and set controller as parent.
    /// </summary>
    /// <param name="objToSnap"></param>
    /// <param name="controllerTransform"></param>
    private void SnapObjectToController(GameObject objToSnap, Transform controllerTransform, Transform grabTransform)
    {
        // Set controller as object's parent
        objToSnap.transform.SetParent(controllerTransform);
        // Zero out the local transformation
        objToSnap.transform.localPosition = -grabTransform.localPosition; // multiply 1/parent scale
        //objToSnap.transform.localPosition = grabTransform.localPosition;
        objToSnap.transform.localRotation = Quaternion.identity;
        if (!objToSnap.GetComponent<Rigidbody>())
        {
            return;
        }
        // Disable object's physics
        objToSnap.GetComponent<Rigidbody>().isKinematic = true;
    }

    /// <summary>
    /// Applies controller physics to given rigidbody.
    /// </summary>
    /// <param name="rb"></param>
    private void ApplyControllerPhysics(Rigidbody rb, Vector3 velo, Vector3 anguVelo)
    {
        if (!rb)
        {
            return;
        }

        // Disable object's physics
        rb.isKinematic = false;
        // Transfer all controller velocities to object
        rb.angularVelocity = anguVelo;
        rb.velocity = velo;
    }
    #endregion HelperMethods
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