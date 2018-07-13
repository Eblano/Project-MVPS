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
                    DrawGun();
                    break;
                case 1:
                    MoveTowardsShootTarget();
                    break;
                case 2:
                    break;
            }
        }

        private void Process_KnifeTarget()
        {

        }

        private void Process_MoveToWaypoint()
        {

        }

        private void MoveTowardsShootTarget()
        {
            if (!aiController.ReachedDestination(aiState.hostileHuman.shootTarget.position, (aiStats.maxGunRange + aiStats.minGunRange)/2) //&&
                // In LOS
                )
                aiController.MoveAITowardsNMAgentDestination(aiStats.runningSpeed);
            else
            {
                aiController.StopMovement();
                aiState.hostileHuman.currSubprocess++;
            }
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
