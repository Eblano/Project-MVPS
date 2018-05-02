using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkUsableObject : MonoBehaviour
{
    public GameObject owner = null;
    public bool use = false;
    IUsableObject obj;

    private void Start()
    {
        obj = GetComponent(typeof(IUsableObject)) as IUsableObject;
    }

    private void Update()
    {
        // Only update if object parent is the owner
        if (gameObject.transform.root.gameObject != owner)
        {
            return;
        }

        if (use)
        {
            use = false;
            obj.UseObject();
        }
    }
}
