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
                case AIState.VIP.State.IDLE:
                    break;
                case AIState.VIP.State.FOLLOWING_PLAYER:
                    break;
                case AIState.VIP.State.GRABBED:
                    break;
                default:
                    break;
            }
        }

        public void SetProcess_FollowingPlayer()
        {
            aiState.vip.followSource = GameManager.instance.GetVIPFollowTargetTransform();

            if(aiState.vip.followSource)
            {
                aiController.SetNMAgentDestination(aiState.vip.followSource.position);
                aiState.vip.currState = AIState.VIP.State.FOLLOWING_PLAYER;
            }
        }

        public void SetProcess_Idle()
        {
            aiController.StopMovement();
        }

        private void Process_FollowingPlayer()
        {
            if (!aiState.vip.followSource || aiController.ReachedDestination(aiState.vip.followSource.position, aiStats.vipFollowPlayerDistance))
            {
                SetProcess_Idle();
                return;
            }

            aiController.SetNMAgentDestination(aiState.vip.followSource.position);
            aiController.MoveAITowardsNMAgentDestination(aiStats.runningSpeed);
        }
    }
}
