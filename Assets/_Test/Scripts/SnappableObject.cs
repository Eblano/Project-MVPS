using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace SealTeam4
{
    public class SnappableObject : InteractableObject
    {
        [SerializeField] private float snapRange;
        private Rigidbody rb;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        #region ServerMethods
        /// <summary>
        /// Checks for snappable objects within radius.
        /// </summary>
        //[Command]
        public void CmdCheckSnappable()
        {
            RpcCheckSnappable();
        }

        /// <summary>
        /// Handles when the object is unsnapped.
        /// </summary>
        //[Command]
        public void CmdUnsnap()
        {
            RpcUnsnap();
        }

        //[ClientRpc]
        public void RpcCheckSnappable()
        {
            // Get all snappable positions within snappable radius
            Collider[] snappabbablesWithinRadius = Physics.OverlapSphere(transform.position, snapRange, 1 << LayerMask.NameToLayer("SnapLayer"), QueryTriggerInteraction.Collide);
            // If there is no snappabbable within the radius, stop running this method
            if (snappabbablesWithinRadius.Length == 0)
            {
                return;
            }

            // If snap position is already taken up
            if (snappabbablesWithinRadius[0].transform.childCount > 0)
            {
                return;
            }

            float nearestDist = float.MaxValue;
            float distance;
            Collider nearestColl = null;
            // Return nearest grabbable object within radius
            foreach (Collider c in snappabbablesWithinRadius)
            {
                distance = Vector3.SqrMagnitude(transform.position - c.transform.position);
                if (distance < nearestDist)
                {
                    nearestDist = distance;
                    nearestColl = c;
                }
            }

            SnapObject(nearestColl.transform);
        }

        //[ClientRpc]
        public void RpcUnsnap()
        {
            Unsnap();
        }
        #endregion ServerMethods

        #region HelperMethods
        /// <summary>
        /// Unset the snappable object and enable physics.
        /// </summary>
        public virtual void Unsnap()
        {
            transform.SetParent(null);
            rb.isKinematic = false;
        }
        
        /// <summary>
        /// Check for snappables within radius.
        /// </summary>
        /// <returns></returns>
        public bool IsNearSnappables()
        {
            // Get all snappable positions within snappable radius
            Collider[] snappabbablesWithinRadius = Physics.OverlapSphere(transform.position, snapRange, 1 << LayerMask.NameToLayer("SnapLayer"), QueryTriggerInteraction.Collide);
            // If there is no snappabbable within the radius, stop running this method
            if (snappabbablesWithinRadius.Length == 0)
            {
                return false;
            }

            // If snap position is already taken up
            if (snappabbablesWithinRadius[0].transform.childCount > 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Set the snappable object, disable physics and reset local transform.
        /// </summary>
        /// <param name="snapTransform"></param>
        public virtual void SnapObject(Transform snapTransform)
        {
            transform.SetParent(snapTransform);
            rb.isKinematic = true;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        #endregion HelperMethods
    }
}