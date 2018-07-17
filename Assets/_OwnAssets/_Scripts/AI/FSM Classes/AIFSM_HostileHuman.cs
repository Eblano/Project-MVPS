using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    public class AIFSM_HostileHuman : AIFSM_Base
    {
        public void FSM_Update()
        {
            switch (aiState.hostileHuman.currState)
            {
                case AIState.HostileHuman.State.IDLE:
                    Process_Idle();
                    break;
                case AIState.HostileHuman.State.SHOOT_TARGET:
                    Process_ShootTarget();
                    break;
                case AIState.HostileHuman.State.KNIFE_TARGET:
                    Process_KnifeTarget();
                    break;
                case AIState.HostileHuman.State.MOVE_TO_WAYPOINT:
                    Process_MoveToWaypoint();
                    break;
            }
        }
        
        public void SetAction_SwitchToShootVIP()
        {
            aiState.hostileHuman.shootTarget = GameManager.instance.GetFirstVIPTransform();
            aiState.hostileHuman.currState = AIState.HostileHuman.State.SHOOT_TARGET;
            aiState.hostileHuman.currShootTargetState = AIState.HostileHuman.ShootTargetState.SPAWN_GUN;
        }

        public void SetAction_MoveToWaypoint(string waypointName)
        {
            if (aiState.hostileHuman.currShootTargetState == AIState.HostileHuman.ShootTargetState.SHOOT_TARGET)
                aiController.LowerGun();

            aiState.hostileHuman.waypointPos = GameManager.instance.GetWaypointMarkerPosition(waypointName);
            aiState.hostileHuman.currShootTargetState = AIState.HostileHuman.ShootTargetState.SPAWN_GUN;
            SetState_MoveToWaypoint();
        }

        private void Process_Idle()
        {
            aiController.StopMovement();
        }

        private void Process_ShootTarget()
        {
            switch(aiState.hostileHuman.currShootTargetState)
            {
                case AIState.HostileHuman.ShootTargetState.INACTIVE:
                    break;
                case AIState.HostileHuman.ShootTargetState.SPAWN_GUN:
                    SpawnGun();
                    break;
                case AIState.HostileHuman.ShootTargetState.DRAW_GUN:
                    DrawGun();
                    break;
                case AIState.HostileHuman.ShootTargetState.MOVE_TO_SHOOT_TARGET:
                    MoveTowardsShootTarget();
                    break;
                case AIState.HostileHuman.ShootTargetState.TRACK_TARGET:
                    TrackTarget();
                    break;
                case AIState.HostileHuman.ShootTargetState.AIM_GUN_ON_TARGET:
                    AimGunOnTarget();
                    break;
                case AIState.HostileHuman.ShootTargetState.SHOOT_TARGET:
                    ShootTarget();
                    break;
            }
        }

        private void Process_MoveToWaypoint()
        {
            if (!aiController.ReachedDestination(aiState.hostileHuman.waypointPos, 0))
            {
                aiController.SetNMAgentDestination(aiState.hostileHuman.waypointPos);
                aiController.MoveAITowardsNMAgentDestination(aiStats.runningSpeed);
            }
            else
            {
                aiController.StopMovement();
                aiController.SetNMAgentDestination(aiState.hostileHuman.waypointPos);
            }
        }

        private void Process_KnifeTarget()
        {

        }

        private void SetState_MoveToWaypoint()
        {
            aiState.hostileHuman.currState = AIState.HostileHuman.State.MOVE_TO_WAYPOINT;
        }

        private void SetState_MoveToShootTarget(Vector3 target)
        {
            aiState.hostileHuman.currState = AIState.HostileHuman.State.SHOOT_TARGET;
            aiState.hostileHuman.currShootTargetState = AIState.HostileHuman.ShootTargetState.MOVE_TO_SHOOT_TARGET;
            Debug.Log("Switch to MOVE_TO_SHOOT_TARGET");
            aiController.SetNMAgentDestination(target);
            aiController.MoveAITowardsNMAgentDestination(aiStats.runningSpeed);
        }

        private void SetState_ShootTarget()
        {
            aiState.hostileHuman.currShootTargetState = AIState.HostileHuman.ShootTargetState.SHOOT_TARGET;
            Debug.Log("Switch to SHOOT_TARGET");
            aiController.StopMovement();
        }

        private void SpawnGun()
        {
            if (!aiController.ref_pistol)
            {
                aiController.SpawnGunOnHand();
                aiState.hostileHuman.currShootTargetState = AIState.HostileHuman.ShootTargetState.DRAW_GUN;
            }
            else
                SetState_MoveToShootTarget(aiState.hostileHuman.shootTarget.position);
        }
        
        private void DrawGun()
        {
            bool finished = aiController.DrawGun();

            if (finished)
            {
                SetState_MoveToShootTarget(aiState.hostileHuman.shootTarget.position);
            }
        }
        
        private void MoveTowardsShootTarget()
        {
            if (!aiController.ReachedDestination(aiState.hostileHuman.shootTarget.position, aiStats.maxGunRange) &&
                !aiController.InLOS3PT(aiState.hostileHuman.shootTarget.position, aiState.hostileHuman.shootTarget.name)
                )
            {
                aiController.SetNMAgentDestination(aiState.hostileHuman.shootTarget.position);
                aiController.MoveAITowardsNMAgentDestination(aiStats.runningSpeed);
            }
            else
            {
                aiController.StopMovement();
                aiState.hostileHuman.currShootTargetState = AIState.HostileHuman.ShootTargetState.TRACK_TARGET;
                Debug.Log("Switch to TRACK_TARGET");
            }
        }
        
        private void TrackTarget()
        {
            aiController.RotateTowardsTargetDirection(aiState.hostileHuman.shootTarget.position);

            if (!aiController.InLOS3PT(aiState.hostileHuman.shootTarget.position, aiState.hostileHuman.shootTarget.name))
            {
                aiState.hostileHuman.currShootTargetState = AIState.HostileHuman.ShootTargetState.AIM_GUN_ON_TARGET;
                Debug.Log("Switch to AIM_GUN_ON_TARGET");
                return;
            }

            if (aiController.LookingAtTarget(aiState.hostileHuman.shootTarget.position, aiStats.lookAngleMarginOfError))
                SetState_ShootTarget();
        }

        private void AimGunOnTarget()
        {
            aiController.AimGun();
            SetState_MoveToShootTarget(aiState.hostileHuman.shootTarget.position);
        }

        private void ShootTarget()
        {
            if (!aiController.LookingAtTarget(aiState.hostileHuman.shootTarget.position, aiStats.shootTargetDir_AngleMarginOfError))
            {
                aiController.LowerGun();
                aiState.hostileHuman.currShootTargetState = AIState.HostileHuman.ShootTargetState.TRACK_TARGET;
                Debug.Log("Switch to TRACK_TARGET");
                return;
            }

            if (!aiController.InLOS3PT(aiState.hostileHuman.shootTarget.position, aiState.hostileHuman.shootTarget.name))
            {
                SetState_MoveToShootTarget(aiState.hostileHuman.shootTarget.position);
                aiController.ResetGunTransformToOrig();
                aiController.LowerGun();
                return;
            }

            // Pistol look at target
            aiController.ref_pistol.gameObject.transform.LookAt(aiState.hostileHuman.shootTarget);
            // Shoot
            aiController.ref_pistol.FireGun();
        }
    }
}
