using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyLocalOffset : MonoBehaviour
{
    [SerializeField] private Vector3 posOffset;
    [SerializeField] private Vector3 rotOffset;
    
    public void ApplyOffset()
    {
        transform.parent.localPosition = posOffset;
        transform.parent.localRotation = Quaternion.Euler(rotOffset);
    }
}
