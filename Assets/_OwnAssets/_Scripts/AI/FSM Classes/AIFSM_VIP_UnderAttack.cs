using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    public class AIFSM_VIP_UnderAttack : AIFSM_Base
    {
        public void FSM_Update()
        {
            switch (aiState.vip.currState)
            {
                case AIState.VIP.State.FOLLOW_PLAYER:
                    Process_FollowPlayer();
                    break;
                case AIState.VIP.State.GRABBED_FOLLOW_PLAYER:
                    Process_GrabbedFollowPlayer();
                    break;
            }
        }

        public void SetProcess_FollowingPlayer()
        {
            aiState.vip.followSource = GameManager.instance.GetVIPFollowTargetTransform();

            if(aiState.vip.followSource)
            {
                aiController.SetNMAgentDestination(aiState.vip.followSource.position);
                aiState.vip.currState = AIState.VIP.State.FOLLOW_PLAYER;
            }
        }

        public void SetProcess_Idle()
        {
            aiController.StopMovement();
            aiState.vip.currState = AIState.VIP.State.IDLE;
        }

        public void Process_Idle()
        {
            if (aiState.vip.followSource)
            {
                aiController.SetNMAgentDestination(aiState.vip.followSource.position);
                aiState.vip.currState = AIState.VIP.State.FOLLOW_PLAYER;
                return;
            }

            aiController.StopMovement();
        }

        private void Process_FollowPlayer()
        {
            if (!aiState.vip.followSource || aiController.ReachedDestination(aiState.vip.followSource.position, aiStats.vipFollowPlayerDistance))
                return;

            aiController.SetNMAgentDestination(aiState.vip.followSource.position);
            aiController.MoveAITowardsNMAgentDestination(aiStats.runningSpeed);
        }

        private void Process_GrabbedFollowPlayer()
        {

        }
    }
}
