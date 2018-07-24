using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    [System.Serializable]
    public class AIStats
    {
        public enum NPCType { TERRORIST, VIP, CIVILLIAN };
        public enum CivillianStressResponseMode { FREEZE, RUNTOEXIT, RANDOM }

        #region User Settable Parameters
        [HideInInspector] public bool activateOnSpawn = true;
        [HideInInspector] public NPCType npcType = NPCType.CIVILLIAN;

        // For Civillian
        [HideInInspector] public CivillianStressResponseMode threatResponseMode = CivillianStressResponseMode.FREEZE;

        // For Terrorist
        [HideInInspector] public float maxGunRange = 5.0f;
        [HideInInspector] public float meleeDist = 0.3f;
        #endregion

        #region Hidden Parameters
        public float lookAngleMarginOfError = 10f; // Min angle of error when rotating to face target
        [HideInInspector] public float stopDist = 0.2f; // Stop distance to any target
        [HideInInspector] public float stopDist_Convo = 1.0f; // Stop distance for conversation

        [HideInInspector] public float turningSpeed = 2f;
        [HideInInspector] public float normalMoveSpeed = 0.4f;
        [HideInInspector] public float runningSpeed = 0.6f;

        [HideInInspector] private int totalHp = 100;
        [HideInInspector] public int headDmg = 100;
        [HideInInspector] public int bodyDmg = 20;
        [HideInInspector] public int handDmg = 15;
        [HideInInspector] public int legDmg = 15;

        // For Terrorist
        [HideInInspector] public List<string> allDynamicWaypoints;
        [HideInInspector] public float shootTargetDir_AngleMarginOfError = 5.0f;
        [HideInInspector] public float losMarginSize = 0.3f;
        [HideInInspector] public float gunCD = 2f;
        [HideInInspector] public float knifeSwingCD = 3f;

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