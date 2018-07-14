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

        public Transform headPos;

        private NavMeshAgent nmAgent;
        private AIAnimationController aiAnimController;
        private AIAnimEventReciever aiAnimEventReciever;

        // Stores various state of this AI
        [SerializeField] private AIState aiState;
        // Stores various stats of this AI
        [SerializeField] private AIStats aiStats;

        // FSM classes
        private AIFSM_FollowSchedule aiFSM_FollowSchedule = new AIFSM_FollowSchedule();
        private AIFSM_Schedule_ParticipateConvo aiFSM_ParticipateConvo = new AIFSM_Schedule_ParticipateConvo();
        private AIFSM_Civillian_UnderAttack aiFSM_Civillian_UnderAttack = new AIFSM_Civillian_UnderAttack();
        private AIFSM_HostileHuman aiFSM_HostileHuman = new AIFSM_HostileHuman();

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
            aiFSM_HostileHuman.InitializeFSM(this, transform, aiState, aiStats, aiAnimController);
        }

        private void Update()
        {
            UpdateActionableParameters();

            if (aiState.prepareEnterHostile && aiState.hostileHuman.schBeforeEnteringHostileMode < aiState.currSchedule)
            {
                aiState.prepareEnterHostile = false;
                aiState.aIMode = AIState.AIMode.HOSTILE;
            }

            if (!aiState.active)
                return;

            // if area under attack
            if (GameManager.instance.areaUnderAttack)
            {
                switch (aiStats.npcType)
                {
                    case AIStats.NPCType.VIP:
                        aiState.aIMode = AIState.AIMode.VIP_UNDER_ATTACK;
                        break;
                        
                    case AIStats.NPCType.CIVILLIAN:
                        aiState.aIMode = AIState.AIMode.CIVILIAN_UNDER_ATTACK;
                        break;
                }
            }

            switch (aiState.aIMode)
            {
                case AIState.AIMode.FOLLOW_SCHEDULE:
                    aiFSM_FollowSchedule.FSM_Update();
                    break;
                case AIState.AIMode.HOSTILE:
                    aiFSM_HostileHuman.FSM_Update();
                    break;
                case AIState.AIMode.CIVILIAN_UNDER_ATTACK:
                    aiFSM_Civillian_UnderAttack.FSM_Update();
                    break;
                case AIState.AIMode.PARTICIPATE_CONVO:
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
                aiState.aIMode = AIState.AIMode.PARTICIPATE_CONVO;
                aiState.waitingForConversationToStart = true;
                StopMovement();
                aiState.currConvoNPCTarget = requester;
                AddAction("End Conversation (Next)");
                return true;
            }
            else
            {
                Debug.Log(gameObject.name + " denied conversation request");
                return false;
            }
        }

        public void SetAction_End_RecivingConvo()
        {
            EndConvoWithConvoNPCTarget();
        }

        public void StartConvoWithConvoNPCTarget()
        {
            aiState.inConversation = true;
            aiAnimController.Anim_StartStandTalking();
        }
        
        public void EndConvoWithConvoNPCTarget()
        {
            aiState.inConversation = false;
            aiState.aIMode = AIState.AIMode.FOLLOW_SCHEDULE;
            aiState.currConvoNPCTarget = null;
            aiAnimController.Anim_StopStandTalking();
            RemoveAction("End Conversation (Next)");
            aiState.timeInConvo = 0;
        }
        
        public bool AvailableForConversation()
        {
            return !aiState.seated && aiState.active && !aiState.inConversation;
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
            if (aiState.seated)
            {
                aiAnimController.Anim_Stand();
            }

            if (aiAnimEventReciever.standing_Completed || !aiState.seated)
            {
                if (aiState.currSeatTarget)
                {
                    aiState.currSeatTarget.GetComponent<SeatMarker>().SetSeatAvailability(true);
                    aiState.currSeatTarget = null;
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

        public bool InLOS(Vector3 source, Vector3 target, string targetGameObjectName)
        {
            RaycastHit hitInfo;
            Ray ray = new Ray(source, target);

            int layerMask = ~(
                1 << LayerMask.NameToLayer("FloatingUI") |
                1 << LayerMask.NameToLayer("UI") |
                1 << LayerMask.NameToLayer("AreaMarker") |
                1 << LayerMask.NameToLayer("Marker"));

            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, layerMask))
            {
                if (hitInfo.transform.name == targetGameObjectName)
                {
                    Debug.Log("Source is in LOS with " + targetGameObjectName);
                    return true;
                }
            }
            return false;
        }

        public bool DrawGun()
        {
            aiAnimController.Anim_DrawGun();
            if (aiAnimEventReciever.gunDraw_Completed)
                aiAnimController.Anim_DrawGunEnd();

            return aiAnimEventReciever.gunDraw_Completed;
        }
                 
        public void FadeAway()
        {
            Destroy(this.gameObject);
        }

        public void AISetActive()
        {
            aiState.active = true;
        }

        public AIStats.NPCType GetNPCType()
        {
            return aiStats.npcType;
        }
        
        #region Interface methods
        public List<string> GetActions()
        {
            return actionableParameters;
        }

        public void SetAction(string action)
        {
            Debug.Log("Setting action: " + action);

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
                    if (npcSchedules[aiState.currSchedule].scheduleType == NPCSchedule.SCHEDULE_TYPE.MOVE_TO_WAYPT)
                        aiFSM_FollowSchedule.SetAction_End_MoveToWaypoint();
                    else if (npcSchedules[aiState.currSchedule].scheduleType == NPCSchedule.SCHEDULE_TYPE.MOVE_TO_WAYPT_ROT)
                        aiFSM_FollowSchedule.SetAction_End_MoveToWaypointAndRotate();
                    break;

                case "End Conversation (Next)":
                    if (npcSchedules[aiState.currSchedule].scheduleType == NPCSchedule.SCHEDULE_TYPE.TALK_TO_OTHER_NPC)
                        aiFSM_FollowSchedule.SetAction_End_TalkToOtherNPC();
                    else if (npcSchedules[aiState.currSchedule].scheduleType == NPCSchedule.SCHEDULE_TYPE.TALK_TO_OTHER_NPC)
                        SetAction_End_RecivingConvo();
                    break;

                case "Enter Hostile Mode":
                    if(actionableParameters.Exists(x => x == "Dismiss from Seat (Next)"))
                        SetAction("Dismiss from Seat (Next)");

                    else if (actionableParameters.Exists(x => x == "End Idle (Next)"))
                        SetAction("End Idle (Next)");

                    else if (actionableParameters.Exists(x => x == "Skip Waypoint (Next)"))
                        SetAction("Skip Waypoint (Next)");

                    else if (actionableParameters.Exists(x => x == "End Conversation (Next)"))
                        SetAction("End Conversation (Next)");

                    if(aiState.currSchedule > npcSchedules.Count - 1)
                        aiState.aIMode = AIState.AIMode.HOSTILE;
                    else
                    {
                        aiState.hostileHuman.schBeforeEnteringHostileMode = aiState.currSchedule;
                        aiState.prepareEnterHostile = true;
                    }
                    break;

                case "Shoot VIP":
                    aiFSM_HostileHuman.SetAction_SwitchToShootVIP();
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
            //if (aiState.active && !actionableParameters.Contains("Fade Away(Debug)"))
            //{
            //    actionableParameters.Add("Fade Away(Debug)");
            //}

            if (aiStats.npcType == AIStats.NPCType.TERRORIST && !actionableParameters.Contains("Enter Hostile Mode") && aiState.aIMode != AIState.AIMode.HOSTILE)
                actionableParameters.Add("Enter Hostile Mode");

            if (aiState.aIMode == AIState.AIMode.HOSTILE && !aiState.prepareEnterHostile && actionableParameters.Contains("Enter Hostile Mode"))
                actionableParameters.Remove("Enter Hostile Mode");

            if (aiState.active && actionableParameters.Contains("Activate NPC"))
                actionableParameters.Remove("Activate NPC");

            if (!aiState.active && !actionableParameters.Contains("Activate NPC"))
            {
                actionableParameters.Clear();
                actionableParameters.Add("Activate NPC");
            }

            if(aiState.aIMode == AIState.AIMode.HOSTILE &&
                (aiState.hostileHuman.currState == AIState.HostileHuman.State.IDLE ||
                 aiState.hostileHuman.currState == AIState.HostileHuman.State.MOVE_TO_WAYPOINT))
            {
                if (!actionableParameters.Contains("Shoot VIP"))
                    actionableParameters.Add("Shoot VIP");

                if (!actionableParameters.Contains("Shoot Player"))
                    actionableParameters.Add("Shoot Player");
            }
            else
            {
                if (actionableParameters.Contains("Shoot VIP"))
                    actionableParameters.Remove("Shoot VIP");

                if (actionableParameters.Contains("Shoot Player"))
                    actionableParameters.Remove("Shoot Player");
            }
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
            switch (aiState.aIMode)
            {
                case AIState.AIMode.FOLLOW_SCHEDULE:
                    objInfo1.content.Add("Mode: Following Schedule");
                    break;
                case AIState.AIMode.HOSTILE:
                    objInfo1.content.Add("Mode: Hostile");
                    break;
                case AIState.AIMode.CIVILIAN_UNDER_ATTACK:
                    objInfo1.content.Add("Mode: Civillian Under Attack");
                    break;
                case AIState.AIMode.VIP_UNDER_ATTACK:
                    objInfo1.content.Add("Mode: VIP Under Attack");
                    break;
                case AIState.AIMode.PARTICIPATE_CONVO:
                    objInfo1.content.Add("Mode: Talking to other NPC");
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

            if(aiState.aIMode == AIState.AIMode.FOLLOW_SCHEDULE)
                objInfo2.contentIndexToHighlight = aiState.currSchedule;
            else
                objInfo2.contentIndexToHighlight = -1;

            foreach (NPCSchedule schedule in npcSchedules)
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
                objInfo2,
            };
            return objInfos;
        }
        #endregion
    }
}