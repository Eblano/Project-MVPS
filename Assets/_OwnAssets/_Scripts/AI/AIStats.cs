using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    [System.Serializable]
    public class AIStats
    {
        public enum NPCType { NONE, TERRORIST, VIP, CIVILLIAN };
        public enum CivillianStressResponseMode { FREEZE, RUNTOEXIT, RANDOM }

        #region User Settable Parameters
        [HideInInspector] public bool activateOnSpawn = true;
        [HideInInspector] public NPCType npcType = NPCType.CIVILLIAN;

        // For Civillian
        [HideInInspector] public CivillianStressResponseMode threatResponseMode = CivillianStressResponseMode.FREEZE;

        // For Terrorist
        public GlobalEnums.GunAccuracy gunAccuracy = GlobalEnums.GunAccuracy.HIGH;
        [HideInInspector] public float gunCD = 1f;
        [HideInInspector] public float maxGunRange = 10.0f;
        #endregion

        #region Hidden Parameters
        [HideInInspector] public float lookAngleMarginOfError = 1f; // Min angle of error when rotating to face target
        [HideInInspector] public float stopDist = 0.05f; // Stop distance to any target
        [HideInInspector] public float stopDist_Convo = 1.0f; // Stop distance for conversation

        [HideInInspector] public float turningSpeed = 30f;
        [HideInInspector] public float normalMoveSpeed = 1f;
        [HideInInspector] public float runningSpeed = 2f;

        [SerializeField] private int totalHp = 100;

        [HideInInspector] public int bulletHeadDmg = 100;
        [HideInInspector] public int bulletBodyDmg = 35;
        [HideInInspector] public int bulletHandDmg = 20;
        [HideInInspector] public int bulletLegDmg = 25;

        [HideInInspector] public int knifeHeadDmg = 100;
        [HideInInspector] public int knifeBodyDmg = 75;
        [HideInInspector] public int knifeHandDmg = 25;
        [HideInInspector] public int knifeLegDmg = 25;

        // For Terrorist
        [HideInInspector] public List<string> allDynamicWaypoints;
        [HideInInspector] public float shootTargetAngleOfError = 5f;
        [HideInInspector] public float losMarginSize = 0.5f;
        [HideInInspector] public float knifeSwingCD = 1f;
        [HideInInspector] public float meleeDist = 0.8f;
        [HideInInspector] public float initEngageDelay = 0f;

        // For VIP
        [HideInInspector] public float vipFollowPlayerDistance = 2f;
        [HideInInspector] public float vipGrabbedPlayerDistance = 0.3f;
        #endregion

        public int GetTotalHP()
        {
            return totalHp;
        }

        public void TakeDamage(int dmgAmt)
        {
            totalHp -= dmgAmt;
            if (totalHp < 0)
                totalHp = 0;
        }
    }
}