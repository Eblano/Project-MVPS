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
    public class AIController : MonoBehaviour, IActions
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

        // Schedules this NPC has
        private List<NPCSchedule> npcSchedules;

        [SerializeField] private List<string> actionableParameters = new List<string>();
        [SerializeField] private Transform highestPoint;

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
            if (!aiState.general.seated && aiState.active)
            {
                aiState.general.aIMode = AIState.General.AIMode.PARTICIPATE_CONVO;
                aiState.general.waitingForConversationToStart = true;
                aiAnimController.Anim_Move(Vector3.zero, 1);
                nmAgent.SetDestination(transform.position);
                aiState.general.currConvoNPCTarget = conversationNPC;
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public void TalkToOtherNPC_Setup()
        {
            // Get gameobject of nearest NPC
            GameObject otherNPC = GameManager.instance.GetNearestCivilianNPC(gameObject);
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
        
        // NEW METHODS THAT IS SHARED, PLEASE ORGANIZE LATER
        public void SetNMAgentDestination(Vector3 position)
        {
            nmAgent.SetDestination(position);
        }

        public bool ReachedDestination(Vector3 destination, float extraStoppingDistance)
        {
            //return Vector3.Distance(transform.position, destination) < aiStats.stopDist + extraStoppingDistance;
            return nmAgent.remainingDistance < aiStats.stopDist + extraStoppingDistance;
        }

        public void MoveAITowardsNMAgentDestination()
        {
            aiAnimController.Anim_Move(nmAgent.desiredVelocity, 1);
        }

        public bool RotateTowardsTargetRotation(Quaternion targetRotation, bool reversedDirection)
        {
            if (reversedDirection)
                targetRotation = Quaternion.Inverse(targetRotation);

            StopMovement();
            aiAnimController.Anim_Turn(targetRotation);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * aiStats.turningSpeed);

            return Quaternion.Angle(transform.rotation, targetRotation) < aiStats.minAngleToFaceTarget;
        }

        public void StopMovement()
        {
            SetNMAgentDestination(transform.position);
            aiAnimController.Anim_Move(Vector3.zero, 1);
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
        //************************************************
                 
        public void FadeAway()
        {
            gameObject.SetActive(false);
        }

        public void AISetActive()
        {
            aiState.active = true;
        }
        
        #region IActions methods & Actionable related Methods
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
            }
        }

        public string GetName()
        {
            return npcName;
        }

        public Vector3 GetHighestPoint()
        {
            return highestPoint.position;
        }

        private void UpdateActionableParameters()
        {
            if (!aiState.active && !actionableParameters.Contains("Activate NPC"))
                actionableParameters.Add("Activate NPC");

            if(aiState.active && !actionableParameters.Contains("Fade Away(Debug)"))
                actionableParameters.Add("Fade Away(Debug)");
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
        #endregion
    }
}