using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace SealTeam4
{
    public class SnappableObject : InteractableObject
    {
        [SerializeField] private float snapRange;
        //protected Transform snappedTo;
        private Rigidbody rb;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        [Command]
        public void CmdCheckSnappable()
        {
            RpcCheckSnappable();
        }

        [Command]
        public void CmdUnsnap()
        {
            RpcUnsnap();
        }

        [ClientRpc]
        public void RpcCheckSnappable()
        {
            // Get all snappable positions within snappable radius
            Collider[] snappabbablesWithinRadius = Physics.OverlapSphere(transform.position, snapRange, 1 << LayerMask.NameToLayer("SnapLayer"), QueryTriggerInteraction.Collide);
            Debug.Log(LayerMask.LayerToName(LayerMask.NameToLayer("SnapLayer")));
            // If there is no snappabbable within the radius, stop running this method
            if (snappabbablesWithinRadius.Length == 0)
            {
                return;
            }

            SnapObject(snappabbablesWithinRadius[0].transform);
        }

        [ClientRpc]
        public void RpcUnsnap()
        {
            Unsnap();
        }

        public virtual void Unsnap()
        {
            //snappedTo = null;
            transform.SetParent(null);
            rb.isKinematic = false;
        }

        public virtual void SnapObject(Transform snapTransform)
        {
            //snappedTo = snapTransform;
            transform.SetParent(snapTransform);
            rb.isKinematic = true;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }
}