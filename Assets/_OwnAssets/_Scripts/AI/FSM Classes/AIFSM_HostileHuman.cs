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
            aiState.hostileHuman.shootTarget = GameManager.instance.GetFirstVIPTransform();
        }

        private void Process_Idle()
        {
            aiController.StopMovement();
        }

        private void Process_ShootTarget()
        {
            switch(aiState.hostileHuman.currSubprocess)
            {
                case 0:
                    SpawnGun();
                    break;
                case 1:
                    DrawGun();
                    break;
                case 2:
                    MoveTowardsShootTarget();
                    break;
                case 3:
                    TrackTarget();
                    break;
                case 4:
                    ShootTarget();
                    break;
            }
        }

        private void Process_KnifeTarget()
        {

        }

        private void Process_MoveToWaypoint()
        {

        }

        private void SpawnGun()
        {
            Debug.LogWarning("SpawnGun not implemented");
            aiState.hostileHuman.currSubprocess++;
        }

        private void MoveTowardsShootTarget()
        {
            if (!aiController.ReachedDestination(aiState.hostileHuman.shootTarget.position, (aiStats.maxGunRange + aiStats.minGunRange) / 2) &&
                !aiController.InLOS(aiController.headPos.position, aiState.hostileHuman.shootTarget.position, aiState.hostileHuman.shootTarget.name)
                )
            {
                aiController.SetNMAgentDestination(aiState.hostileHuman.shootTarget.position);
                aiController.MoveAITowardsNMAgentDestination(aiStats.runningSpeed);
            }
            else
            {
                aiController.StopMovement();
                aiState.hostileHuman.currSubprocess++;
            }
        }

        private void TrackTarget()
        {
            bool facingTarget = aiController.RotateTowardsTargetDirection(aiState.hostileHuman.shootTarget.position);
            if (facingTarget)
                aiState.hostileHuman.currSubprocess++;
        }

        private void ShootTarget()
        {
            if(!aiController.InLOS(aiController.headPos.position, aiState.hostileHuman.shootTarget.position, aiState.hostileHuman.shootTarget.name))
            {
                aiState.hostileHuman.currSubprocess = 2;
                return;
            }

            // Shoot
            Debug.Log("Shooting " + aiState.hostileHuman.shootTarget.name);
        }

        private void DrawGun()
        {
            bool finished = aiController.DrawGun();

            if (finished)
            {
                aiController.SetNMAgentDestination(aiState.hostileHuman.shootTarget.position);
                aiState.hostileHuman.currSubprocess++;
            }
        }
    }
}
