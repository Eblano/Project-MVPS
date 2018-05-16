using Battlehub.RTSaveLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Battlehub;
using Battlehub.RTGizmos;
using System;

namespace SealTeam4
{
    /// <summary>
    /// Script used to configure an object in Runtime Editor and 
    /// spawns the copy of the configured object after runtime editor exits
    /// </summary>
    public class RTEObject : MonoBehaviour
    {
        // Decides what component to copy
        [SerializeIgnore] [SerializeField] private bool copyMeshFilter = true;
        [SerializeIgnore] [SerializeField] private bool copyMaterial = true;
        [SerializeIgnore] [SerializeField] private bool copyBoxColToNavMeshCollider = true;
        [SerializeIgnore] [SerializeField] private bool copyBoxCol = true;

        // Delete extra mesh collider after spawned in RTE
        [SerializeIgnore] [SerializeField] private bool deleteMeshColliderAfterRTESpawn = true;

        // Toogle visibility of Box Collider Editor
        [SerializeField] [HideInInspector] private bool editCollider = false;
        private bool editCollider_LastState = false;
        // Box Collider Editor Color
        [SerializeField] private Color color = Color.green;

        [SerializeField] private GameObject objToSpawn;

        private void Start()
        {
            // Destroy unessesary Mesh Collider that was created
            // after object is spawned in runtime editor
            if (deleteMeshColliderAfterRTESpawn)
            {
                Destroy(GetComponent<MeshCollider>());
            }

            if (!objToSpawn)
            {
                Debug.Log("No Object to Spwan");
                Destroy(gameObject);
            }
        }

        protected void Update()
        {
            // When runtime editor can't be found
            if (Dependencies.ProjectManager == null)
            {
                if (objToSpawn)
                {
                    SpawnAndCopyComponents();
                    Destroy(gameObject);
                }
            }
            else
            {
                HandleColliderEditing();
            }
        }

        /// <summary>
        /// Handling visibility of box collider by adding/removing the gizmos
        /// </summary>
        private void HandleColliderEditing()
        {
            if (editCollider != editCollider_LastState)
            {
                if (editCollider)
                {
                    gameObject.AddComponent<BoxColliderGizmo>();
                    BoxColliderGizmo boxColliderGizmo = GetComponent<BoxColliderGizmo>();
                    boxColliderGizmo.LineColor = color;
                    boxColliderGizmo.HandlesColor = color;
                }
                else
                {
                    Destroy(gameObject.GetComponent<BoxColliderGizmo>());
                }
                editCollider_LastState = editCollider;
            }
        }

        /// <summary>
        /// Spawn and adds the nessesary components to the duplicated GameObject
        /// </summary>
        private void SpawnAndCopyComponents()
        {
            GameObject spawnedObj = Instantiate(objToSpawn, transform.position, transform.rotation);

            if (copyMeshFilter)
            {
                spawnedObj.GetComponent<MeshFilter>().mesh = GetComponent<MeshFilter>().mesh;
            }

            if (copyMaterial)
            {
                spawnedObj.GetComponent<MeshRenderer>().materials = GetComponent<MeshRenderer>().materials;
            }

            if (copyBoxColToNavMeshCollider)
            {
                spawnedObj.GetComponent<NavMeshObstacle>().size = GetComponent<BoxCollider>().size;
                spawnedObj.GetComponent<NavMeshObstacle>().center = GetComponent<BoxCollider>().center;
            }

            if (copyBoxCol)
            {
                spawnedObj.GetComponent<BoxCollider>().center = GetComponent<BoxCollider>().center;
                spawnedObj.GetComponent<BoxCollider>().size = GetComponent<BoxCollider>().size;
            }

            objToSpawn.transform.localScale = transform.lossyScale;
        }
    }
}
