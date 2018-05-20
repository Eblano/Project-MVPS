using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HolsterItem : MonoBehaviour
{
    [SerializeField] private Transform holsterTransform;
    [SerializeField] private GameObject holsterItem;
    private GameObject holsterItemGO;

    private bool doneOnce = false;

    public void SpawnItem()
    {
        if (holsterItem)
        {
            holsterItemGO = Instantiate(holsterItem);
            holsterItemGO.transform.SetParent(transform);
            holsterItemGO.transform.localPosition = Vector3.zero;
            holsterItemGO.transform.localRotation = Quaternion.identity;
            NetworkServer.Spawn(holsterItemGO);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.transform.parent == null && other.gameObject.CompareTag("CanBeHolstered"))
        {
            other.transform.SetParent(transform);
            other.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            other.transform.localPosition = Vector3.zero;
            other.transform.localRotation = Quaternion.identity;
        }
    }
}
