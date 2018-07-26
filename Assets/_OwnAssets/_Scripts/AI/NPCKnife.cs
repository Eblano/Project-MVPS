using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    public class NPCKnife : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if(other.transform.root.name != transform.name)
            {
                IDamageable iDamagable = other.transform.root.GetComponent<IDamageable>();

                if (iDamagable != null && iDamagable.GetNPCType() != AIStats.NPCType.TERRORIST)
                    iDamagable.OnHit(other);
            }
        }
    }
}

