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

        [Header("User Set-Able parameters")]
        public bool activateOnSpawn = true;
        public NPCType npcType = NPCType.CIVILLIAN;
        public CivillianStressResponseMode threatResponseMode = CivillianStressResponseMode.FREEZE;

        [Header("Hidden Parameters")]
        public float minAngleToFaceTarget = 5.0f; // Min angle of error when rotating to face target
        public float stopDist = 0.2f; // Stop distance to any target
        public float stopDist_Convo = 1.0f; // Stop distance for conversation

        public float turningSpeed = 2.0f;
        public float normalMoveSpeed = 1.0f;
        public float runningSpeed = 2.0f;

        public float minGunRange = 2.0f;
        public float maxGunRange = 5.0f;
    }
}