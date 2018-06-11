using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Battlehub.RTSaveLoad;

namespace SealTeam4
{
    /// <summary>
    /// Script to add navmesh obstacle on object after exiting runtime editor
    /// </summary>
    public class RTEObstacle : MonoBehaviour
    {
        [Header("Colliders to use as navmesh obstacle")]
        [SerializeField] private List<BoxCollider> boxCollidersToCopy;

        [SerializeField] private bool forceAddNavMeshObstacles = false;

        [Header("Scripts to attach")]
        [SerializeField] private List<ScriptableObject> scriptsToAttach;

        private void Update()
        {
            if(Dependencies.ProjectManager == null || forceAddNavMeshObstacles)
            {
                AddNavMeshObstacles();
                Destroy(this);
            }
        }

        private void AddNavMeshObstacles()
        {
            foreach(BoxCollider collider in boxCollidersToCopy)
            {
                GameObject go = new GameObject();
                go.transform.position = transform.position;
                go.transform.rotation = transform.rotation;
                go.transform.SetParent(gameObject.transform);
                go.transform.localScale = new Vector3(1, 1, 1);
                NavMeshObstacle obstacle = go.AddComponent<NavMeshObstacle>();

                obstacle.shape = NavMeshObstacleShape.Box;
                obstacle.center = collider.center;
                obstacle.size = collider.size;
                obstacle.carving = true;
            }
        }
    }
}
