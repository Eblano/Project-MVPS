using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    [System.Serializable]
    public class AIStats
    {
        public bool activateOnStart = true;

        public enum NPCType { TERRORIST, VIP, CIVILLIAN };
        public enum CivillianStressResponseMode { FREEZE, RUNTOEXIT, RANDOM }

        [Header("User Set-Able parameters")]
        public NPCType npcType = NPCType.CIVILLIAN;
        public CivillianStressResponseMode threatResponseMode = CivillianStressResponseMode.FREEZE;

        [Header("Hidden Parameters")]
        [Range(0, 100)] public float chanceEnterHostileMode = 0.0f;
        public float minAngleToFaceTarget = 5.0f; // Min angle of error when rotating to face target
        public float stopDist = 0.2f; // Stop distance to any target
        public float stopDist_Convo = 1.0f; // Stop distance for conversation
        public float turningSpeed = 2.0f;
    }
}