using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageColliderHandler : MonoBehaviour
{
    [SerializeField] private int damageTaken;
    [SerializeField] private PlayerStats playerStats;

    public void ColliderOnHit()
    {
        // Called by NPC for player to take dmg
        playerStats.TakeDamage(damageTaken);
        TriggerSomething();
    }

    private void TriggerSomething()
    {
        // I triggerable?
    }
}