using System.Collections.Generic;
using UnityEngine;
using ProtoBuf;
using UnityEngine.AI;

/// <summary>
/// Manages AI thought process
/// </summary>
namespace SealTeam4
{
    public class AIController : MonoBehaviour, IActions
    {
        private NavMeshAgent nmAgent;
        private AIAnimationController aiAnimController;
        private AIAnimEventReciever animEventReciever;

        // Stores various state of this AI
        AIState aiState = new AIState();
        // Stores various stats of this AI
        AIStats aiStats;

        // FSM classes
        AIFSM_FollowSchedule aiFSM_FollowSchedule = new AIFSM_FollowSchedule();
        AIFSM_Schedule_ParticipateConvo aiFSM_ParticipateConvo = new AIFSM_Schedule_ParticipateConvo();
        AIFSM_Civillian_UnderAttack aiFSM_Civillian_UnderAttack = new AIFSM_Civillian_UnderAttack();

        // Schedules this NPC has
        private List<NPCSchedule> npcSchedules;

        private List<string> actionableParameters = new List<string>();
        private string[] allActionParameters =
        { "Activate NPC", "Deactivate NPC"};

        private void Start()
        {
            nmAgent = GetComponent<NavMeshAgent>();
            aiAnimController = GetComponent<AIAnimationController>();
            animEventReciever = GetComponent<AIAnimEventReciever>();
            
            // Initializing FSM classes
            aiFSM_FollowSchedule.InitializeFSM(this, transform, aiState, aiStats, aiAnimController, npcSchedules);
            aiFSM_ParticipateConvo.InitializeFSM(this, transform, aiState, aiStats, aiAnimController, npcSchedules);
            aiFSM_Civillian_UnderAttack.InitializeFSM(this, transform, aiState, aiStats, aiAnimController, npcSchedules);
        }

        private void Update()
        {
            UpdateAIActionableParameters();

            if (!aiState.active)
                return;

            // if area under attack
            if(GameManager.instance.areaUnderAttack)
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
        
        public void ConvoProcess_TalkToOtherNPC()
        {
            AIController otherNPCAIController = aiState.general.currConvoNPCTarget.GetComponent<AIController>();

            if (!aiState.general.inConversation)
            {
                otherNPCAIController.ConvoProcess_StartConvo();
                aiState.general.inConversation = true;
            }
            if (aiState.general.timeInConvo > float.Parse(npcSchedules[aiState.general.currSchedule].argument))
            {
                otherNPCAIController.ConvoProcess_EndConvo();
                aiState.general.inConversation = false;
                aiState.general.timeInConvo = 0;
                aiAnimController.Anim_StopStandTalking();
                aiState.general.currSubschedule++;
            }
            else
            {
                aiAnimController.Anim_StartStandTalking();
                aiState.general.timeInConvo += Time.deltaTime;
            }
        }
        
        public void ConvoProcess_StartConvo()
        {
            aiState.general.inConversation = true;
            aiState.general.waitingForConversationToStart = false;
            nmAgent.SetDestination(transform.position);
            aiAnimController.Anim_StartStandTalking();
            aiState.general.timeInConvo = 0;
        }
        
        public void ConvoProcess_EndConvo()
        {
            aiState.general.inConversation = false;
            aiState.general.aIMode = AIState.General.AIMode.FOLLOW_SCHEDULE;
            aiState.general.currConvoNPCTarget = null;
            aiAnimController.Anim_StopStandTalking();
            aiState.general.timeInConvo = 0;
        }
        
        private bool ConvoProcess_ReqForConvo(GameObject conversationNPC)
        {
            if (!aiState.general.seated)
            {
                aiState.general.aIMode = AIState.General.AIMode.PARTICIPATE_CONVO;
                aiState.general.waitingForConversationToStart = true;
                aiAnimController.Anim_Move(Vector3.zero, false, 1);
                nmAgent.SetDestination(transform.position);
                aiState.general.currConvoNPCTarget = conversationNPC;
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public void MoveToPosition(Vector3 targetPos , float extraStoppingDistance)
        {
            nmAgent.SetDestination(targetPos);
            if (nmAgent.remainingDistance > nmAgent.stoppingDistance + extraStoppingDistance)
            {
                if(aiStats.enableCollisionAvoidance)
                {
                    targetPos = GetCollisionAvoidanceVector(targetPos);
                }

                aiAnimController.Anim_Move(nmAgent.desiredVelocity, false, 1);
            }
            else
            {
                aiState.general.currSubschedule++;
            }
        }
        
        public void MoveToWaypoint_ProcSetup()
        {
            aiState.general.currWaypointTarget = GetTargetMarkerPosition();
            nmAgent.SetDestination(aiState.general.currWaypointTarget.position);
            aiState.general.currSubschedule++;
        }

        public void MoveToWaypoint_ProcTerm()
        {
            StopNMAgentMovement();
            aiAnimController.Anim_Move(Vector3.zero, false, 1);
            aiState.general.currSubschedule++;
        }
        
        public void TalkToOtherNPC_Setup()
        {
            // Get gameobject of nearest NPC
            GameObject otherNPC = GameManager.instance.GetNearestCivilianNPC(transform, gameObject);
            if(otherNPC) // If found NPC
            {
                if(otherNPC.GetComponent<AIController>().ConvoProcess_ReqForConvo(gameObject))
                {
                    aiState.general.currConvoNPCTarget = otherNPC;
                    nmAgent.SetDestination(aiState.general.currConvoNPCTarget.transform.position);
                    aiState.general.currSubschedule++;
                }
            }
            else
            {
                Debug.Log("Failed to find available NPC");
                aiState.general.currSubschedule = -1;
            }
        }
        
        public void Idle()
        {
            if (aiState.general.currTimerValue <= 0)
            {
                aiState.general.currTimerValue -= Time.deltaTime;
            }
            else
            {
                aiState.general.currSubschedule++;
            }
        }

        public void Idle_Setup()
        {
            aiState.general.currTimerValue = float.Parse(npcSchedules[aiState.general.currSchedule].argument);
            aiState.general.currSubschedule++;
        }

        public void Idle_Term()
        {
            aiState.general.currSubschedule++;
        }
        
        public void LeaveIfSittingOnSeat()
        {
            if(aiState.general.seated)
            {
                aiAnimController.Anim_Stand();
            }

            if (animEventReciever.standing_Completed || !aiState.general.seated)
            {
                if(aiState.general.currSeatTarget)
                {
                    aiState.general.currSeatTarget.GetComponent<SeatMarker>().SetSeatAvailability(true);
                    aiState.general.currSeatTarget = null;
                }
                aiState.general.seated = false;
                aiState.general.currSubschedule++;
            }
        }

        public void RotateToTargetRotation(Transform targetRotation, bool reversedDirection)
        {
            if (!RotationIsInLine(targetRotation))
            {
                if(reversedDirection)
                    aiAnimController.Anim_Move(-targetRotation.forward, true, 1);
                else
                    aiAnimController.Anim_Move(targetRotation.forward, true, 1);
            }
            else
            {
                aiState.general.currSubschedule++;
            }
        }
        
        public void SitDownInArea_Setup()
        {
            // Get Area
            AreaMarker areaMarker = GameManager.instance.GetAreaMarkerByName(npcSchedules[aiState.general.currSchedule].argument);
            // Empty seat from selected Area
            aiState.general.currSeatTarget = areaMarker.GetRandomEmptySeat();

            if (aiState.general.currSeatTarget)
            {
                aiState.general.currSeatTarget.GetComponent<SeatMarker>().SetSeatAvailability(false);
                nmAgent.SetDestination(aiState.general.currSeatTarget.transform.position);
                aiState.general.currSubschedule++;
            }
            else
            {
                Debug.Log("No Seat Found in " + areaMarker.name);
                aiState.general.currSubschedule = -1;
            }
        }

        public void SitDownInArea_Term()
        {
            nmAgent.SetDestination(transform.position);
            aiAnimController.Anim_Move(Vector3.zero, false, 1);
            aiState.general.currSubschedule++;
        }
        
        public void SitDownOnSeat()
        {
            aiAnimController.Anim_Sit();

            if (animEventReciever.sitting_Completed)
            {
                aiState.general.seated = true;
                aiState.general.currSubschedule++;
            }
        }
        
        public void StopNMAgentMovement()
        {
            nmAgent.SetDestination(transform.position);
        }
        
        private Vector3 GetCollisionAvoidanceVector(Vector3 direction)
        {
            RaycastHit centerHitInfo;
            RaycastHit leftHitInfo;
            RaycastHit rightHitInfo;
            Ray centerRay = new Ray(transform.position, transform.forward);
            Ray leftRay = new Ray(transform.position, ((transform.forward - transform.right) / 1).normalized);
            Ray rightRay = new Ray(transform.position, ((transform.forward + transform.right) / 1).normalized);

            // Create 3 raycasts and calculate avoidance vector
            if(Physics.Raycast(centerRay, out centerHitInfo, aiStats.collisionAvoidanceRayLen, 1 << LayerMask.NameToLayer("NPC")))
            {
                Debug.Log("Collision Avoidance");
                return direction + (centerHitInfo.normal * aiStats.collisionAvoidanceMultiplyer);
            }
            if (Physics.Raycast(leftRay, out leftHitInfo, aiStats.collisionAvoidanceRayLen, 1 << LayerMask.NameToLayer("NPC")))
            {
                Debug.Log("Collision Avoidance");
                return direction + (leftHitInfo.normal * aiStats.collisionAvoidanceMultiplyer);
            }
            if (Physics.Raycast(rightRay, out rightHitInfo, aiStats.collisionAvoidanceRayLen, 1 << LayerMask.NameToLayer("NPC")))
            {
                Debug.Log("Collision Avoidance");
                return direction + (rightHitInfo.normal * aiStats.collisionAvoidanceMultiplyer);
            }
            return direction;
        }
        
        public bool RotationIsInLine(Transform t)
        {
            return Vector3.Angle(transform.forward, t.forward) < aiStats.minAngleToFaceTarget;
        }
        
        public Transform GetTargetMarkerPosition()
        {
            string targetName = npcSchedules[aiState.general.currSchedule].argument;
            return GameManager.instance.GetTargetMarkerTransform(targetName);
        }
        
        public void SetSchedule(List<NPCSchedule> npcSchedules)
        {
            this.npcSchedules = npcSchedules;
        }

        public void SetAIStats(AIStats aiStats)
        {
            this.aiStats = aiStats;
        }

        public Vector3 GetRandNavmeshPos(float radius)
        {
            Vector3 randomDirection = Random.insideUnitSphere * radius;
            randomDirection += transform.position;
            NavMeshHit hit;
            Vector3 finalPosition = Vector3.zero;
            if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
            {
                finalPosition = hit.position;
            }
            return finalPosition;
        }

        private void UpdateAIActionableParameters()
        {
            actionableParameters.Clear();

            if (!aiState.active)
                actionableParameters.Add("Activate NPC");
        }

        #region IActions Interace methods
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
            }
        }
        #endregion

        private void SetAction_ActivateNPC()
        {
            aiState.active = true;
            actionableParameters.Remove("Deactivate NPC");
        }
    }
}

