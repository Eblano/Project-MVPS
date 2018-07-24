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
            if(aiState.hostileHuman.shootTarget)
            {
                SetState_ShootTarget();
                SetState_ShootTarget_SpawnGun();
            }
        }

        public void SetAction_SwitchToKnifeVIP()
        {
            aiState.hostileHuman.knifeTarget = GameManager.instance.GetFirstVIPTransform();
            if (aiState.hostileHuman.knifeTarget)
            {
                SetState_KnifeTarget();
                SetState_KnifeTarget_SpawnKnife();
            }
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

        public void SetState_KnifeTarget()
        {
            aiState.hostileHuman.currState = AIState.HostileHuman.State.KNIFE_TARGET;
        }

        private void SetState_MoveToWaypoint()
        {
            aiState.hostileHuman.currState = AIState.HostileHuman.State.MOVE_TO_WAYPOINT;
        }

        private void SetState_ShootTarget_MoveToShootTarget()
        {
            aiState.hostileHuman.currShootTargetState = AIState.HostileHuman.ShootTargetState.MOVE_TO_SHOOT_TARGET;
            aiController.SetNMAgentDestination(aiState.hostileHuman.shootTarget.position);
            aiController.MoveAITowardsNMAgentDestination(aiStats.runningSpeed);
        }

        private void SetState_KnifeTarget_MoveToKnifeTarget()
        {
            aiState.hostileHuman.currKnifeTargetState = AIState.HostileHuman.KnifeTargetState.MOVE_TO_KNIFE_TARGET;
            aiController.SetNMAgentDestination(aiState.hostileHuman.knifeTarget.position);
            aiController.MoveAITowardsNMAgentDestination(aiStats.runningSpeed);
        }

        private void SetState_ShootTarget_Shoot()
        {
            aiState.hostileHuman.currShootTargetState = AIState.HostileHuman.ShootTargetState.SHOOT;
            aiController.StopMovement();
        }

        private void SetState_KnifeTarget_Knife()
        {
            aiState.hostileHuman.currKnifeTargetState = AIState.HostileHuman.KnifeTargetState.KNIFE;
            aiController.StopMovement();
        }

        private void SetState_ShootTarget_DrawGun()
        {
            aiState.hostileHuman.currShootTargetState = AIState.HostileHuman.ShootTargetState.DRAW_GUN;
        }

        private void SetState_KnifeTarget_DrawKnife()
        {
            aiState.hostileHuman.currKnifeTargetState = AIState.HostileHuman.KnifeTargetState.DRAW_KNIFE;
        }

        private void SetState_ShootTarget_TrackTarget()
        {
            aiController.StopMovement();
            aiState.hostileHuman.currShootTargetState = AIState.HostileHuman.ShootTargetState.TRACK_TARGET;
        }

        private void SetState_KnifeTarget_TrackTarget()
        {
            aiController.StopMovement();
            aiState.hostileHuman.currKnifeTargetState = AIState.HostileHuman.KnifeTargetState.TRACK_TARGET;
        }

        private void SetState_ShootTarget_AimGunOnTarget()
        {
            aiState.hostileHuman.currShootTargetState = AIState.HostileHuman.ShootTargetState.AIM_GUN_ON_TARGET;
        }

        private void SetState_ShootTarget_SpawnGun()
        {
            aiState.hostileHuman.currShootTargetState = AIState.HostileHuman.ShootTargetState.SPAWN_GUN;
        }

        private void SetState_KnifeTarget_SpawnKnife()
        {
            aiState.hostileHuman.currKnifeTargetState = AIState.HostileHuman.KnifeTargetState.SPAWN_KNIFE;
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
                    ShootTarget_SpawnGun();
                    break;
                case AIState.HostileHuman.ShootTargetState.DRAW_GUN:
                    ShootTarget_DrawGun();
                    break;
                case AIState.HostileHuman.ShootTargetState.MOVE_TO_SHOOT_TARGET:
                    ShootTarget_MoveToShootTarget();
                    break;
                case AIState.HostileHuman.ShootTargetState.TRACK_TARGET:
                    ShootTarget_TrackTarget();
                    break;
                case AIState.HostileHuman.ShootTargetState.AIM_GUN_ON_TARGET:
                    ShootTarget_AimGunOnTarget();
                    break;
                case AIState.HostileHuman.ShootTargetState.SHOOT:
                    ShootTarget_Shoot();
                    break;
            }
        }
        private void Process_KnifeTarget()
        {
            switch (aiState.hostileHuman.currKnifeTargetState)
            {
                case AIState.HostileHuman.KnifeTargetState.INACTIVE:
                    break;
                case AIState.HostileHuman.KnifeTargetState.SPAWN_KNIFE:
                    KnifeTarget_SpawnKnife();
                    break;
                case AIState.HostileHuman.KnifeTargetState.DRAW_KNIFE:
                    KnifeTarget_DrawKnife();
                    break;
                case AIState.HostileHuman.KnifeTargetState.MOVE_TO_KNIFE_TARGET:
                    KnifeTarget_MoveToKnifeTarget();
                    break;
                case AIState.HostileHuman.KnifeTargetState.TRACK_TARGET:
                    KnifeTarget_TrackTarget();
                    break;
                case AIState.HostileHuman.KnifeTargetState.KNIFE:
                    KnifeTarget_Knife();
                    break;
                default:
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
        #endregion
        //***************************

        //***************************
        #region SubFSM Methods
        private void ShootTarget_SpawnGun()
        {
            if (aiController.ref_pistol)
            {
                SetState_ShootTarget_MoveToShootTarget();
                return;
            }

            aiController.SpawnGunOnHand();
            SetState_ShootTarget_DrawGun();
        }

        private void ShootTarget_DrawGun()
        {
            aiController.DrawWeapon();
            SetState_ShootTarget_MoveToShootTarget();
        }

        private void KnifeTarget_DrawKnife()
        {
            aiController.DrawWeapon();
            SetState_KnifeTarget_MoveToKnifeTarget();
        }

        private void ShootTarget_MoveToShootTarget()
        {
            if (aiController.WithinDistance(aiState.hostileHuman.shootTarget.position, aiStats.maxGunRange) &&
                aiController.InLOS3PT(aiState.hostileHuman.shootTarget.position, aiState.hostileHuman.shootTarget.root.name, 1)
                )
            {
                SetState_ShootTarget_TrackTarget();
                return;
            }

            aiController.SetNMAgentDestination(aiState.hostileHuman.shootTarget.position);
            aiController.MoveAITowardsNMAgentDestination(aiStats.runningSpeed);
        }

        private void KnifeTarget_MoveToKnifeTarget()
        {
            if (aiController.WithinDistance(aiState.hostileHuman.knifeTarget.position, aiStats.meleeDist))
            {
                SetState_KnifeTarget_TrackTarget();
                return;
            }

            aiController.SetNMAgentDestination(aiState.hostileHuman.knifeTarget.position);
            aiController.MoveAITowardsNMAgentDestination(aiStats.runningSpeed);
        }

        private void ShootTarget_TrackTarget()
        {
            if (!aiController.WithinDistance(aiState.hostileHuman.shootTarget.position, aiStats.maxGunRange) ||
                !aiController.InLOS3PT(aiState.hostileHuman.shootTarget.position, aiState.hostileHuman.shootTarget.root.name, 1))
            {
                SetState_ShootTarget_MoveToShootTarget();
                return;
            }

            if (aiController.LookingAtTarget(aiState.hostileHuman.shootTarget.root.position, aiStats.shootTargetDir_AngleMarginOfError))
            {
                SetState_ShootTarget_AimGunOnTarget();
                return;
            }

            aiController.RotateTowardsTargetDirection(aiState.hostileHuman.shootTarget.position);
        }

        private void KnifeTarget_TrackTarget()
        {
            if (!aiController.WithinDistance(aiState.hostileHuman.knifeTarget.position, aiStats.meleeDist))
            {
                SetState_KnifeTarget_MoveToKnifeTarget();
                return;
            }

            if (aiController.LookingAtTarget(aiState.hostileHuman.knifeTarget.root.position, aiStats.shootTargetDir_AngleMarginOfError))
            {
                SetState_KnifeTarget_Knife();
                return;
            }

            aiController.RotateTowardsTargetDirection(aiState.hostileHuman.knifeTarget.position);
        }

        private void ShootTarget_AimGunOnTarget()
        {
            aiController.AimGun();
            SetState_ShootTarget_Shoot();
        }

        private void ShootTarget_Shoot()
        {
            if (!aiController.LookingAtTarget(aiState.hostileHuman.shootTarget.root.position, aiStats.shootTargetDir_AngleMarginOfError))
            {
                aiState.hostileHuman.currGunCD = aiStats.gunCD;
                aiController.ResetGunTransformToOrig();
                aiController.LowerGun();
                SetState_ShootTarget_TrackTarget();
                return;
            }

            if (!aiController.WithinDistance(aiState.hostileHuman.shootTarget.position, aiStats.maxGunRange) ||
                !aiController.InLOS3PT(aiState.hostileHuman.shootTarget.position, aiState.hostileHuman.shootTarget.root.name, 1))
            {
                SetState_ShootTarget_MoveToShootTarget();
                return;
            }

            // Pistol look at target
            aiController.ref_pistol.gameObject.transform.LookAt(aiState.hostileHuman.shootTarget);

            // Shoot
            if (aiState.hostileHuman.currGunCD <= 0)
            {
                aiController.FireGun();
                aiState.hostileHuman.currGunCD = aiStats.gunCD;
            }
            else
                aiState.hostileHuman.currGunCD -= Time.deltaTime;
        }

        private void KnifeTarget_Knife()
        {
            if (!aiController.LookingAtTarget(aiState.hostileHuman.knifeTarget.root.position, aiStats.shootTargetDir_AngleMarginOfError))
            {
                aiState.hostileHuman.currKnifeSwingCD = aiStats.knifeSwingCD;
                aiController.ResetKnifeTransformToOrig();
                SetState_KnifeTarget_TrackTarget();
                return;
            }

            // Swing Knife
            if (aiState.hostileHuman.currGunCD <= 0)
            {
                aiController.SwingKnife();
                aiState.hostileHuman.currKnifeSwingCD = aiStats.knifeSwingCD;
            }
            else
                aiState.hostileHuman.currKnifeSwingCD -= Time.deltaTime;
        }

        private void KnifeTarget_SpawnKnife()
        {
            if (aiController.ref_knife)
            {
                SetState_KnifeTarget_MoveToKnifeTarget();
                return;
            }

            aiController.SpawnKnifeOnHand();
            SetState_KnifeTarget_DrawKnife();
        }
        #endregion
        //***************************
    }
}
