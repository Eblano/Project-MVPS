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

        // For Civillian
        public CivillianStressResponseMode threatResponseMode = CivillianStressResponseMode.FREEZE;

        // For Terrorist
        public List<string> allDynamicWaypoints;
        public float shootTargetDir_AngleMarginOfError = 20.0f;
        public float losMarginSize = 0.3f;
        public float maxGunRange = 5.0f;

        // For VIP
        public float vipFollowPlayerDistance = 2f;
        public float vipGrabbedPlayerDistance = 0.3f;

        [Header("Hidden Parameters")]
        public float lookAngleMarginOfError = 5.0f; // Min angle of error when rotating to face target
        public float stopDist = 0.2f; // Stop distance to any target
        public float stopDist_Convo = 1.0f; // Stop distance for conversation

        public float turningSpeed = 10.0f;
        public float normalMoveSpeed = 1.0f;
        public float runningSpeed = 2.0f;

        public int totalHp = 100;
        public int headDmg = 100;
        public int bodyDmg = 20;
        public int handDmg = 15;
        public int legDmg = 15;
    }
}