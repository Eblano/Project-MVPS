using System.Collections;
using System.Collections.Generic;
using SealTeam4;
using UnityEngine;
using UnityEngine.Networking;
using VRTK;

public class PlayerInteractionSync : NetworkBehaviour, IActions
{
    [SerializeField] private Transform headset, lControl, rControl;
    [SerializeField] private Transform realLHandTrans, realRHandTrans;
    [SerializeField] private GameObject leftHandObj;    // DEBUG
    [SerializeField] private GameObject rightHandObj;   // DEBUG
    private Vector3AndQuaternion head, lHand, rHand;
    private Vector3 headPos, lHandPos, rHandPos;
    private Quaternion headRot, lHandRot, rHandRot;

    [Space(10)]
    [SerializeField] private Transform highestPoint;
    [SerializeField] private Collider headCollider;

    private NetworkInstanceId playerID;

    private GameManager gameManager;

    private BipedGrabNode lGrabNode;
    private BipedGrabNode rGrabNode;

    [SerializeField] private Animator anim;

    public override void OnStartServer()
    {
        gameManager = GameManager.instance;
    }

    public override void OnStartLocalPlayer()
    {
        playerID = GetComponent<NetworkIdentity>().netId;
    }

    public Transform GetHeadPos()
    {
        return headset;
    }

    public Transform GetLControllerPos()
    {
        return lControl;
    }

    public void UpdateLocal(Vector3AndQuaternion headTrans, Vector3AndQuaternion lHandTrans, Vector3AndQuaternion rHandTrans)
    {
        LerpTransform(headset, headTrans);
        LerpTransform(lControl, lHandTrans);
        LerpTransform(rControl, rHandTrans);
    }

    public void LerpTransform(Transform t, Vector3AndQuaternion targetTransform)
    {
        t.position = Vector3.Lerp(t.position, targetTransform.pos, 0.5f);
        t.rotation = Quaternion.Lerp(t.rotation, targetTransform.rot, 0.5f);
    }

    [Command]
    public void CmdSyncVRTransform(Vector3AndQuaternion head, Vector3AndQuaternion lHand, Vector3AndQuaternion rHand)
    {
        RpcSyncVRTransform(head, lHand, rHand);
    }

    [ClientRpc]
    public void RpcSyncVRTransform(Vector3AndQuaternion head, Vector3AndQuaternion lHand, Vector3AndQuaternion rHand)
    {
        // Make a method for this
        LerpTransform(headset, head);
        LerpTransform(lControl, lHand);
        LerpTransform(rControl, rHand);
    }

    private void TransferObject(VRTK_DeviceFinder.Devices control, GameObject obj)
    {
        Transform snapTarget = null;

        switch (control)
        {
            case VRTK_DeviceFinder.Devices.LeftController:
                snapTarget = realLHandTrans;
                leftHandObj = obj;
                rightHandObj = null;
                break;
            case VRTK_DeviceFinder.Devices.RightController:
                snapTarget = realRHandTrans;
                rightHandObj = obj;
                leftHandObj = null;
                break;
        }

        SnapObjectToController(obj, snapTarget, obj.GetComponent<InteractableObject>().GetGrabPosition());
    }

    public void AnimateHand(bool isLeft, bool isGrab)
    {
        if (isLeft)
        {
            anim.SetBool("LeftHandGrab", isGrab);
        }
        else
        {
            anim.SetBool("RightHandGrab", isGrab);
        }
    }

    public void Grab(VRTK_DeviceFinder.Devices control, float grabRadius)
    {
        // If the controller is grabbing something
        if (ControllerIsGrabbingSomething(control))
        {
            return;
        }

        Transform snapTarget = null;

        bool isLeftGrab = false;

        switch (control)
        {
            case VRTK_DeviceFinder.Devices.LeftController:
                snapTarget = realLHandTrans;
                isLeftGrab = true;
                if (anim)
                {
                    GameManagerAssistant.instance.RelaySenderCmdSyncGrabAnim(true, true);
                    anim.SetBool("LeftHandGrab", true);
                }
                break;
            case VRTK_DeviceFinder.Devices.RightController:
                snapTarget = realRHandTrans;
                isLeftGrab = false;
                if (anim)
                {
                    GameManagerAssistant.instance.RelaySenderCmdSyncGrabAnim(false, true);
                    anim.SetBool("RightHandGrab", true);
                }
                break;
        }

        GameObject currGrabbedObj;

        // Set current grabbed object as nearest game object within radius
        currGrabbedObj = GetNearestGameObjectWithinGrabRadius(grabRadius, snapTarget.position);

        if (currGrabbedObj)
        {
            if (isLeftGrab)
            {
                lGrabNode = currGrabbedObj.GetComponent<BipedGrabNode>();

                if (lGrabNode != null)
                {
                    lGrabNode.OnGrabbed(playerID, isLeftGrab);
                    return;
                }
            }
            else
            {
                rGrabNode = currGrabbedObj.GetComponent<BipedGrabNode>();

                if (rGrabNode != null)
                {
                    rGrabNode.OnGrabbed(playerID, isLeftGrab);
                    return;
                }
            }
        }

        GrabCalculate(currGrabbedObj, control);

        if (!currGrabbedObj)
        {
            return;
        }

        if (!currGrabbedObj.GetComponent<NetworkIdentity>())
            return;

        GameManagerAssistant.instance.RelaySenderCmdSnapToController(currGrabbedObj.GetComponent<NetworkIdentity>().netId, isLeftGrab);
    }

    public void Ungrab(VRTK_DeviceFinder.Devices control, Vector3 velo, Vector3 anguVelo)
    {
        bool isLeftGrab = false;

        switch (control)
        {
            case VRTK_DeviceFinder.Devices.LeftController:
                isLeftGrab = true;
                if (anim)
                {
                    GameManagerAssistant.instance.RelaySenderCmdSyncGrabAnim(true, false);
                    anim.SetBool("LeftHandGrab", false);
                }
                break;
            case VRTK_DeviceFinder.Devices.RightController:
                isLeftGrab = false;
                if (anim)
                {
                    GameManagerAssistant.instance.RelaySenderCmdSyncGrabAnim(false, false);
                    anim.SetBool("RightHandGrab", false);
                }
                break;
        }

        if (isLeftGrab)
        {
            if (lGrabNode)
            {
                lGrabNode.OnUngrabbed();
                lGrabNode = null;
            }
        }
        else
        {
            if (rGrabNode)
            {
                rGrabNode.OnUngrabbed();
                rGrabNode = null;
            }
        }

        UnGrabCalculate(control, velo, anguVelo);
        GameManagerAssistant.instance.RelaySenderCmdUnSnapFromController(isLeftGrab, velo, anguVelo);
    }

    public void TriggerClick(VRTK_DeviceFinder.Devices control, NetworkInstanceId networkInstanceId)
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

    public void TouchpadButton(VRTK_DeviceFinder.Devices control, Vector2 touchpadAxis)
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
            Debug.Log("No objects near grab radius");
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
        objToSnap.transform.localRotation = grabTransform.localRotation;
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

    public Transform GetControllerTransform(bool isLeft)
    {
        if (isLeft)
        {
            return realLHandTrans;
        }
        else
        {
            return realRHandTrans;
        }
    }

    public void GrabCalculate(GameObject currGrabbedObj, VRTK_DeviceFinder.Devices control)
    {
        // If there is no grabbable object, stop running this method
        if (currGrabbedObj == null)
        {
            Debug.Log("No grabbable");
            return;
        }

        if (currGrabbedObj.GetComponent<DoorKnob>())
        {
            currGrabbedObj.GetComponent<DoorKnob>().ActivateDoor();
            return;
        }

        // If grabbing the same object
        if (IsSameObjectGrab(control, currGrabbedObj))
        {
            Debug.Log("SameObjectGrab");
            TransferObject(control, currGrabbedObj);
            return;
        }

        if (IsSecondaryGrab(control, currGrabbedObj))
        {
            Debug.Log("SecondaryGrab");
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
                slide.OnGrabbed(playerID);
            }
        }

        Transform snapTarget = null;

        // Store grabbed object on the correct hand
        switch (control)
        {
            case VRTK_DeviceFinder.Devices.LeftController:
                leftHandObj = currGrabbedObj;
                snapTarget = realLHandTrans;
                break;
            case VRTK_DeviceFinder.Devices.RightController:
                rightHandObj = currGrabbedObj;
                snapTarget = realRHandTrans;
                break;
        }

        InteractableObject interactableObject = currGrabbedObj.GetComponent<InteractableObject>();
        interactableObject.SetOwner(gameObject);
        SnapObjectToController(currGrabbedObj, snapTarget, interactableObject.GetGrabPosition());
    }

    public void UnGrabCalculate(VRTK_DeviceFinder.Devices control, Vector3 velo, Vector3 anguVelo)
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

            if (twoHandedObject != null)
            {
                twoHandedObject.SecondHandInactive();
            }

            currGrabbedObj.transform.SetParent(grabbedObjParent);

            if (currGrabbedObj.GetComponent<SlideHandler>())
            {
                SlideHandler slide = currGrabbedObj.GetComponent<SlideHandler>();
                slide.OnUngrabbed(playerID);
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
                currGrabbedObj.GetComponent<SnappableObject>().CheckSnappable();
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

    public void SyncControllerSnap(bool isLeftControl, GameObject childObj)
    {
        if (isLeftControl)
        {
            GrabCalculate(childObj, VRTK_DeviceFinder.Devices.LeftController);
        }
        else
        {
            GrabCalculate(childObj, VRTK_DeviceFinder.Devices.RightController);
        }
    }

    public void SyncControllerUnSnap(bool isLeftControl, Vector3 velo, Vector3 anguVelo)
    {
        if (isLeftControl)
        {
            UnGrabCalculate(VRTK_DeviceFinder.Devices.LeftController, velo, anguVelo);
        }
        else
        {
            UnGrabCalculate(VRTK_DeviceFinder.Devices.RightController, velo, anguVelo);
        }
    }

    public List<string> GetActions()
    {
        return new List<string>();
    }

    public void SetAction(string action)
    {

    }

    public string GetName()
    {
        return gameObject.name;
    }

    public Vector3 GetHighestPointPos()
    {
        return highestPoint.position;
    }

    public Transform GetHighestPointTransform()
    {
        return highestPoint;
    }

    public Collider GetCollider()
    {
        return headCollider;
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