using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    public class NPCKnife : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            IDamageable iDamagable = other.transform.root.GetComponent<IDamageable>();

            if (iDamagable != null)
                iDamagable.OnHit(other);
        }
    }
}

