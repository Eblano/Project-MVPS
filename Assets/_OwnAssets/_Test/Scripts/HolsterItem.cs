using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HolsterItem : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if(other.transform.parent == null && other.gameObject.CompareTag("CanBeHolstered"))
        {
            Debug.Log("Holstered: " + other.name);
            other.transform.SetParent(transform);
            other.GetComponent<Rigidbody>().isKinematic = true;
            //other.GetComponent<SyncParent>().RpcSetParent(gameObject);
            other.transform.localPosition = Vector3.zero;
            other.transform.localRotation = Quaternion.identity;
        }
    }
}
