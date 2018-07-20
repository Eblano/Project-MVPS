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
                    Process_Idle();
                    break;
                case AIState.VIP.State.FOLLOW_PLAYER:
                    Process_FollowPlayer();
                    break;
                case AIState.VIP.State.GRABBED_FOLLOW_PLAYER:
                    Process_GrabbedFollowPlayer();
                    break;
            }
        }

        //**************************
        #region SetProcess methods
        public void SetProcess_FollowPlayer()
        {
            aiState.vip.playerFollowTarget = GameManager.instance.GetVIPFollowTargetTransform();

            if(!aiState.vip.playerFollowTarget)
            {
                SetProcess_Idle();
            }

            aiController.SetNMAgentDestination(aiState.vip.playerFollowTarget.position);
            aiState.vip.currState = AIState.VIP.State.FOLLOW_PLAYER;
        }

        public void SetProcess_GrabbedFollowPlayer(Transform grabSource)
        {
            aiState.vip.playerFollowTarget = grabSource;

            if (!aiState.vip.playerFollowTarget)
            {
                SetProcess_Idle();
            }

            aiController.SetNMAgentDestination(aiState.vip.playerFollowTarget.position);
            aiState.vip.currState = AIState.VIP.State.GRABBED_FOLLOW_PLAYER;
        }

        public void SetProcess_Idle()
        {
            aiController.StopMovement();
            aiState.vip.currState = AIState.VIP.State.IDLE;
        }
        #endregion
        //**************************

        //**************************
        #region Process methods
        public void Process_Idle()
        {
            aiController.SetNMAgentDestination(aiController.transform.position);
            if (aiState.vip.playerFollowTarget && !aiController.ReachedDestination(aiState.vip.playerFollowTarget.position, aiStats.vipFollowPlayerDistance))
            {
                SetProcess_FollowPlayer();
                return;
            }
            
            aiController.StopMovement();
        }

        private void Process_FollowPlayer()
        {
            if (!aiState.vip.playerFollowTarget || aiController.ReachedDestination(aiState.vip.playerFollowTarget.position, aiStats.vipFollowPlayerDistance))
            {
                SetProcess_Idle();
                return;
            }

            aiController.SetNMAgentDestination(aiState.vip.playerFollowTarget.position);
            aiController.MoveAITowardsNMAgentDestination(aiStats.runningSpeed);
        }

        private void Process_GrabbedFollowPlayer()
        {
            if (!aiState.vip.playerFollowTarget || aiController.ReachedDestination(aiState.vip.playerFollowTarget.position, aiStats.vipGrabbedPlayerDistance))
            {
                SetProcess_Idle();
                return;
            }

            aiController.SetNMAgentDestination(aiState.vip.playerFollowTarget.position);
            aiController.MoveAITowardsNMAgentDestination(aiStats.runningSpeed);
        }
        #endregion
        //**************************
    }
}
