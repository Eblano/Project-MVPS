using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RefreshPrefab
{
    [MenuItem("Tools/Refresh Selected Prefabs")]
    private static void RefreshSelectedPrefabs()
    {
        int counter = 0;
        Object[] selectedObjects = Selection.objects;
        List<GameObject> goToDestroy = new List<GameObject>();

        foreach (Object selectedObject in selectedObjects)
        {
            if (GameObject.Find(selectedObject.name) == true)
            {
                Debug.LogWarning("Existing " + selectedObject.name + " with same name is on scene, skipping");
                continue;
            }
            else
            {
                PrefabUtility.InstantiatePrefab(selectedObject);

                GameObject instantiatedGO = GameObject.Find(selectedObject.name);
                PrefabUtility.CreatePrefab(AssetDatabase.GetAssetPath(selectedObject), instantiatedGO);

                goToDestroy.Add(instantiatedGO);

                counter++;
            }
        }

        foreach (GameObject go in goToDestroy)
        {
            GameObject.DestroyImmediate(go);
        }

        Debug.Log("Successfully Refreshed " + counter + " prefabs, please manually expose them to editor again");
    }
}