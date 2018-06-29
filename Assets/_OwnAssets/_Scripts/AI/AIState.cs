using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    /// <summary>
    /// Stores various states of the AI
    /// </summary>
    public class AIState : MonoBehaviour
    {
        // If NPC is allowed to function
        public bool active = false;

        [System.Serializable]
        public class General
        {
            public enum AIMode
            {
                FOLLOW_SCHEDULE,
                HOSTILE,
                CIVILIAN_UNDER_ATTACK,
                VIP_UNDER_ATTACK,
                PARTICIPATE_CONVO
            };

            public AIMode aIMode = AIMode.FOLLOW_SCHEDULE;
            // Current schedule NPC is running
            public int currSchedule = 0;
            // Current subschedule NPC is running
            public int currSubschedule = 0;
            // Current timer value, used for delay schedules
            public float currTimerValue = 0;
            // Waypoint NPC traverses to
            public Transform currWaypointTarget = null;

            // Current Seat NPC is on
            public GameObject currSeatTarget = null;
            // If NPC is curently seated
            public bool seated = false;

            // if NPC is waiting for conversation to start
            public bool waitingForConversationToStart = true;
            // If NPC is currently in conversation
            public bool inConversation = false;
            // Current conversation time passed
            public float timeInConvo = 0;
            // Reference to NPC that is in conversation with
            public GameObject currConvoNPCTarget = null;
        }
        public General general = new General();

        [System.Serializable]
        public class Civilian
        {
            [System.Serializable]
            public class UnderAttack
            {
                public enum AI_Civilian_UnderAttack
                {
                    SETUP,
                    FREEZE,
                    RUNTOEXIT
                }
                public AI_Civilian_UnderAttack Civilian_UnderAttack = AI_Civilian_UnderAttack.SETUP;

                public bool bracing = false;
                public float timeLeftBeforeFindingNewRandPosition = 0;
                public Vector3 currMoveVector = Vector3.zero;
            }
            public UnderAttack underAttack = new UnderAttack();
        }
        public Civilian civilian = new Civilian();
    }
}