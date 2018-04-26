using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battlehub.RTSaveLoad;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class RTNetworkObject : MonoBehaviour
{
    [SerializeField] private GameObject netObjPrefab;

    [SerializeField] private bool copyMeshFilter;
    [SerializeField] private bool copyMeshRendererMaterials;

    private void Update()
    {
        if(Dependencies.ProjectManager != null)
        {
            return;
        }

        if (!netObjPrefab)
        {
            Debug.LogError("No Object Prefab Found");
            Destroy(gameObject);
        }

        GameObject go = Instantiate(netObjPrefab, transform.position, transform.rotation);

        CopyGOScale(go);

        if (copyMeshFilter)
        {
            CopyMeshFilterBehaviour(go);
        }

        if (copyMeshRendererMaterials)
        {
            CopyMeshRendererBehaviour(go);
        }

        Destroy(this.gameObject);
    }

    private void CopyGOScale(GameObject go)
    {
        go.transform.localScale = transform.localScale;
    }

    private void CopyMeshFilterBehaviour(GameObject go)
    {
        if(!go.GetComponent<MeshFilter>())
        {
            go.AddComponent<MeshFilter>();
        }

        go.GetComponent<MeshFilter>().mesh = GetComponent<MeshFilter>().mesh;
    }

    private void CopyMeshRendererBehaviour(GameObject go)
    {
        if (!go.GetComponent<MeshRenderer>())
        {
            go.AddComponent<MeshRenderer>();
        }

        MeshRenderer thisGOMeshRenderer = GetComponent<MeshRenderer>();
        MeshRenderer newGoMeshRenderer = go.GetComponent<MeshRenderer>();

        newGoMeshRenderer.materials = thisGOMeshRenderer.materials;
    }
}
