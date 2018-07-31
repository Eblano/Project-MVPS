using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalTransformSetter : MonoBehaviour
{
    [System.Serializable]
    private class TransformOffset
    {
        public Vector3 posOffset = Vector3.zero;
        public Vector3 rotOffset = Vector3.zero;
        public float scale = 1;
    }

    [SerializeField] private GameObject targetObject;
    [SerializeField] private TransformOffset transformOffset;

    private void Update()
    {
        if(targetObject)
        {
            targetObject.transform.localPosition = transformOffset.posOffset;
            targetObject.transform.localRotation = Quaternion.Euler(transformOffset.rotOffset);
            targetObject.transform.localScale = new Vector3(transformOffset.scale, transformOffset.scale, transformOffset.scale);
        }
    }
}
