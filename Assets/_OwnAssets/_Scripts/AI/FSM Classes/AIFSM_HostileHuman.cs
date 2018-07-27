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
            aiState.hostileHuman.shootTargetT = GameManager.instance.GetFirstVIPCenterMassTransform();
            if(aiState.hostileHuman.shootTargetT)
            {
                SetState_ShootTarget();
                SetState_ShootTarget_SpawnGun();
            }
        }

        public void SetAction_SwitchToKnifeVIP()
        {
            aiState.hostileHuman.knifeTargetT = GameManager.instance.GetFirstVIPCenterMassTransform();
            if (aiState.hostileHuman.knifeTargetT)
            {
                SetState_KnifeTarget();
                SetState_KnifeTarget_SpawnKnife();
            }
        }

        public void SetAction_MoveToWaypoint(string waypointName)
        {
            if (aiState.hostileHuman.currShootTargetState == AIState.HostileHuman.ShootTargetState.SHOOT ||
                aiState.hostileHuman.currShootTargetState == AIState.HostileHuman.ShootTargetState.AIM_GUN_ON_TARGET)
            {
                aiState.hostileHuman.currShootTargetState = AIState.HostileHuman.ShootTargetState.INACTIVE;
                //aiController.ResetGunTransformToOrig();
                aiController.LowerGun();
            }

            aiState.hostileHuman.waypointPos = GameManager.instance.GetWaypointMarkerPosition(waypointName);
            SetState_MoveToWaypoint();
        }

        #endregion

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
            aiController.SetNMAgentDestination(aiState.hostileHuman.shootTargetT.position);
            aiController.MoveAITowardsNMAgentDestination(aiStats.runningSpeed);
        }

        private void SetState_KnifeTarget_MoveToKnifeTarget()
        {
            aiState.hostileHuman.currKnifeTargetState = AIState.HostileHuman.KnifeTargetState.MOVE_TO_KNIFE_TARGET;
            aiController.SetNMAgentDestination(aiState.hostileHuman.knifeTargetT.position);
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

        private void SetState_ShootTarget_Idle()
        {
            aiState.hostileHuman.currShootTargetState = AIState.HostileHuman.ShootTargetState.INACTIVE;
            aiController.StopMovement();
        }

        private void SetState_KnifeTarget_Idle()
        {
            aiState.hostileHuman.currKnifeTargetState = AIState.HostileHuman.KnifeTargetState.INACTIVE;
            aiController.StopMovement();
        }
        #endregion

        //***************************
        #region FSM Methods

        private void Process_ShootTarget()
        {
            switch (aiState.hostileHuman.currShootTargetState)
            {
                case AIState.HostileHuman.ShootTargetState.INACTIVE:
                    ShootTarget_Inactive();
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
                    KnifeTarget_Inactive();
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
        #region SubFSM Methods

        private void KnifeTarget_Inactive()
        {
            if (aiController.KnifeSpawned())
                aiController.SetKnifeTransformOffset(aiController.knife_TOffset);

            aiController.StopMovement();
        }

        private void ShootTarget_Inactive()
        {
            if (aiController.GunSpawned())
                aiController.SetGunTransformOffset(aiController.pistol_NormalOffset);

            aiController.StopMovement();
        }

        private void ShootTarget_SpawnGun()
        {
            if (aiController.GunSpawned())
            {
                SetState_ShootTarget_MoveToShootTarget();
                return;
            }

            if (aiController.GunSpawned())
                aiController.SetGunTransformOffset(aiController.pistol_NormalOffset);

            aiController.SpawnGunOnHand();
            SetState_ShootTarget_DrawGun();
        }

        private void ShootTarget_DrawGun()
        {
            if (aiController.GunSpawned())
                aiController.SetGunTransformOffset(aiController.pistol_NormalOffset);

            aiController.DrawWeapon();
            SetState_ShootTarget_MoveToShootTarget();
            GameManager.instance.TriggerThreatInLevel();
        }

        private void KnifeTarget_DrawKnife()
        {
            if (aiController.KnifeSpawned())
                aiController.SetKnifeTransformOffset(aiController.knife_TOffset);

            aiController.DrawWeapon();
            SetState_KnifeTarget_MoveToKnifeTarget();
            GameManager.instance.TriggerThreatInLevel();
        }

        private void ShootTarget_MoveToShootTarget()
        {
            if (aiController.GunSpawned())
                aiController.SetGunTransformOffset(aiController.pistol_NormalOffset);

            if (!aiState.hostileHuman.shootTargetT)
            {
                SetState_ShootTarget_Idle();
                return;
            }

            if (aiController.WithinDistance(aiState.hostileHuman.shootTargetT.position, aiStats.maxGunRange) &&
                aiController.InLOS3PT(aiState.hostileHuman.shootTargetT.position, aiState.hostileHuman.shootTargetT.root.name)
                )
            {
                SetState_ShootTarget_TrackTarget();
                return;
            }

            aiController.SetNMAgentDestination(aiState.hostileHuman.shootTargetT.position);
            aiController.MoveAITowardsNMAgentDestination(aiStats.runningSpeed);
        }

        private void KnifeTarget_MoveToKnifeTarget()
        {
            if (aiController.KnifeSpawned())
                aiController.SetKnifeTransformOffset(aiController.knife_TOffset);

            if (!aiState.hostileHuman.knifeTargetT)
            {
                SetState_KnifeTarget_Idle();
                return;
            }

            if (aiController.WithinDistance(aiState.hostileHuman.knifeTargetT.position, aiStats.meleeDist))
            {
                SetState_KnifeTarget_TrackTarget();
                return;
            }

            aiController.SetNMAgentDestination(aiState.hostileHuman.knifeTargetT.position);
            aiController.MoveAITowardsNMAgentDestination(aiStats.runningSpeed);
        }

        private void ShootTarget_TrackTarget()
        {
            if (aiController.GunSpawned())
                aiController.SetGunTransformOffset(aiController.pistol_NormalOffset);

            if (!aiState.hostileHuman.shootTargetT)
            {
                SetState_ShootTarget_Idle();
                return;
            }
            
            // Use LOS
            if (!aiController.WithinDistance(aiState.hostileHuman.shootTargetT.position, aiStats.maxGunRange + 1) ||
                !aiController.InLOS(aiState.hostileHuman.shootTargetT.position, aiState.hostileHuman.shootTargetT.root.name))
            {
                SetState_ShootTarget_MoveToShootTarget();
                return;
            }

            if (aiController.LookingAtTarget(aiState.hostileHuman.shootTargetT.position, aiStats.targetDir_AngleMarginOfError))
            {
                SetState_ShootTarget_AimGunOnTarget();
                return;
            }

            aiController.RotateTowardsTargetDirection(aiState.hostileHuman.shootTargetT.position);
        }

        private void KnifeTarget_TrackTarget()
        {
            if (aiController.KnifeSpawned())
                aiController.SetKnifeTransformOffset(aiController.knife_TOffset);

            if (!aiState.hostileHuman.knifeTargetT)
            {
                SetState_KnifeTarget_Idle();
                return;
            }

            if (!aiController.WithinDistance(aiState.hostileHuman.knifeTargetT.position, aiStats.meleeDist))
            {
                SetState_KnifeTarget_MoveToKnifeTarget();
                return;
            }

            if (aiController.LookingAtTarget(aiState.hostileHuman.knifeTargetT.position, aiStats.targetDir_AngleMarginOfError))
            {
                SetState_KnifeTarget_Knife();
                return;
            }

            aiController.RotateTowardsTargetDirection(aiState.hostileHuman.knifeTargetT.position);
        }

        private void ShootTarget_AimGunOnTarget()
        {
            if (aiController.GunSpawned())
                aiController.SetGunTransformOffset(aiController.pistol_HoldingGunOffset);

            aiController.AimGun();
            SetState_ShootTarget_Shoot();
        }

        private void ShootTarget_Shoot()
        {
            if (aiController.GunSpawned())
                aiController.SetGunTransformOffset(aiController.pistol_HoldingGunOffset);
        
            if (!aiState.hostileHuman.shootTargetT)
            {
                SetState_ShootTarget_Idle();
                return;
            }

            if (!aiController.LookingAtTarget(aiState.hostileHuman.shootTargetT.position, aiStats.targetDir_AngleMarginOfError))
            {
                aiState.hostileHuman.currGunCD = aiStats.gunCD;
                aiController.LowerGun();
                SetState_ShootTarget_TrackTarget();
                return;
            }

            // Use LOS
            if (!aiController.WithinDistance(aiState.hostileHuman.shootTargetT.position, aiStats.maxGunRange + 1) ||
                !aiController.InLOS(aiState.hostileHuman.shootTargetT.position, aiState.hostileHuman.shootTargetT.root.name))
            {
                SetState_ShootTarget_MoveToShootTarget();
                return;
            }

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
            if (aiController.KnifeSpawned())
                aiController.SetKnifeTransformOffset(aiController.knife_TOffset);

            if (!aiState.hostileHuman.knifeTargetT)
            {
                SetState_KnifeTarget_Idle();
                return;
            }

            if (!aiController.LookingAtTarget(aiState.hostileHuman.knifeTargetT.position, aiStats.targetDir_AngleMarginOfError))
            {
                aiState.hostileHuman.currKnifeSwingCD = aiStats.knifeSwingCD;
                SetState_KnifeTarget_TrackTarget();
                return;
            }

            // Swing Knife
            if (aiState.hostileHuman.currKnifeSwingCD <= 0)
            {
                aiController.SwingKnife();
                aiState.hostileHuman.currKnifeSwingCD = aiStats.knifeSwingCD;
            }
            else
                aiState.hostileHuman.currKnifeSwingCD -= Time.deltaTime;
        }

        private void KnifeTarget_SpawnKnife()
        {
            if (aiController.KnifeSpawned())
                aiController.SetKnifeTransformOffset(aiController.knife_TOffset);

            if (aiController.KnifeSpawned())
            {
                SetState_KnifeTarget_MoveToKnifeTarget();
                return;
            }

            aiController.SpawnKnifeOnHand();
            SetState_KnifeTarget_DrawKnife();
        }
        #endregion
    }
}
