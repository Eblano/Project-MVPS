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

        //***************************
        #region SetAction Methods

        public void SetAction_SwitchToShootVIP()
        {
            aiState.hostileHuman.shootTarget = GameManager.instance.GetFirstVIPTransform();
            SetState_ShootTarget();
            SetState_ShootTarget_SpawnGun();
        }

        public void SetAction_MoveToWaypoint(string waypointName)
        {
            if (aiState.hostileHuman.currShootTargetState == AIState.HostileHuman.ShootTargetState.SHOOT)
            {
                aiController.ResetGunTransformToOrig();
                aiController.LowerGun();
            }

            aiState.hostileHuman.waypointPos = GameManager.instance.GetWaypointMarkerPosition(waypointName);
            SetState_MoveToWaypoint();
        }

        #endregion
        //***************************

        //***************************
        #region FSM State Switching Methods

        private void SetState_ShootTarget()
        {
            aiState.hostileHuman.currState = AIState.HostileHuman.State.SHOOT_TARGET;
        }

        private void SetState_MoveToWaypoint()
        {
            aiState.hostileHuman.currState = AIState.HostileHuman.State.MOVE_TO_WAYPOINT;
        }

        private void SetState_ShootTarget_MoveToShootTarget()
        {
            SetState_ShootTarget();
            aiState.hostileHuman.currShootTargetState = AIState.HostileHuman.ShootTargetState.MOVE_TO_SHOOT_TARGET;
            aiController.SetNMAgentDestination(aiState.hostileHuman.shootTarget.position);
            aiController.MoveAITowardsNMAgentDestination(aiStats.runningSpeed);
        }

        private void SetState_ShootTarget_Shoot()
        {
            aiState.hostileHuman.currShootTargetState = AIState.HostileHuman.ShootTargetState.SHOOT;
            aiController.StopMovement();
        }

        private void SetState_ShootTarget_DrawGun()
        {
            aiState.hostileHuman.currShootTargetState = AIState.HostileHuman.ShootTargetState.DRAW_GUN;
        }

        private void SetState_ShootTarget_TrackTarget()
        {
            aiController.StopMovement();
            aiState.hostileHuman.currShootTargetState = AIState.HostileHuman.ShootTargetState.TRACK_TARGET;
        }

        private void SetState_ShootTarget_AimGunOnTarget()
        {
            aiState.hostileHuman.currShootTargetState = AIState.HostileHuman.ShootTargetState.AIM_GUN_ON_TARGET;
        }

        private void SetState_ShootTarget_SpawnGun()
        {
            aiState.hostileHuman.currShootTargetState = AIState.HostileHuman.ShootTargetState.SPAWN_GUN;
        }
        #endregion
        //***************************

        //***************************
        #region FSM Methods

        private void Process_ShootTarget()
        {
            switch (aiState.hostileHuman.currShootTargetState)
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
                    MoveToShootTarget();
                    break;
                case AIState.HostileHuman.ShootTargetState.TRACK_TARGET:
                    TrackTarget();
                    break;
                case AIState.HostileHuman.ShootTargetState.AIM_GUN_ON_TARGET:
                    AimGunOnTarget();
                    break;
                case AIState.HostileHuman.ShootTargetState.SHOOT:
                    Shoot();
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
        private void Process_Idle()
        {
            aiController.StopMovement();
        }
        private void Process_KnifeTarget()
        {

        }
        #endregion
        //***************************

        //***************************
        #region SubFSM Methods
        private void SpawnGun()
        {
            if (aiController.ref_pistol)
            {
                SetState_ShootTarget_MoveToShootTarget();
                return;
            }

            aiController.SpawnGunOnHand();
            SetState_ShootTarget_DrawGun();
        }

        private void DrawGun()
        {
            aiController.DrawGun();
            SetState_ShootTarget_MoveToShootTarget();
        }

        private void MoveToShootTarget()
        {
            if (aiController.WithinStoppingDistance(aiState.hostileHuman.shootTarget.position, aiStats.maxGunRange) &&
                aiController.InLOS3PT(aiState.hostileHuman.shootTarget.position, aiState.hostileHuman.shootTarget.name)
                )
            {
                SetState_ShootTarget_TrackTarget();
                return;
            }

            aiController.SetNMAgentDestination(aiState.hostileHuman.shootTarget.position);
            aiController.MoveAITowardsNMAgentDestination(aiStats.runningSpeed);
        }

        private void TrackTarget()
        {
            if (!aiController.WithinStoppingDistance(aiState.hostileHuman.shootTarget.position, aiStats.maxGunRange) ||
                !aiController.InLOS3PT(aiState.hostileHuman.shootTarget.position, aiState.hostileHuman.shootTarget.name))
            {
                SetState_ShootTarget_MoveToShootTarget();
                return;
            }

            if (aiController.LookingAtTarget(aiState.hostileHuman.shootTarget.position, aiStats.shootTargetDir_AngleMarginOfError))
            {
                SetState_ShootTarget_AimGunOnTarget();
                return;
            }

            aiController.RotateTowardsTargetDirection(aiState.hostileHuman.shootTarget.position);
        }

        private void AimGunOnTarget()
        {
            aiController.AimGun();
            SetState_ShootTarget_Shoot();
        }

        private void Shoot()
        {
            if (!aiController.LookingAtTarget(aiState.hostileHuman.shootTarget.position, aiStats.shootTargetDir_AngleMarginOfError))
            {
                aiController.ResetGunTransformToOrig();
                aiController.LowerGun();
                SetState_ShootTarget_TrackTarget();
                return;
            }

            // Pistol look at target
            aiController.ref_pistol.gameObject.transform.LookAt(aiState.hostileHuman.shootTarget);
            // Shoot
            aiController.ref_pistol.FireGun();
        }
        #endregion
        //***************************
    }
}
