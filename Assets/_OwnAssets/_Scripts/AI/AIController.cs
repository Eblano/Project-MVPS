using System.Collections.Generic;
using UnityEngine;
using ProtoBuf;
using UnityEngine.AI;
using UnityEngine.Events;

/// <summary>
/// Manages AI thought process
/// </summary>
namespace SealTeam4
{
    public class AIController : MonoBehaviour, IActions, IObjectInfo
    {
        private string npcName;

        private NavMeshAgent nmAgent;
        private AIAnimationController aiAnimController;
        private AIAnimEventReciever aiAnimEventReciever;

        // Stores various state of this AI
        [SerializeField] private AIState aiState;
        // Stores various stats of this AI
        [SerializeField] private AIStats aiStats;

        // FSM classes
        AIFSM_FollowSchedule aiFSM_FollowSchedule = new AIFSM_FollowSchedule();
        AIFSM_Schedule_ParticipateConvo aiFSM_ParticipateConvo = new AIFSM_Schedule_ParticipateConvo();
        AIFSM_Civillian_UnderAttack aiFSM_Civillian_UnderAttack = new AIFSM_Civillian_UnderAttack();
        //AIFSM_HostileTerrorist 

        // Schedules this NPC has
        private List<NPCSchedule> npcSchedules;

        [SerializeField] private List<string> actionableParameters = new List<string>();

        // Exposed Variables for InterfaceManager
        [Header("Exposed for InterfaceManager")]
        [SerializeField] private Transform highestPoint;
        [SerializeField] private Collider col;

        public void Setup(string npcName, AIStats aiStats, List<NPCSchedule> npcSchedules)
        {
            this.npcName = npcName;
            this.aiStats = aiStats;
            this.npcSchedules = npcSchedules;

            nmAgent = GetComponent<NavMeshAgent>();
            aiAnimController = GetComponent<AIAnimationController>();
            aiAnimEventReciever = GetComponent<AIAnimEventReciever>();
            this.aiStats = aiStats;

            // Initializing FSM classes
            aiFSM_FollowSchedule.InitializeFSM(this, transform, aiState, aiStats, aiAnimController, npcSchedules);
            aiFSM_ParticipateConvo.InitializeFSM(this, transform, aiState, aiStats, aiAnimController);
            aiFSM_Civillian_UnderAttack.InitializeFSM(this, transform, aiState, aiStats, aiAnimController);
        }

        private void Update()
        {
            UpdateActionableParameters();

            if (!aiState.active)
                return;

            // if area under attack
            if (GameManager.instance.areaUnderAttack)
            {
                switch (aiStats.npcType)
                {
                    case AIStats.NPCType.VIP:
                        aiState.general.aIMode = AIState.General.AIMode.VIP_UNDER_ATTACK;
                        break;
                        
                    case AIStats.NPCType.CIVILLIAN:
                        aiState.general.aIMode = AIState.General.AIMode.CIVILIAN_UNDER_ATTACK;
                        break;
                }
            }

            switch (aiState.general.aIMode)
            {
                case AIState.General.AIMode.FOLLOW_SCHEDULE:
                    aiFSM_FollowSchedule.FSM_Update();
                    break;
                case AIState.General.AIMode.HOSTILE:
                    break;
                case AIState.General.AIMode.CIVILIAN_UNDER_ATTACK:
                    aiFSM_Civillian_UnderAttack.FSM_Update();
                    break;
                case AIState.General.AIMode.PARTICIPATE_CONVO:
                    aiFSM_ParticipateConvo.FSM_Update();
                    break;
                default:
                    break;
            }
        }
        
        public bool RequestStartConvo(AIController requester)
        {
            if(AvailableForConversation())
            {
                aiState.general.aIMode = AIState.General.AIMode.PARTICIPATE_CONVO;
                aiState.general.waitingForConversationToStart = true;
                StopMovement();
                aiState.general.currConvoNPCTarget = requester;
                return true;
            }
            else
            {
                Debug.Log(gameObject.name + " denied conversation request");
                return false;
            }
        }

        public void StartConvoWithConvoNPCTarget()
        {
            aiState.general.inConversation = true;
            aiAnimController.Anim_StartStandTalking();
        }
        
        public void EndConvoWithConvoNPCTarget()
        {
            aiState.general.inConversation = false;
            aiState.general.aIMode = AIState.General.AIMode.FOLLOW_SCHEDULE;
            aiState.general.currConvoNPCTarget = null;
            aiAnimController.Anim_StopStandTalking();
            aiState.general.timeInConvo = 0;
        }
        
        public bool AvailableForConversation()
        {
            return !aiState.general.seated && aiState.active && !aiState.general.inConversation;
        }
        
        public void SetNMAgentDestination(Vector3 position)
        {
            nmAgent.SetDestination(position);
        }

        public bool ReachedDestination(Vector3 destination, float extraStoppingDistance)
        {
            return nmAgent.remainingDistance < aiStats.stopDist + extraStoppingDistance;
        }

        public void MoveAITowardsNMAgentDestination(float speed)
        {
            aiAnimController.Anim_Move(nmAgent.desiredVelocity, speed);
        }

        public bool RotateTowardsTargetRotation(Quaternion targetRotation)
        {
            StopMovement();
            aiAnimController.Anim_Turn(targetRotation);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * aiStats.turningSpeed);

            return Quaternion.Angle(transform.rotation, targetRotation) < aiStats.minAngleToFaceTarget;
        }

        public bool RotateTowardsTargetDirection(Vector3 targetPosition)
        {
            StopMovement();

            Vector3 direction = targetPosition - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            aiAnimController.Anim_Turn(lookRotation);

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * aiStats.turningSpeed);
            return Quaternion.Angle(transform.rotation, lookRotation) < aiStats.minAngleToFaceTarget;
        }

        public void StopMovement()
        {
            SetNMAgentDestination(transform.position);
            aiAnimController.Anim_Move(Vector3.zero, 1);
        }

        public void StopRotation()
        {
            aiAnimController.Anim_StopTurn();
        }

        public void StopStandTalking()
        {
            aiAnimController.Anim_StopStandTalking();
        }

        public bool LeaveSeat()
        {
            if (aiState.general.seated)
            {
                aiAnimController.Anim_Stand();
            }

            if (aiAnimEventReciever.standing_Completed || !aiState.general.seated)
            {
                if (aiState.general.currSeatTarget)
                {
                    aiState.general.currSeatTarget.GetComponent<SeatMarker>().SetSeatAvailability(true);
                    aiState.general.currSeatTarget = null;
                }
                return true;
            }
            else
                return false;
        }

        public bool SitDown()
        {
            aiAnimController.Anim_Sit();
            return aiAnimEventReciever.sitting_Completed;
        }
                 
        public void FadeAway()
        {
            Destroy(this.gameObject);
        }

        public void AISetActive()
        {
            aiState.active = true;
        }
        
        #region Interface methods
        public List<string> GetActions()
        {
            return actionableParameters;
        }

        public void SetAction(string action)
        {
            switch (action)
            {
                case "Activate NPC":
                    SetAction_ActivateNPC();
                    break;

                case "Fade Away(Debug)":
                    SetAction_KillNPC();
                    break;

                case "Dismiss from Seat (Next)":
                    aiFSM_FollowSchedule.End_SitAndWaitForTime();
                    break;

                case "End Idle (Next)":
                    aiFSM_FollowSchedule.End_Idle();
                    break;

                case "Skip Waypoint (Next)":
                    if (npcSchedules[aiState.general.currSchedule].scheduleType == NPCSchedule.SCHEDULE_TYPE.MOVE_TO_WAYPT)
                        aiFSM_FollowSchedule.End_MoveToWaypoint();
                    else if (npcSchedules[aiState.general.currSchedule].scheduleType == NPCSchedule.SCHEDULE_TYPE.MOVE_TO_WAYPT_ROT)
                        aiFSM_FollowSchedule.End_MoveToWaypointAndRotate();
                    break;
            }
        }

        public string GetName()
        {
            return npcName;
        }

        public Vector3 GetHighestPointPos()
        {
            return highestPoint.position;
        }

        public Transform GetHighestPointTransform()
        {
            return highestPoint;
        }

        public Collider GetCollider()
        {
            return col;
        }

        public bool IsActivateFromSpawn()
        {
            return aiStats.activateOnSpawn;
        }


        public void AddAction(string action)
        {
            actionableParameters.Add(action);
        }

        public void RemoveAction(string action)
        {
            if(actionableParameters.Exists(x => x == action))
            {
                actionableParameters.Remove(action);
            }
        }

        private void UpdateActionableParameters()
        {
            if (!aiState.active && !actionableParameters.Contains("Activate NPC"))
                actionableParameters.Add("Activate NPC");
            
            if (aiState.active && actionableParameters.Exists(x => x == "Activate NPC"))
                actionableParameters.Remove("Activate NPC");

            //if (aiState.active && !actionableParameters.Contains("Fade Away(Debug)"))
            //{
            //    actionableParameters.Add("Fade Away(Debug)");
            //}

            if (aiStats.npcType == AIStats.NPCType.TERRORIST && aiState.general.aIMode != AIState.General.AIMode.HOSTILE)
                actionableParameters.Add("Enter Hostile Mode");

            if (aiState.general.aIMode == AIState.General.AIMode.HOSTILE && actionableParameters.Exists(x => x == "Enter Hostile Mode"))
                actionableParameters.Remove("Enter Hostile Mode");
        }

        private void SetAction_ActivateNPC()
        {
            aiState.active = true;
            actionableParameters.Remove("Activate NPC");
        }

        private void SetAction_KillNPC()
        {
            actionableParameters.Remove("Fade Away(Debug)");
            FadeAway();
        }

        public List<ObjectInfo> GetObjectInfos()
        {
            ObjectInfo objInfo1 = new ObjectInfo();
            objInfo1.contentIndexToHighlight = -1;
            objInfo1.title = "NPC Info";
            switch (aiState.general.aIMode)
            {
                case AIState.General.AIMode.FOLLOW_SCHEDULE:
                    objInfo1.content.Add("Follow Schedule");
                    break;
                case AIState.General.AIMode.HOSTILE:
                    objInfo1.content.Add("Hostile Schedule");
                    break;
                case AIState.General.AIMode.CIVILIAN_UNDER_ATTACK:
                    objInfo1.content.Add("Civillian Under Attack");
                    break;
                case AIState.General.AIMode.VIP_UNDER_ATTACK:
                    objInfo1.content.Add("VIP Under Attack");
                    break;
                case AIState.General.AIMode.PARTICIPATE_CONVO:
                    objInfo1.content.Add("Talking to other NPC");
                    break;
                default:
                    objInfo1.content.Add("-");
                    break;
            }
            switch (aiStats.npcType)
            {
                case AIStats.NPCType.TERRORIST:
                    objInfo1.content.Add("Type: Terrorist");
                    break;
                case AIStats.NPCType.VIP:
                    objInfo1.content.Add("Type: VIP");
                    break;
                case AIStats.NPCType.CIVILLIAN:
                    objInfo1.content.Add("Type: Civillian");
                    break;
                default:
                    break;
            }

            ObjectInfo objInfo2 = new ObjectInfo();
            objInfo2.title = "Schedules";
            objInfo2.contentIndexToHighlight = aiState.general.currSchedule;

            foreach(NPCSchedule schedule in npcSchedules)
            {
                switch(schedule.scheduleType)
                {
                    case NPCSchedule.SCHEDULE_TYPE.IDLE:
                        objInfo2.content.Add("Idle for " + schedule.argument_1 + "s");
                        break;
                    case NPCSchedule.SCHEDULE_TYPE.MOVE_TO_WAYPT_ROT:
                        objInfo2.content.Add("Move to waypoint \"" + schedule.argument_1 + "\" and rotate");
                        break;
                    case NPCSchedule.SCHEDULE_TYPE.MOVE_TO_WAYPT:
                        objInfo2.content.Add("Move to waypoint \"" + schedule.argument_1 + "\"");
                        break;
                    case NPCSchedule.SCHEDULE_TYPE.SIT_IN_AREA:
                        objInfo2.content.Add("Sit in empty seat in \"" + schedule.argument_1 + "\" for " + schedule.argument_2 + "s");
                        break;
                    case NPCSchedule.SCHEDULE_TYPE.TALK_TO_OTHER_NPC:
                        objInfo2.content.Add("Talk to nearest NPC");
                        break;
                    default:
                        objInfo2.content.Add("???");
                        break;
                }
            }

            List<ObjectInfo> objInfos = new List<ObjectInfo>
            {
                objInfo1,
                objInfo2
            };
            return objInfos;
        }
        #endregion
    }
}