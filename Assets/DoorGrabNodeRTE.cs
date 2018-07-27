using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    public class DoorGrabNodeRTE : MonoBehaviour
    {
        [SerializeField] private Transform grabPosition;

        [SerializeField] private Door[] doors;

        private void Update()
        {
            if(!GameManager.instance.IsInLevelEditMode())
            {
                gameObject.AddComponent<UnityEngine.Networking.NetworkIdentity>();
                DoorGrabNode doorGrabNode = gameObject.AddComponent<DoorGrabNode>();
                doorGrabNode.SetGrabPosition(grabPosition);
                doorGrabNode.SetDoors(doors);
                Destroy(this);
            }
        }
    }
}
