using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    public class AIFSM_FollowSchedule : AIFSM_Base
    {
        public void FSM_Update()
        {
            // if all schedule is finished or NPC not active
            if (aiState.general.currSchedule >= npcSchedules.Count)
                return;

            switch (npcSchedules[aiState.general.currSchedule].scheduleType)
            {
                case NPCSchedule.SCHEDULE_TYPE.MOVE_TO_POS:
                    ScheduleProcess_MoveToPosition();
                    break;

                case NPCSchedule.SCHEDULE_TYPE.MOVE_TO_POS_WITH_ROT:
                    ScheduleProcess_MoveToPosition_And_Rotation();
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
                    aiController.Idle_Setup();
                    break;
                case 1:
                    aiController.Idle();
                    break;
                case 2:
                    aiController.Idle_Term();
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
        private void ScheduleProcess_MoveToPosition()
        {
            switch (aiState.general.currSubschedule)
            {
                case 0:
                    aiController.LeaveIfSittingOnSeat();
                    break;
                case 1:
                    aiController.MoveToWaypoint_ProcSetup();
                    break;
                case 2:
                    aiController.MoveToPosition(aiState.general.currWaypointTarget.position, 0);
                    break;
                case 3:
                    aiController.MoveToWaypoint_ProcTerm();
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
        private void ScheduleProcess_MoveToPosition_And_Rotation()
        {
            switch (aiState.general.currSubschedule)
            {
                case 0:
                    aiController.LeaveIfSittingOnSeat();
                    break;
                case 1:
                    aiController.MoveToWaypoint_ProcSetup();
                    break;
                case 2:
                    aiController.MoveToPosition(aiState.general.currWaypointTarget.position, 0);
                    break;
                case 3:
                    aiController.RotateToTargetRotation(aiState.general.currWaypointTarget, false);
                    break;
                case 4:
                    aiController.MoveToWaypoint_ProcTerm();
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
                    aiController.LeaveIfSittingOnSeat();
                    break;
                case 1:
                    aiController.SitDownInArea_Setup();
                    break;
                case 2:
                    aiController.MoveToPosition(aiState.general.currSeatTarget.transform.position, 0);
                    break;
                case 3:
                    aiController.RotateToTargetRotation(aiState.general.currSeatTarget.transform, false);
                    break;
                case 4:
                    aiController.SitDownOnSeat();
                    break;
                case 5:
                    aiController.SitDownInArea_Term();
                    break;
                case 6:
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
                    aiController.LeaveIfSittingOnSeat();
                    break;
                case 1:
                    aiController.TalkToOtherNPC_Setup();
                    break;
                case 2:
                    aiController.MoveToPosition(aiState.general.currConvoNPCTarget.transform.position, aiStats.extraStoppingDistForConvo);
                    break;
                case 3:
                    aiController.MoveToWaypoint_ProcTerm();
                    break;
                case 4:
                    aiController.ConvoProcess_TalkToOtherNPC();
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
        #endregion
        
    }
}
