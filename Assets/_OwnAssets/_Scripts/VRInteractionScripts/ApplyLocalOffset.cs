using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyLocalOffset : MonoBehaviour
{
    [SerializeField] private Vector3 posROffset;
    [SerializeField] private Vector3 rotROffset;
    [SerializeField] private Vector3 posLOffset;
    [SerializeField] private Vector3 rotLOffset;

    public Vector3 GetRightPosOffset()
    {
        return posROffset;
    }
    public Quaternion GetRightRotOffset()
    {
        return Quaternion.Euler(rotROffset);
    }
    public Vector3 GetLeftPosOffset()
    {
        return posLOffset;
    }
    public Quaternion GetLeftRotOffset()
    {
        return Quaternion.Euler(rotLOffset);
    }
}
