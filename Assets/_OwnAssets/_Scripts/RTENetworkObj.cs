using Battlehub.RTSaveLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTENetworkObj : MonoBehaviour
{
    [SerializeField] private bool copyMeshFilter;
    [SerializeField] private bool copyMaterial;

    [SerializeField] private GameObject objToSpawn;

    private void Update()
    {
        if (Dependencies.ProjectManager == null)
        {
            if (objToSpawn)
            {
                GameObject go = Instantiate(objToSpawn, transform.position, transform.rotation);

                if (copyMeshFilter)
                {
                    go.GetComponent<MeshFilter>().mesh = GetComponent<MeshFilter>().mesh;
                }

                if (copyMaterial)
                {
                    go.GetComponent<MeshRenderer>().materials = GetComponent<MeshRenderer>().materials;
                }

                objToSpawn.transform.localScale = transform.localScale;

                Destroy(gameObject);
            }
            else
            {
                Debug.Log("No Object to Spwan");
                Destroy(gameObject);
            }
        }
    }
}
