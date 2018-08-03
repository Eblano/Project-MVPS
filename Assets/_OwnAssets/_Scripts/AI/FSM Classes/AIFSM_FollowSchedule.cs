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
            if (aiState.currSchedule >= npcSchedules.Count)
                return;

            switch (npcSchedules[aiState.currSchedule].scheduleType)
            {
                case NPCSchedule.SCHEDULE_TYPE.MOVE_TO_WAYPT:
                    ScheduleProcess_MoveToWaypoint();
                    break;

                case NPCSchedule.SCHEDULE_TYPE.MOVE_TO_WAYPT_ROT:
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
            switch (aiState.currSubschedule)
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
                    aiState.currSubschedule = 0;
                    aiState.currSchedule++;
                    break;
                default:
                    aiState.currSubschedule = 0;
                    aiState.currSchedule++;
                    break;
            }
        }

        /// <summary>
        /// Run move to position schedule
        /// </summary>
        private void ScheduleProcess_MoveToWaypoint()
        {
            switch (aiState.currSubschedule)
            {
                case 0:
                    LeaveSeatIfSeated();
                    break;
                case 1:
                    Setup_MoveToWaypoint();
                    break;
                case 2:
                    MoveToWaypoint(aiState.currWaypointPosition, aiStats.normalMoveSpeed, 0);
                    break;
                case 3:
                    Terminate_MoveToWaypoint();
                    break;
                case 4:
                    aiState.currSubschedule = 0;
                    aiState.currSchedule++;
                    break;
                default:
                    aiState.currSubschedule = 0;
                    aiState.currSchedule++;
                    break;
            }
        }

        /// <summary>
        /// Run move to position with rotation schedule
        /// </summary>
        private void ScheduleProcess_MoveToWaypoint_And_Rotate()
        {
            switch (aiState.currSubschedule)
            {
                case 0:
                    LeaveSeatIfSeated();
                    break;
                case 1:
                    Setup_MoveToWaypoint();
                    break;
                case 2:
                    MoveToWaypoint(aiState.currWaypointPosition, aiStats.normalMoveSpeed, 0);
                    break;
                case 3:
                    RotateToTargetRotation(aiState.currWaypointRotation);
                    break;
                case 4:
                    Terminate_MoveToWaypoint();
                    break;
                case 5:
                    aiState.currSubschedule = 0;
                    aiState.currSchedule++;
                    break;
                default:
                    aiState.currSubschedule = 0;
                    aiState.currSchedule++;
                    break;
            }
        }

        /// <summary>
        /// Sit down on a seat in the area
        /// </summary>
        private void ScheduleProcess_SitDownInArea()
        {
            switch (aiState.currSubschedule)
            {
                case 0:
                    LeaveSeatIfSeated();
                    break;
                case 1:
                    Setup_SitDownInArea();
                    break;
                case 2:
                    MoveToWaypoint(aiState.currSeatTarget.transform.position, aiStats.normalMoveSpeed, 0);
                    break;
                case 3:
                    RotateToTargetRotation(aiState.currSeatTarget.transform.rotation);
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
                    aiState.currSubschedule = 0;
                    aiState.currSchedule++;
                    break;
                default:
                    aiState.currSubschedule = 0;
                    aiState.currSchedule++;
                    break;
            }
        }

        /// <summary>
        /// Talking to ther othe NPC
        /// </summary>
        private void ScheduleProcess_TalkToOtherNPCs()
        {
            switch (aiState.currSubschedule)
            {
                case 0:
                    LeaveSeatIfSeated();
                    break;
                case 1:
                    TalkToOtherNPC_FindPartner(npcSchedules[aiState.currSchedule].argument_1);
                    break;
                case 2:
                    MoveToWaypoint(aiState.currConvoNPCTarget.transform.position, aiStats.normalMoveSpeed, aiStats.stopDist_Convo);
                    break;
                case 3:
                    Terminate_MoveToWaypoint();
                    break;
                case 4:
                    RotateLookAtConvoNPC();
                    break;
                case 5:
                    Setup_TalkToOtherNPC();
                    break;
                case 6:
                    TalkToOtherNPC();
                    break;
                case 7:
                    Terminate_TalkToOtherNPC();
                    break;
                case 8:
                    aiState.currSubschedule = 0;
                    aiState.currSchedule++;
                    break;
                default:
                    aiState.currSubschedule = 0;
                    aiState.currSchedule++;
                    break;
            }
        }
        #endregion

        public void Setup_MoveToWaypoint()
        {
            aiState.currWaypointPosition = GetWaypointMarkerPosition();
            aiState.currWaypointRotation = GetWaypointMarkerRotation();
            aiController.SetNMAgentDestination(aiState.currWaypointPosition);
            aiController.AddAction("Skip Waypoint (Next)");
            aiState.currSubschedule++;
        }

        public void MoveToWaypoint(Vector3 waypointPos, float moveSpeed, float extraStoppingDistance)
        {
            if (!aiController.ReachedDestination(aiState.currWaypointPosition, extraStoppingDistance))
            {
                if(!aiState.currSeatTarget.SeatTakenByOwnSelf(aiController))
                {
                    aiState.currSubschedule--;
                    return;
                }

                aiController.MoveAITowardsNMAgentDestination(moveSpeed);
            }
            else
                aiState.currSubschedule++;
        }

        public void SetAction_End_MoveToWaypoint()
        {
            aiState.currSubschedule++;
        }

        public void SetAction_End_MoveToWaypointAndRotate()
        {
            aiState.currSubschedule += 2;
        }

        public void RotateLookAtConvoNPC()
        {
            aiController.RotateTowardsTargetDirection(aiState.currConvoNPCTarget.transform.position);
            if (aiController.LookingAtTarget(aiState.currConvoNPCTarget.transform.position, aiStats.lookAngleMarginOfError))
            {
                aiController.StopRotation();
                aiState.currSubschedule++;
            }
        }

        public Vector3 GetWaypointMarkerPosition()
        {
            string targetName = npcSchedules[aiState.currSchedule].argument_1;
            return GameManager.instance.GetWaypointMarkerPosition(targetName);
        }

        public Quaternion GetWaypointMarkerRotation()
        {
            string targetName = npcSchedules[aiState.currSchedule].argument_1;
            return GameManager.instance.GetWaypointMarkerRotation(targetName);
        }

        public void Terminate_MoveToWaypoint()
        {
            aiController.RemoveAction("Skip Waypoint (Next)");
            aiController.StopMovement();
            aiState.currWaypointPosition = new Vector3();
            aiState.currSubschedule++;
        }

        public void LeaveSeatIfSeated()
        {
            bool leftSeat = aiController.LeaveSeat();

            if(leftSeat)
                aiState.seated = false;
                aiState.currSubschedule++;
        }

        public void RotateToTargetRotation(Quaternion targetRotation)
        {
            bool rotateComplete = aiController.RotateTowardsTargetRotation(targetRotation);

            if (rotateComplete)
            {
                aiState.currSubschedule++;
            }
        }

        public void Idle_Setup()
        {
            aiState.currTimerValue = float.Parse(npcSchedules[aiState.currSchedule].argument_1);
            aiController.AddAction("End Idle (Next)");
            aiState.currSubschedule++;
        }

        public void Idle()
        {
            if (aiState.currTimerValue > 0)
            {
                aiState.currTimerValue -= Time.deltaTime;
            }
            else
            {
                aiState.currSubschedule++;
            }
        }

        public void End_Idle()
        {
            aiState.currTimerValue = 0;
        }

        public void Idle_Term()
        {
            aiState.currTimerValue = 0;
            aiController.RemoveAction("End Idle (Next)");
            aiState.currSubschedule++;
        }

        public void Setup_SitDownInArea()
        {
            // Get Area
            AreaMarker areaMarker = GameManager.instance.GetAreaMarkerByName(npcSchedules[aiState.currSchedule].argument_1);
            // Empty seat from selected Area
            aiState.currSeatTarget = areaMarker.GetRandomEmptySeat(aiController);

            if (aiState.currSeatTarget)
            {
                aiController.SetNMAgentDestination(aiState.currSeatTarget.transform.position);
                aiState.currSubschedule++;
            }
            else
            {
                Debug.Log("No Seat Found in " + areaMarker.name);
            }
        }

        public void Terminate_SetDownInArea()
        {
            bool notSitting = aiController.LeaveSeat();

            if (notSitting)
            {
                aiController.StopMovement();
                aiState.currSubschedule++;
            }
        }

        public void SitDownOnSeat()
        {
            bool seated = aiController.SitDown();

            if(seated)
            {
                AreaMarker areaMarker = GameManager.instance.GetAreaMarkerByName(npcSchedules[aiState.currSchedule].argument_1);

                aiState.seated = true;
                aiController.AddAction("Dismiss from Seat (Next)");
                aiState.currSubschedule++;
            }
        }

        public void End_SitAndWaitForTime()
        {
            aiState.seatedTimePassed = float.Parse(npcSchedules[aiState.currSchedule].argument_2);
        }

        public void SitAndWaitForTime()
        {
            float totalTimeAllocated = float.Parse(npcSchedules[aiState.currSchedule].argument_2);

            if (aiState.seatedTimePassed < totalTimeAllocated)
            {
                aiState.seatedTimePassed += Time.deltaTime;
            }
            else
            {
                AreaMarker areaMarker = GameManager.instance.GetAreaMarkerByName(npcSchedules[aiState.currSchedule].argument_1);
                areaMarker.UnregisterNPCSitInArea(aiController);

                aiState.seatedTimePassed = 0;
                aiController.RemoveAction("Dismiss from Seat (Next)");
                aiState.currSubschedule++;
            }
        }

        public void TalkToOtherNPC_FindPartner(string targetNPCName)
        {
            // Get gameobject of nearest NPC
            AIController otherNPC = GameManager.instance.GetNPCForConvo(targetNPCName, aiController);

            if(otherNPC && otherNPC.AvailableForConversation()) // If found NPC & able to convo
            {
                // Request for conversation
                bool reqResults = otherNPC.RequestStartConvo(aiController);

                if (reqResults == true)
                {
                    aiState.currConvoNPCTarget = otherNPC;
                    aiController.SetNMAgentDestination(aiState.currConvoNPCTarget.transform.position);
                    aiState.currSubschedule++;
                }
                else
                {
                    Debug.Log("Failed to find available NPC");
                    aiState.currSubschedule = -1;
                }
            }
        }

        public void Setup_TalkToOtherNPC()
        {
            aiState.currConvoNPCTarget.StartConvoWithConvoNPCTarget();
            aiAnimController.Anim_StartStandTalking();
            aiState.inConversation = true;
            aiState.currSubschedule++;
            aiController.AddAction("End Conversation (Next)");
        }

        public void TalkToOtherNPC()
        {
            float conversationDuration = float.Parse(npcSchedules[aiState.currSchedule].argument_2);

            if (aiState.timeInConvo >= conversationDuration)
                aiState.currSubschedule++;
            else
                aiState.timeInConvo += Time.deltaTime;
        }

        public void SetAction_End_TalkToOtherNPC()
        {
            aiState.timeInConvo = float.Parse(npcSchedules[aiState.currSchedule].argument_2);
        }

        public void Terminate_TalkToOtherNPC()
        {
            aiState.inConversation = false;
            aiState.timeInConvo = 0;
            aiAnimController.Anim_StopStandTalking();
            aiState.currConvoNPCTarget.EndConvoWithConvoNPCTarget();
            aiState.currConvoNPCTarget = null;
            aiController.RemoveAction("End Conversation (Next)");
            aiState.currSubschedule++;
        }
    }
}
