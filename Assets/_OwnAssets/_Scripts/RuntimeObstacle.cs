using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Battlehub.RTSaveLoad;
using ProtoBuf;

namespace SealTeam4
{
    /// <summary>
    /// Script to add navmesh obstacle on object after exiting runtime editor
    /// </summary>
    public class RuntimeObstacle : MonoBehaviour
    {
        private void Update()
        {
            if (!GameManager.instance.IsInLevelEditMode())
            {
                AddNavMeshObstacles(GetComponents<BoxCollider>()[0]);
                Destroy(GetComponents<BoxCollider>()[0]);
                Destroy(this);
            }
        }

        private void AddNavMeshObstacles(BoxCollider collider)
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
