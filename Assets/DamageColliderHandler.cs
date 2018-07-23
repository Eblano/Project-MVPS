using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageColliderHandler : IDamageable
{
    [SerializeField] private int damageTaken;
    [SerializeField] private PlayerStats playerStats;

    public void ColliderOnHit()
    {
        // Called by NPC for player to take dmg
        playerStats.TakeDamage(damageTaken);
        TriggerSomething();
    }

    public void OnHit(Collider c)
    {

    }

    private void TriggerSomething()
    {
        // I triggerable?
    }
}