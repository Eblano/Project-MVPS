using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SealTeam4;

public class PlayerStats : MonoBehaviour, IDamageable
{
    [SerializeField] private int totalHp = 100;
    [SerializeField] private HitBoxColliders hitBoxColliders;

    public int headDmg = 100;
    public int bodyDmg = 20;
    public int handDmg = 15;
    public int legDmg = 15;

    private NetworkInstanceId netID;

    [System.Serializable]
    private class HitBoxColliders
    {
        public List<Collider> headColliders;
        public List<Collider> bodyColliders;
        public List<Collider> HandColliders;
        public List<Collider> legColliders;
    }

    private void Start()
    {
        if (GameManagerAssistant.instance.isServer)
        {
            Destroy(this);
        }

        netID = GetComponent<NetworkIdentity>().netId;
    }

    public void TakeDamage(int damage)
    {
        totalHp -= damage;
        UpdateOverlay();
        CheckHealth();
    }

    private void UpdateOverlay()
    {
        GameManagerAssistant.instance.TargetUpdatePanelTransparency(NetworkServer.objects[netID].connectionToClient, totalHp);
    }

    private void CheckHealth()
    {
        if (totalHp <= 0)
        {
            // Stop game when dead
            Time.timeScale = 0;
        }
    }

    public void OnHit(Collider c)
    {
        foreach (Collider bodyColl in hitBoxColliders.bodyColliders)
        {
            if (c == bodyColl)
            {
                TakeDamage(bodyDmg);
                return;
            }
        }

        foreach (Collider headColl in hitBoxColliders.headColliders)
        {
            if (c == headColl)
            {
                TakeDamage(headDmg);
                return;
            }
        }

        foreach (Collider handColl in hitBoxColliders.HandColliders)
        {
            if (c == handColl)
            {
                TakeDamage(handDmg);
                return;
            }
        }

        foreach (Collider legColl in hitBoxColliders.legColliders)
        {
            if (c == legColl)
            {
                TakeDamage(handDmg);
                return;
            }
        }
    }
}