using System.Collections.Generic;
using UnityEngine;
using ProtoBuf;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Networking;

/// <summary>
/// Manages AI thought process
/// </summary>
namespace SealTeam4
{
    public class AIController : MonoBehaviour, IActions, IObjectInfo, IDamageable
    {
        [System.Serializable]
        private class HitBoxColliders
        {
            public List<Collider> headColliders;
            public List<Collider> bodyColliders;
            public List<Collider> HandColliders;
            public List<Collider> legColliders;
        }

        [System.Serializable]
        public class TransformOffset
        {
            public Vector3 posOffset = Vector3.zero;
            public Vector3 rotOffset = Vector3.zero;
            public float scale = 1;
        }

        [System.Serializable]
        public class OutfitMaterials
        {
            public Material body;
            public Material bottoms;
            public Material eyes;
            public Material hair;
            public Material shoes;
            public Material tops;
        }
        
        [Header("Body Parts Renderers")]
        [SerializeField] private Renderer bodyRenderer;
        [SerializeField] private Renderer bottomsRenderer;
        [SerializeField] private Renderer eyesRenderer;
        [SerializeField] private Renderer hairRenderer;
        [SerializeField] private Renderer shoesRenderer;
        [SerializeField] private Renderer topsRenderer;

        [SerializeField] private OutfitMaterials outfitType_1;
        [SerializeField] private OutfitMaterials outfitType_2;
        [SerializeField] private OutfitMaterials outfitType_3;

        private string npcName;

        [Header("Body Parts")]
        public Transform headT;
        public Transform rightHandT;
        public Transform centerMassT;

        [Header("Weapon/Pistol")]
        [SerializeField] private GameObject pistol_Prefab;
        public TransformOffset pistol_NormalOffset;
        public TransformOffset pistol_HoldingGunOffset;
        private NPCGun ref_pistol;

        [Header("Weapon/Knife")]
        [SerializeField] private GameObject knife_Prefab;
        public TransformOffset knife_TOffset;
        private GameObject ref_knife;

        private NavMeshAgent nmAgent;
        private AIAnimationController aiAnimController;
        private AIAnimEventReciever aiAnimEventReciever;

        [Space(10)]

        // Stores various state of this AI
        [SerializeField] private AIState aiState;

        [Space(10)]

        // Stores various stats of this AI
        [SerializeField] private AIStats aiStats;

        // FSM classes
        private AIFSM_FollowSchedule aiFSM_FollowSchedule = new AIFSM_FollowSchedule();
        private AIFSM_Schedule_ParticipateConvo aiFSM_ParticipateConvo = new AIFSM_Schedule_ParticipateConvo();
        private AIFSM_Civillian_UnderAttack aiFSM_Civillian_UnderAttack = new AIFSM_Civillian_UnderAttack();
        private AIFSM_HostileHuman aiFSM_HostileHuman = new AIFSM_HostileHuman();
        private AIFSM_VIP_UnderAttack aiFSM_VIP_UnderAttack = new AIFSM_VIP_UnderAttack();

        // Schedules this NPC has
        private List<NPCSchedule> npcSchedules;

        [SerializeField] private List<string> actionableParameters = new List<string>();

        // Exposed Variables for InterfaceManager
        [Header("Exposed for InterfaceManager")]
        public Transform highestPoint;
        public Collider col;

        [Space(10)]
        [SerializeField] private HitBoxColliders hitBoxColliders;

        public void Setup(string npcName, NpcSpawnData.NPCOutfit outfit, AIStats aiStats, List<NPCSchedule> npcSchedules)
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
            aiFSM_VIP_UnderAttack.InitializeFSM(this, transform, aiState, aiStats, aiAnimController);

            SetupOutfit(outfit);
        }

        private void SetupOutfit(NpcSpawnData.NPCOutfit outfit)
        {
            if(!bodyRenderer || !bottomsRenderer || !eyesRenderer || !hairRenderer || !shoesRenderer || !topsRenderer)
            {
                Debug.Log("One or more renderers are missing, failed to set outfit");
                return;
            }

            switch (outfit)
            {
                case NpcSpawnData.NPCOutfit.TYPE_1:
                    bodyRenderer.material = outfitType_1.body;
                    bottomsRenderer.material = outfitType_1.bottoms;
                    eyesRenderer.material = outfitType_1.eyes;
                    hairRenderer.material = outfitType_1.hair;
                    shoesRenderer.material = outfitType_1.shoes;
                    topsRenderer.material = outfitType_1.tops;
                    break;
                case NpcSpawnData.NPCOutfit.TYPE_2:
                    bodyRenderer.material = outfitType_2.body;
                    bottomsRenderer.material = outfitType_2.bottoms;
                    eyesRenderer.material = outfitType_2.eyes;
                    hairRenderer.material = outfitType_2.hair;
                    shoesRenderer.material = outfitType_2.shoes;
                    topsRenderer.material = outfitType_2.tops;
                    break;
                case NpcSpawnData.NPCOutfit.TYPE_3:
                    bodyRenderer.material = outfitType_3.body;
                    bottomsRenderer.material = outfitType_3.bottoms;
                    eyesRenderer.material = outfitType_3.eyes;
                    hairRenderer.material = outfitType_3.hair;
                    shoesRenderer.material = outfitType_3.shoes;
                    topsRenderer.material = outfitType_3.tops;
                    break;
            }
        }

        private void EnterHostileMode()
        {
            aiState.prepareEnterHostile = false;
            aiState.aIMode = AIState.AIMode.HOSTILE;
        }

        private void Update()
        {
            UpdateActionableParameters();

            if (!aiState.active || !aiState.alive)
                return;

            CheckHPStatus();

            if (aiState.prepareEnterHostile && aiState.hostileHuman.schBeforeEnteringHostileMode < aiState.currSchedule)
                EnterHostileMode();

            switch (aiState.aIMode)
            {
                case AIState.AIMode.FOLLOW_SCHEDULE:
                    aiFSM_FollowSchedule.FSM_Update();
                    break;
                case AIState.AIMode.VIP_UNDER_ATTACK:
                    aiFSM_VIP_UnderAttack.FSM_Update();
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

        public void TriggerUnderThreatMode()
        {
            if (aiStats.npcType == AIStats.NPCType.CIVILLIAN)
                aiState.aIMode = AIState.AIMode.CIVILIAN_UNDER_ATTACK;
            else if (aiStats.npcType == AIStats.NPCType.VIP)
                aiState.aIMode = AIState.AIMode.VIP_UNDER_ATTACK;
        }

        public bool RequestStartConvo(AIController requester)
        {
            if (AvailableForConversation())
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
            if(aiState.currConvoNPCTarget)
                aiState.currConvoNPCTarget.SetAction("End Conversation (Next)");
            
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

        public bool WithinDistance(Vector3 destination, float stopDist)
        {
            Vector3 origin = transform.position;
            origin.y = 0;
            destination.y = 0;
            
            return Vector3.Distance(origin, destination) < stopDist;
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

            return Quaternion.Angle(transform.rotation, targetRotation) < aiStats.lookAngleMarginOfError;
        }

        public void RotateTowardsTargetDirection(Vector3 targetPosition)
        {
            StopMovement();

            Vector3 direction = targetPosition - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            lookRotation = Quaternion.Euler(0, lookRotation.eulerAngles.y, 0);
            //aiAnimController.Anim_Turn(lookRotation);

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * aiStats.turningSpeed);
        }

        public bool LookingAtTarget(Vector3 targetPosition, float angleOfError)
        {
            Vector3 forwardLook = headT.forward.normalized;
            Vector3 targetLook = (targetPosition - headT.position).normalized;

            forwardLook.y = 0;
            targetLook.y = 0;

            Debug.DrawLine(headT.position, headT.position + forwardLook * 7, Color.blue);
            Debug.DrawLine(headT.position, headT.position + targetLook * 7, Color.cyan);
            
            return Vector3.Angle(forwardLook, targetLook) < angleOfError;
        }

        public bool KnifeSpawned()
        {
            return ref_knife;
        }

        public bool GunSpawned()
        {
            return ref_pistol;
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
                aiState.seated = false;
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

        private void CheckHPStatus()
        {
            if(aiStats.GetTotalHP() == 0)
            {
                aiAnimController.Anim_Die();
                aiState.alive = false;
                aiState.active = false;
                Destroy(centerMassT.gameObject);
                StopMovement();
            }
        }

        public bool InLOS(Vector3 target, string targetGameObjectName)
        {
            RaycastHit centerRayHitInfo;

            bool centerRayPassed = false;

            Ray rayCenter = new Ray(headT.position, target - headT.position);

            int layerMask = ~(
                1 << LayerMask.NameToLayer("FloatingUI") |
                1 << LayerMask.NameToLayer("UI") |
                1 << LayerMask.NameToLayer("AreaMarker") |
                1 << LayerMask.NameToLayer("Marker"));

            if (Physics.Raycast(rayCenter, out centerRayHitInfo, Mathf.Infinity, layerMask))
            {
                if (centerRayHitInfo.transform.name == targetGameObjectName)
                {
                    centerRayPassed = true;
                }
            }

            Debug.DrawRay(headT.position, target - headT.position);

            return centerRayPassed;
        }

        public bool InLOS3PT(Vector3 target, string targetGameObjectName)
        {
            RaycastHit centerRayHitInfo;
            RaycastHit leftRayHitInfo;
            RaycastHit rightRayHitInfo;

            bool centerRayPassed = false;
            bool leftRayPassed = false;
            bool rightRayPassed = false;

            Ray rayCenter = new Ray(headT.position, target - headT.position);

            int layerMask = ~(
                1 << LayerMask.NameToLayer("FloatingUI") |
                1 << LayerMask.NameToLayer("UI") |
                1 << LayerMask.NameToLayer("AreaMarker") |
                1 << LayerMask.NameToLayer("Marker"));

            if (Physics.Raycast(rayCenter, out centerRayHitInfo, Mathf.Infinity, layerMask))
            {
                if (centerRayHitInfo.transform.name == targetGameObjectName)
                {
                    centerRayPassed = true;
                }
            }

            Vector3 headLeftPos = (Quaternion.AngleAxis(-90, Vector3.up) * (target - headT.position).normalized * aiStats.losMarginSize) + headT.position;
            Vector3 headRightPos = (Quaternion.AngleAxis(90, Vector3.up) * (target - headT.position).normalized * aiStats.losMarginSize) + headT.position;

            Ray rayLeft = new Ray(headLeftPos, target - headLeftPos);
            Ray rayRight = new Ray(headRightPos, target - headRightPos);

            if (Physics.Raycast(rayLeft, out leftRayHitInfo, centerRayHitInfo.distance, layerMask))
            {
                if (leftRayHitInfo.transform.name == targetGameObjectName)
                    leftRayPassed = true;
            }
            else
                leftRayPassed = true;


            if (Physics.Raycast(rayRight, out rightRayHitInfo, centerRayHitInfo.distance, layerMask))
            {
                if (rightRayHitInfo.transform.name == targetGameObjectName)
                    rightRayPassed = true;
            }
            else
                rightRayPassed = true;

            Debug.DrawRay(headT.position, target - headT.position);
            Debug.DrawRay(headLeftPos, target - headLeftPos);
            Debug.DrawRay(headRightPos, target - headRightPos);

            return centerRayPassed && leftRayPassed && rightRayPassed;
        }

        public void DrawWeapon()
        {
            aiAnimController.Anim_DrawGun();
        }

        public void GunLookAtTarget(Transform target)
        {
            ref_pistol.transform.LookAt(target);
        }

        public void AimGun()
        {
            aiAnimController.Anim_AimGun();
        }

        public void FireGun()
        {
            ref_pistol.FireGun(aiStats.gunAccuracy);
        }

        public void SwingKnife()
        {
            aiAnimController.Anim_SwingKnife();
        }

        public void LowerGun()
        {
            aiAnimController.Anim_LowerGun();
        }

        public void SetGunTransformOffset(TransformOffset offset)
        {
            ref_pistol.transform.localPosition = offset.posOffset;
            ref_pistol.transform.localRotation = Quaternion.Euler(offset.rotOffset);
            ref_pistol.transform.localScale = new Vector3(offset.scale, offset.scale, offset.scale);
        }

        public void SetKnifeTransformOffset(TransformOffset offset)
        {
            ref_knife.transform.localPosition = offset.posOffset;
            ref_knife.transform.localRotation = Quaternion.Euler(offset.rotOffset);
            ref_knife.transform.localScale = new Vector3(offset.scale, offset.scale, offset.scale);
        }

        public void SpawnGunOnHand()
        {
            if (!ref_pistol)
            {
                ref_pistol = Instantiate(pistol_Prefab, rightHandT.transform).GetComponent<NPCGun>();
                NetworkServer.Spawn(ref_pistol.gameObject);
            }
        }

        public void DespawnGun()
        {
            Destroy(ref_pistol.gameObject);
        }

        public void SpawnKnifeOnHand()
        {
            if (!ref_knife)
            {
                ref_knife = Instantiate(knife_Prefab, rightHandT.transform);
                NetworkServer.Spawn(ref_knife.gameObject);
            }
        }

        public void DespawnKnife()
        {
            Destroy(ref_knife.gameObject);
        }

        public void FadeAway()
        {
            Destroy(gameObject);
        }

        public void AISetActive()
        {
            aiState.active = true;
        }

        public AIStats.NPCType GetNPCType()
        {
            return aiStats.npcType;
        }

        public List<string> GetActions()
        {
            return actionableParameters;
        }

        public void SetAction(string action)
        {
            if (action.Contains("Move To "))
            {
                aiFSM_HostileHuman.SetAction_MoveToWaypoint(action.Substring(8));
            }

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
                    if (!aiState.inConversation)
                        return;

                    if (aiState.aIMode == AIState.AIMode.PARTICIPATE_CONVO)
                        SetAction_End_RecivingConvo();

                    if (npcSchedules.Count == 0)
                        return;

                    if (npcSchedules[aiState.currSchedule].scheduleType == NPCSchedule.SCHEDULE_TYPE.TALK_TO_OTHER_NPC)
                    aiFSM_FollowSchedule.SetAction_End_TalkToOtherNPC();
                    break;

                case "Enter Hostile Mode":
                    if (actionableParameters.Exists(x => x == "Dismiss from Seat (Next)"))
                        SetAction("Dismiss from Seat (Next)");

                    else if (actionableParameters.Exists(x => x == "End Idle (Next)"))
                        SetAction("End Idle (Next)");

                    else if (actionableParameters.Exists(x => x == "Skip Waypoint (Next)"))
                        SetAction("Skip Waypoint (Next)");

                    else if (actionableParameters.Exists(x => x == "End Conversation (Next)"))
                        SetAction("End Conversation (Next)");

                    if (aiState.currSchedule > npcSchedules.Count - 1)
                        EnterHostileMode();
                    else
                    {
                        aiState.hostileHuman.schBeforeEnteringHostileMode = aiState.currSchedule;
                        aiState.prepareEnterHostile = true;
                    }
                    break;

                case "Shoot VIP":
                    aiFSM_HostileHuman.SetAction_SwitchToShootVIP();
                    break;

                case "Knife VIP":
                    aiFSM_HostileHuman.SetAction_SwitchToKnifeVIP();
                    break;
            }
        }

        public string GetName()
        {
            return npcName;
        }

        public void SetGrabModeTransform(Transform grabSource)
        {
            if (grabSource)
                aiFSM_VIP_UnderAttack.SetProcess_GrabbedFollowPlayer(grabSource);
            else
                aiFSM_VIP_UnderAttack.SetProcess_FollowPlayer();
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
            if (actionableParameters.Exists(x => x == action))
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

            if (aiState.active && aiStats.npcType == AIStats.NPCType.TERRORIST && !actionableParameters.Contains("Enter Hostile Mode") && aiState.aIMode != AIState.AIMode.HOSTILE)
            {
                actionableParameters.Add("Enter Hostile Mode");
            }

            if (aiState.aIMode == AIState.AIMode.HOSTILE && !aiState.prepareEnterHostile && actionableParameters.Contains("Enter Hostile Mode"))
            {
                actionableParameters.Remove("Enter Hostile Mode");
            }

            if ((aiState.active && actionableParameters.Contains("Activate NPC")) || (!aiState.alive && actionableParameters.Contains("Activate NPC")))
                actionableParameters.Remove("Activate NPC");

            if (!aiState.active && !actionableParameters.Contains("Activate NPC") && aiState.alive)
            {
                actionableParameters.Clear();
                actionableParameters.Add("Activate NPC");
            }

            if (aiState.active && aiState.aIMode == AIState.AIMode.HOSTILE &&
                (aiState.hostileHuman.currState == AIState.HostileHuman.State.IDLE ||
                 aiState.hostileHuman.currState == AIState.HostileHuman.State.MOVE_TO_WAYPOINT))
            {
                if (!actionableParameters.Contains("Shoot VIP"))
                    actionableParameters.Add("Shoot VIP");

                if (!actionableParameters.Contains("Knife VIP"))
                    actionableParameters.Add("Knife VIP");

                foreach (string dynWPMarkerName in aiStats.allDynamicWaypoints)
                {
                    if (!actionableParameters.Contains("Move To " + dynWPMarkerName))
                        actionableParameters.Add("Move To " + dynWPMarkerName);
                }
            }
            else
            {
                if (aiState.hostileHuman.currState == AIState.HostileHuman.State.SHOOT_TARGET && actionableParameters.Contains("Shoot VIP"))
                    actionableParameters.Remove("Shoot VIP");

                if (aiState.hostileHuman.currState == AIState.HostileHuman.State.KNIFE_TARGET && actionableParameters.Contains("Knife VIP"))
                    actionableParameters.Remove("Knife VIP");

                if (aiState.hostileHuman.currState == AIState.HostileHuman.State.KNIFE_TARGET && !actionableParameters.Contains("Shoot VIP"))
                    actionableParameters.Add("Shoot VIP");

                if (aiState.hostileHuman.currState == AIState.HostileHuman.State.SHOOT_TARGET && !actionableParameters.Contains("Knife VIP"))
                    actionableParameters.Add("Knife VIP");

                foreach (string dynWPMarkerName in aiStats.allDynamicWaypoints)
                {
                    if (!actionableParameters.Contains("Move To " + dynWPMarkerName))
                        actionableParameters.Remove("Move To " + dynWPMarkerName);
                }
            }
        }

        public void TriggerUnderAttackState()
        {
            switch (aiStats.npcType)
            {
                case AIStats.NPCType.VIP:
                    if (aiState.aIMode != AIState.AIMode.VIP_UNDER_ATTACK)
                    {
                        aiState.aIMode = AIState.AIMode.VIP_UNDER_ATTACK;
                        aiFSM_VIP_UnderAttack.SetProcess_FollowPlayer();
                    }
                    break;

                case AIStats.NPCType.CIVILLIAN:
                    if (aiState.aIMode != AIState.AIMode.CIVILIAN_UNDER_ATTACK)
                    {
                        aiState.aIMode = AIState.AIMode.CIVILIAN_UNDER_ATTACK;
                    }
                    break;
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

            if (aiState.aIMode == AIState.AIMode.FOLLOW_SCHEDULE)
                objInfo2.contentIndexToHighlight = aiState.currSchedule;
            else
                objInfo2.contentIndexToHighlight = -1;

            foreach (NPCSchedule schedule in npcSchedules)
            {
                switch (schedule.scheduleType)
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
                        objInfo2.content.Add("Talk to " + schedule.argument_1 + " for " + schedule.argument_2 + "s");
                        break;
                    default:
                        objInfo2.content.Add("???");
                        break;
                }
            }

            ObjectInfo objInfo3 = new ObjectInfo();
            objInfo3.title = "Health";
            objInfo3.content.Add("Current HP: " + aiStats.GetTotalHP());

            List<ObjectInfo> objInfos = new List<ObjectInfo>
            {
                objInfo1,
                objInfo2,
                objInfo3
            };
            return objInfos;
        }

        public void OnHit(Collider c, GlobalEnums.WeaponType weaponType)
        {
            if (!aiState.alive)
                return;

            aiAnimController.Anim_Flinch();


            if (aiState.invincible)
                return;

            foreach (Collider bodyColl in hitBoxColliders.bodyColliders)
            {
                if (c == bodyColl)
                {
                    if(weaponType == GlobalEnums.WeaponType.PISTOL)
                        aiStats.TakeDamage(aiStats.bulletBodyDmg);
                    else if(weaponType == GlobalEnums.WeaponType.KNIFE)
                        aiStats.TakeDamage(aiStats.knifeBodyDmg);
                    return;
                }
            }

            foreach (Collider headColl in hitBoxColliders.headColliders)
            {
                if (c == headColl)
                {
                    if (weaponType == GlobalEnums.WeaponType.PISTOL)
                        aiStats.TakeDamage(aiStats.bulletHeadDmg);
                    else if (weaponType == GlobalEnums.WeaponType.KNIFE)
                        aiStats.TakeDamage(aiStats.bulletHeadDmg);
                    return;
                }
            }

            foreach (Collider handColl in hitBoxColliders.HandColliders)
            {
                if (c == handColl)
                {
                    if (weaponType == GlobalEnums.WeaponType.PISTOL)
                        aiStats.TakeDamage(aiStats.bulletHandDmg);
                    else if (weaponType == GlobalEnums.WeaponType.KNIFE)
                        aiStats.TakeDamage(aiStats.knifeLegDmg);
                    return;
                }
            }

            foreach (Collider legColl in hitBoxColliders.legColliders)
            {
                if (c == legColl)
                {
                    if (weaponType == GlobalEnums.WeaponType.PISTOL)
                        aiStats.TakeDamage(aiStats.bulletLegDmg);
                    else if (weaponType == GlobalEnums.WeaponType.KNIFE)
                        aiStats.TakeDamage(aiStats.knifeLegDmg);
                    return;
                }
            }
        }

        public bool IsTalking()
        {
            return aiState.inConversation;
        }

        public bool IsInDistress()
        {
            return aiState.aIMode == AIState.AIMode.CIVILIAN_UNDER_ATTACK;
        }
    }
}