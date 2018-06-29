using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    [System.Serializable]
    public class AIStats : MonoBehaviour
    {
        // User Set-Able parameters
        public enum NPCType { TERRORIST, VIP, CIVILLIAN };
        public NPCType npcType = NPCType.CIVILLIAN;
        public bool activateOnStart = true;

        // Hidden Parameters
        [Range(0, 100)] public float chanceEnterHostileMode = 0.0f;

        // Min angle to move to 
        public float minAngleToFaceTarget = 5.0f;
        public float stopDist = 0.2f;
        public float stopDist_Convo = 1.0f;

        // Collision avoidance ray legth
        public float collisionAvoidanceRayLen = 0.7f;        // Multiplyer for avoidance vector length
        public float collisionAvoidanceMultiplyer = 1.0f;
        // Enable collision avoidance via raycast
        public bool enableCollisionAvoidance = false;
    }
}