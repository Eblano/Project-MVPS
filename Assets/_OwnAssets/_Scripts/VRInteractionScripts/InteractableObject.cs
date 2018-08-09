using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace SealTeam4
{
    public class InteractableObject : NetworkBehaviour
    {
        [SerializeField] private Transform grabPosition;
        [SerializeField] private Transform parentTransform;
        protected GameObject owner;

        public void SetOwner(GameObject ownerToSet)
        {
            owner = ownerToSet;
        }

        public Transform GetParent()
        {
            return parentTransform;
        }

        public GameObject GetOwner()
        {
            return owner;
        }

        public Transform GetGrabPosition()
        {
            return grabPosition;
        }

        public void SetGrabPosition(Transform grabPosition)
        {
            this.grabPosition = grabPosition;
        }
    }
}