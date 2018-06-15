using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AIStats
{
    //public bool isTerrorist;
    //public bool isVIP;
    public enum AiType { TERRORIST, VIP, CIVILLIAN };
    public AiType aiType = AiType.CIVILLIAN;

    [Range(0, 100)] public float chanceEnterHostileMode = 0.0f;

    // Min angle to move to 
    public float minAngleToFaceTarget = 5.0f;
    // Extra Stopping distance for conversation
    public float extraStoppingDistForConvo = 1.0f;

    // Collision avoidance ray legth
    public float collisionAvoidanceRayLen = 0.7f;        // Multiplyer for avoidance vector length
    public float collisionAvoidanceMultiplyer = 1.0f;
    // Enable collision avoidance via raycast
    public bool enableCollisionAvoidance = false;
}
