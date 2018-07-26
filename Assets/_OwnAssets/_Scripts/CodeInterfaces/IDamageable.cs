using UnityEngine;

namespace SealTeam4
{
    interface IDamageable
    {
        void OnHit(Collider c);

        AIStats.NPCType GetNPCType();
    }
}

