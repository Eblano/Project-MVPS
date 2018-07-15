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
            aiState.hostileHuman.currState = AIState.HostileHuman.State.SHOOT_TARGET;
            aiState.hostileHuman.currShootTargetState = AIState.HostileHuman.ShootTargetState.SPAWN_GUN;
            aiState.hostileHuman.shootTarget = GameManager.instance.GetFirstVIPTransform();
        }

        public void SetAction_MoveToWaypoint(string waypointName)
        {
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
                case AIState.HostileHuman.ShootTargetState.SHOOT_TARGET:
                    ShootTarget();
                    break;
            }
        }

        private void Process_MoveToWaypoint()
        {
            aiController.SetNMAgentDestination(aiState.hostileHuman.waypointPos);
            aiController.MoveAITowardsNMAgentDestination(aiStats.runningSpeed);

            if (!aiController.ReachedDestination(aiState.hostileHuman.waypointPos, aiStats.stopDist))
            {
                aiController.SetNMAgentDestination(aiState.hostileHuman.waypointPos);
                aiController.MoveAITowardsNMAgentDestination(aiStats.runningSpeed);
            }
            else
            {
                aiController.StopMovement();
            }
        }

        private void Process_KnifeTarget()
        {

        }

        private void SetState_MoveToWaypoint()
        {
            aiController.SetNMAgentDestination(aiState.hostileHuman.waypointPos);
            aiController.MoveAITowardsNMAgentDestination(aiStats.runningSpeed);
            aiState.hostileHuman.currState = AIState.HostileHuman.State.MOVE_TO_WAYPOINT;
        }

        private void SetState_MoveToShootTarget(Vector3 target)
        {
            aiState.hostileHuman.currState = AIState.HostileHuman.State.SHOOT_TARGET;
            aiController.SetNMAgentDestination(target);
            aiController.MoveAITowardsNMAgentDestination(aiStats.runningSpeed);
        }
        
        private void SpawnGun()
        {
            Debug.LogWarning("SpawnGun not implemented");
            aiState.hostileHuman.currShootTargetState = AIState.HostileHuman.ShootTargetState.DRAW_GUN;
        }
        
        private void DrawGun()
        {
            bool finished = aiController.DrawGun();

            if (finished)
            {
                aiState.hostileHuman.currShootTargetState = AIState.HostileHuman.ShootTargetState.MOVE_TO_SHOOT_TARGET;
                SetState_MoveToShootTarget(aiState.hostileHuman.shootTarget.position);
            }
        }
        
        private void MoveTowardsShootTarget()
        {
            if (!aiController.ReachedDestination(aiState.hostileHuman.shootTarget.position, aiStats.maxGunRange / 2) &&
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
            }
        }
        
        private void TrackTarget()
        {
            aiController.RotateTowardsTargetDirection(aiState.hostileHuman.shootTarget.position);

            if (!aiController.InLOS3PT(aiState.hostileHuman.shootTarget.position, aiState.hostileHuman.shootTarget.name))
            {
                aiState.hostileHuman.currShootTargetState = AIState.HostileHuman.ShootTargetState.MOVE_TO_SHOOT_TARGET;
                SetState_MoveToShootTarget(aiState.hostileHuman.shootTarget.position);
                return;
            }

            if (aiController.LookingAtTarget(aiState.hostileHuman.shootTarget.position, aiStats.lookAngleMarginOfError))
                aiState.hostileHuman.currShootTargetState = AIState.HostileHuman.ShootTargetState.SHOOT_TARGET;
        }
        
        private void ShootTarget()
        {
            if (!aiController.LookingAtTarget(aiState.hostileHuman.shootTarget.position, aiStats.shootTargetDir_AngleMarginOfError))
            {
                aiState.hostileHuman.currShootTargetState = AIState.HostileHuman.ShootTargetState.TRACK_TARGET;
                return;
            }

            if (!aiController.InLOS3PT(aiState.hostileHuman.shootTarget.position, aiState.hostileHuman.shootTarget.name))
            {
                aiState.hostileHuman.currShootTargetState = AIState.HostileHuman.ShootTargetState.MOVE_TO_SHOOT_TARGET;
                SetState_MoveToShootTarget(aiState.hostileHuman.shootTarget.position);
                return;
            }

            aiController.StopMovement();

            // Shoot
            Debug.Log("Shooting " + aiState.hostileHuman.shootTarget.name);
        }
    }
}
