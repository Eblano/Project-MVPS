using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyLocalOffset : MonoBehaviour
{
    [SerializeField] private Vector3 posROffset;
    [SerializeField] private Vector3 rotROffset;
    [SerializeField] private Vector3 posLOffset;
    [SerializeField] private Vector3 rotLOffset;

    public void ApplyRightOffset()
    {
        transform.parent.localPosition = posROffset;
        transform.parent.localRotation = Quaternion.Euler(rotROffset);
    }

    public void ApplyLeftOffset()
    {
        transform.parent.localPosition = posLOffset;
        transform.parent.localRotation = Quaternion.Euler(rotLOffset);
    }
}
