using UnityEngine;

namespace SealTeam4
{
    interface IDamageable
    {
        void OnHit(Collider c, GlobalEnums.WeaponType weaponType);

        AIStats.NPCType GetNPCType();
    }
}

