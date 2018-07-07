using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    public class AIFSM_FollowSchedule : AIFSM_Base
    {
        // Schedules this NPC has
        protected List<NPCSchedule> npcSchedules;

        public void InitializeFSM(
            AIController aiController,
            Transform aiTransform,
            AIState aiState,
            AIStats aiStats,
            AIAnimationController aiAnimController,
            List<NPCSchedule> npcSchedules
            )
        {
            this.aiController = aiController;
            this.aiTransform = aiTransform;
            this.aiState = aiState;
            this.aiStats = aiStats;
            this.aiAnimController = aiAnimController;
            this.npcSchedules = npcSchedules;
        }

        public void FSM_Update()
        {
            // if all schedule is finished or NPC not active
            if (aiState.general.currSchedule >= npcSchedules.Count)
                return;

            switch (npcSchedules[aiState.general.currSchedule].scheduleType)
            {
                case NPCSchedule.SCHEDULE_TYPE.MOVE_TO_POS:
                    ScheduleProcess_MoveToWaypoint();
                    break;

                case NPCSchedule.SCHEDULE_TYPE.MOVE_TO_POS_WITH_ROT:
                    ScheduleProcess_MoveToWaypoint_And_Rotate();
                    break;

                case NPCSchedule.SCHEDULE_TYPE.IDLE:
                    ScheduleProcess_Idle();
                    break;

                case NPCSchedule.SCHEDULE_TYPE.SIT_IN_AREA:
                    ScheduleProcess_SitDownInArea();
                    break;

                case NPCSchedule.SCHEDULE_TYPE.TALK_TO_OTHER_NPC:
                    ScheduleProcess_TalkToOtherNPCs();
                    break;
            }

        }
        #region Schedule Processes Methods
        /// <summary>
        /// Run delay timer schedule
        /// </summary>
        private void ScheduleProcess_Idle()
        {
            switch (aiState.general.currSubschedule)
            {
                case 0:
                    Idle_Setup();
                    break;
                case 1:
                    Idle();
                    break;
                case 2:
                    Idle_Term();
                    break;
                case 3:
                    aiState.general.currSubschedule = 0;
                    aiState.general.currSchedule++;
                    break;
                default:
                    aiState.general.currSubschedule = 0;
                    aiState.general.currSchedule++;
                    break;
            }
        }

        /// <summary>
        /// Run move to position schedule
        /// </summary>
        private void ScheduleProcess_MoveToWaypoint()
        {
            switch (aiState.general.currSubschedule)
            {
                case 0:
                    LeaveSeatIfSeated();
                    break;
                case 1:
                    Setup_MoveToWaypoint();
                    break;
                case 2:
                    MoveToWaypoint(aiState.general.currWaypointPosition, aiStats.normalMoveSpeed, 0);
                    break;
                case 3:
                    Terminate_MoveToWaypoint();
                    break;
                case 4:
                    aiState.general.currSubschedule = 0;
                    aiState.general.currSchedule++;
                    break;
                default:
                    aiState.general.currSubschedule = 0;
                    aiState.general.currSchedule++;
                    break;
            }
        }

        /// <summary>
        /// Run move to position with rotation schedule
        /// </summary>
        private void ScheduleProcess_MoveToWaypoint_And_Rotate()
        {
            switch (aiState.general.currSubschedule)
            {
                case 0:
                    LeaveSeatIfSeated();
                    break;
                case 1:
                    Setup_MoveToWaypoint();
                    break;
                case 2:
                    MoveToWaypoint(aiState.general.currWaypointPosition, aiStats.normalMoveSpeed, 0);
                    break;
                case 3:
                    RotateToTargetRotation(aiState.general.currWaypointRotation);
                    break;
                case 4:
                    Terminate_MoveToWaypoint();
                    break;
                case 5:
                    aiState.general.currSubschedule = 0;
                    aiState.general.currSchedule++;
                    break;
                default:
                    aiState.general.currSubschedule = 0;
                    aiState.general.currSchedule++;
                    break;
            }
        }

        /// <summary>
        /// Sit down on a seat in the area
        /// </summary>
        private void ScheduleProcess_SitDownInArea()
        {
            switch (aiState.general.currSubschedule)
            {
                case 0:
                    LeaveSeatIfSeated();
                    break;
                case 1:
                    Setup_SitDownInArea();
                    break;
                case 2:
                    MoveToWaypoint(aiState.general.currSeatTarget.transform.position, aiStats.normalMoveSpeed, 0);
                    break;
                case 3:
                    RotateToTargetRotation(aiState.general.currSeatTarget.transform.rotation);
                    break;
                case 4:
                    SitDownOnSeat();
                    break;
                case 5:
                    SitAndWaitForTime();
                    break;
                case 6:
                    Terminate_SetDownInArea();
                    break;
                case 7:
                    aiState.general.currSubschedule = 0;
                    aiState.general.currSchedule++;
                    break;
                default:
                    aiState.general.currSubschedule = 0;
                    aiState.general.currSchedule++;
                    break;
            }
        }

        /// <summary>
        /// Talking to ther othe NPC
        /// </summary>
        private void ScheduleProcess_TalkToOtherNPCs()
        {
            switch (aiState.general.currSubschedule)
            {
                case 0:
                    LeaveSeatIfSeated();
                    break;
                case 1:
                    TalkToOtherNPC_FindPartner();
                    break;
                case 2:
                    MoveToWaypoint(aiState.general.currConvoNPCTarget.transform.position, aiStats.normalMoveSpeed, aiStats.stopDist_Convo);
                    break;
                case 3:
                    Terminate_MoveToWaypoint();
                    break;
                case 4:
                    Setup_TalkToOtherNPC();
                    break;
                case 5:
                    TalkToOtherNPC();
                    break;
                case 6:
                    Terminate_TalkToOtherNPC();
                    break;
                case 7:
                    aiState.general.currSubschedule = 0;
                    aiState.general.currSchedule++;
                    break;
                default:
                    aiState.general.currSubschedule = 0;
                    aiState.general.currSchedule++;
                    break;
            }
        }
        #endregion

        public void Setup_MoveToWaypoint()
        {
            aiState.general.currWaypointPosition = GetWaypointMarkerPosition();
            aiState.general.currWaypointRotation = GetWaypointMarkerRotation();
            aiController.SetNMAgentDestination(aiState.general.currWaypointPosition);
            aiState.general.currSubschedule++;
        }

        public bool MoveToWaypoint(Vector3 waypointPos, float moveSpeed, float extraStoppingDistance)
        {
            if (!aiController.ReachedDestination(aiState.general.currWaypointPosition, extraStoppingDistance))
            {
                aiController.MoveAITowardsNMAgentDestination(moveSpeed);
                return false;
            }
            else
            {
                aiState.general.currSubschedule++;
                return true;
            }
        }

        public Vector3 GetWaypointMarkerPosition()
        {
            string targetName = npcSchedules[aiState.general.currSchedule].argument_1;
            return GameManager.instance.GetWaypointMarkerPosition(targetName);
        }

        public Quaternion GetWaypointMarkerRotation()
        {
            string targetName = npcSchedules[aiState.general.currSchedule].argument_1;
            return GameManager.instance.GetWaypointMarkerRotation(targetName);
        }

        public void Terminate_MoveToWaypoint()
        {
            aiController.StopMovement();
            aiState.general.currWaypointPosition = new Vector3();
            aiState.general.currSubschedule++;
        }

        public void LeaveSeatIfSeated()
        {
            bool leftSeat = aiController.LeaveSeat();

            if(leftSeat)
                aiState.general.seated = false;
                aiState.general.currSubschedule++;
        }

        public void RotateToTargetRotation(Quaternion targetRotation)
        {
            bool rotateComplete = aiController.RotateTowardsTargetRotation(targetRotation);

            if (rotateComplete)
            {
                aiState.general.currSubschedule++;
            }
        }

        public void Idle_Setup()
        {
            aiState.general.currTimerValue = float.Parse(npcSchedules[aiState.general.currSchedule].argument_1);
            aiState.general.currSubschedule++;
        }

        public void Idle()
        {
            if (aiState.general.currTimerValue > 0)
            {
                aiState.general.currTimerValue -= Time.deltaTime;
            }
            else
            {
                aiState.general.currSubschedule++;
            }
        }

        public void Idle_Term()
        {
            aiState.general.currTimerValue = 0;
            aiState.general.currSubschedule++;
        }

        public void Setup_SitDownInArea()
        {
            // Get Area
            AreaMarker areaMarker = GameManager.instance.GetAreaMarkerByName(npcSchedules[aiState.general.currSchedule].argument_1);
            // Empty seat from selected Area
            aiState.general.currSeatTarget = areaMarker.GetRandomEmptySeat();

            if (aiState.general.currSeatTarget)
            {
                aiState.general.currSeatTarget.GetComponent<SeatMarker>().SetSeatAvailability(false);
                aiController.SetNMAgentDestination(aiState.general.currSeatTarget.transform.position);
                aiState.general.currSubschedule++;
            }
            else
            {
                Debug.Log("No Seat Found in " + areaMarker.name);
                aiState.general.currSubschedule = -1;
            }
        }

        public void Terminate_SetDownInArea()
        {
            bool notSitting = aiController.LeaveSeat();
            aiState.general.seated = false;

            if (notSitting)
            {
                aiController.StopMovement();
                aiState.general.currSubschedule++;
            }
        }

        public void SitDownOnSeat()
        {
            bool seated = aiController.SitDown();

            if(seated)
            {
                aiState.general.seated = true;
                aiController.AddAction("Leave Seat (Next Sch)");
                aiState.general.currSubschedule++;
            }
        }

        public void End_SitAndWaitForTime()
        {
            aiState.general.seatedTimePassed = float.Parse(npcSchedules[aiState.general.currSchedule].argument_2);
        }

        public void SitAndWaitForTime()
        {
            float totalTimeAllocated = float.Parse(npcSchedules[aiState.general.currSchedule].argument_2);

            if (aiState.general.seatedTimePassed < totalTimeAllocated)
            {
                aiState.general.seatedTimePassed += Time.deltaTime;
            }
            else
            {
                aiState.general.seatedTimePassed = 0;
                aiController.RemoveAction("Leave Seat (Next Sch)");
                aiState.general.currSubschedule++;
            }
        }

        public void TalkToOtherNPC_FindPartner()
        {
            // Get gameobject of nearest NPC
            AIController otherNPC = GameManager.instance.GetNearestAvailableCivilianNPCForConvo(aiController);

            if(otherNPC && otherNPC.AvailableForConversation()) // If found NPC & able to convo
            {
                // Request for conversation
                bool reqResults = otherNPC.RequestStartConvo(aiController);

                if (reqResults == true)
                {
                    aiState.general.currConvoNPCTarget = otherNPC;
                    aiController.SetNMAgentDestination(aiState.general.currConvoNPCTarget.transform.position);
                    aiState.general.currSubschedule++;
                }
                else
                {
                    Debug.Log("Failed to find available NPC");
                    aiState.general.currSubschedule = -1;
                }
            }
        }

        public void Setup_TalkToOtherNPC()
        {
            aiState.general.currConvoNPCTarget.StartConvoWithConvoNPCTarget();
            aiAnimController.Anim_StartStandTalking();
            aiState.general.inConversation = true;
            aiState.general.currSubschedule++;
        }

        public void TalkToOtherNPC()
        {
            float conversationDuration = float.Parse(npcSchedules[aiState.general.currSchedule].argument_1);

            if (aiState.general.timeInConvo > conversationDuration)
                aiState.general.currSubschedule++;
            else
                aiState.general.timeInConvo += Time.deltaTime;
        }

        public void Terminate_TalkToOtherNPC()
        {
            aiState.general.inConversation = false;
            aiState.general.timeInConvo = 0;
            aiAnimController.Anim_StopStandTalking();
            aiState.general.currConvoNPCTarget.EndConvoWithConvoNPCTarget();
            aiState.general.currSubschedule++;
        }
    }
}
