using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SealTeam4;

public class HolsterPosition : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.CompareTag("CanBeHolstered"));
        if (other.transform.parent == null && other.gameObject.CompareTag("CanBeHolstered"))
        {
            other.transform.SetParent(this.transform);
            other.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            other.transform.localPosition = Vector3.zero;
            other.transform.localRotation = Quaternion.identity;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log(other.gameObject.CompareTag("CanBeHolstered"));
        if (other.transform.parent == null && other.gameObject.CompareTag("CanBeHolstered"))
        {
            other.transform.SetParent(this.transform);
            other.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            other.transform.localPosition = Vector3.zero;
            other.transform.localRotation = Quaternion.identity;
        }
    }
}
