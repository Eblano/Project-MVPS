using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    /// <summary>
    /// Stores various states of the AI
    /// </summary>
    [System.Serializable]
    public class AIState
    {
        // If NPC is allowed to function
        public bool active = false;

        public enum AIMode
        {
            FOLLOW_SCHEDULE,
            HOSTILE,
            CIVILIAN_UNDER_ATTACK,
            VIP_UNDER_ATTACK,
            PARTICIPATE_CONVO
        };

        // Current AI Mode
        public AIMode aIMode = AIMode.FOLLOW_SCHEDULE;
        // Current schedule NPC is running
        public int currSchedule = 0;
        // Current subschedule NPC is running
        public int currSubschedule = 0;
        // Current timer value, used for delay schedules
        public float currTimerValue = 0;
        // Waypoint position NPC traverses to
        public Vector3 currWaypointPosition;
        // Waypoint rotation NPC traverses to
        public Quaternion currWaypointRotation;

        // Current Seat NPC is on
        public SeatMarker currSeatTarget = null;
        // If NPC is curently seated
        public bool seated = false;
        // Duration npc is seated for
        public float seatedTimePassed = 0;

        // if NPC is waiting for conversation to start
        public bool waitingForConversationToStart = true;
        // If NPC is currently in conversation
        public bool inConversation = false;
        // Current conversation time passed
        public float timeInConvo = 0;
        // Reference to NPC that is in conversation with
        public AIController currConvoNPCTarget = null;

        public bool prepareEnterHostile = false;

        [System.Serializable]
        public class Civilian
        {
            [System.Serializable]
            public class UnderAttack
            {
                public enum Mode
                {
                    SETUP,
                    FREEZE,
                    RUNTOEXIT
                }
                public Mode mode = Mode.SETUP;

                public bool bracing = false;
                public float timeLeftBeforeFindingNewRandPosition = 0;
                public Vector3 currMoveVector = Vector3.zero;
            }
            public UnderAttack underAttack = new UnderAttack();
        }
        public Civilian civilian = new Civilian();

        [System.Serializable]
        public class HostileHuman
        {
            public enum State { IDLE, SHOOT_TARGET, KNIFE_TARGET, MOVE_TO_WAYPOINT };
            public enum ShootTargetState { INACTIVE, SPAWN_GUN, DRAW_GUN, MOVE_TO_SHOOT_TARGET, TRACK_TARGET, AIM_GUN_ON_TARGET, SHOOT }
            public enum KnifeTargetState { INACTIVE, SPAWN_KNIFE, DRAW_KNIFE, MOVE_TO_KNIFE_TARGET, TRACK_TARGET, KNIFE }

            public State currState = State.IDLE;
            public ShootTargetState currShootTargetState = ShootTargetState.INACTIVE;
            public KnifeTargetState currKnifeTargetState = KnifeTargetState.INACTIVE;

            public int schBeforeEnteringHostileMode = 0;
            public Vector3 waypointPos = Vector3.zero;
            public Transform shootTarget = null;
            public Transform knifeTarget = null;
        }
        public HostileHuman hostileHuman = new HostileHuman();

        [System.Serializable]
        public class VIP
        {
            public enum State { IDLE, FOLLOW_PLAYER, GRABBED_FOLLOW_PLAYER };
            public State currState = State.FOLLOW_PLAYER;

            public Transform playerFollowTarget = null;
        }
        public VIP vip = new VIP();
    }
}