using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class DelLete : MonoBehaviour
{
    public static VRTK_ControllerReference lHandRef;
    public static VRTK_ControllerReference rHandRef;

    private void Start()
    {
        StartCoroutine(SetController());
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
}
