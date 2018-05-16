using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    public class AIFSM_Civillian_UnderAttack : AIFSM_Base
    {
        public void FSM_Update()
        {
            switch (aiState.civilian.underAttack.Civilian_UnderAttack)
            {
                case AIState.Civilian.UnderAttack.AI_Civilian_UnderAttack.SETUP:
                    CivilianUnderAttack_Setup();
                    break;

                case AIState.Civilian.UnderAttack.AI_Civilian_UnderAttack.BRACE_ON_SPOT:
                    BraceOnSpot();
                    break;

                case AIState.Civilian.UnderAttack.AI_Civilian_UnderAttack.RUN_RANDOMLY:
                    RunRandomly();
                    break;
            }
        }
        private void CivilianUnderAttack_Setup()
        {
            if (aiState.general.seated)
            {
                aiController.LeaveIfSittingOnSeat();
            }
            if (aiState.general.inConversation)
            {
                aiAnimController.Anim_StopStandTalking();
                aiState.general.inConversation = false;
            }
            aiState.civilian.underAttack.Civilian_UnderAttack = AIState.Civilian.UnderAttack.AI_Civilian_UnderAttack.RUN_RANDOMLY;
        }

        private void BraceOnSpot()
        {
            if (GameManager.instance.LineOfSightAgainstHostileNPC(aiTransform))
            {
                aiState.civilian.underAttack.Civilian_UnderAttack = AIState.Civilian.UnderAttack.AI_Civilian_UnderAttack.RUN_RANDOMLY;
                aiAnimController.Anim_UnBrace();
                aiState.civilian.underAttack.bracing = false;
                return;
            }

            if (!aiState.civilian.underAttack.bracing)
            {
                aiAnimController.Anim_Brace();
                aiState.civilian.underAttack.bracing = true;
            }
        }

        private void RunRandomly()
        {
            if (!GameManager.instance.LineOfSightAgainstHostileNPC(aiTransform))
            {
                aiState.civilian.underAttack.Civilian_UnderAttack = AIState.Civilian.UnderAttack.AI_Civilian_UnderAttack.BRACE_ON_SPOT;
                aiAnimController.Anim_Move(Vector3.zero, false, 1);
                aiController.StopNMAgentMovement();
                aiState.civilian.underAttack.timeLeftBeforeFindingNewRandPosition = 2;
                return;
            }
            else
            {
                if (aiState.civilian.underAttack.timeLeftBeforeFindingNewRandPosition <= 0)
                {
                    aiState.civilian.underAttack.currMoveVector = aiController.GetRandNavmeshPos(10);
                    aiState.civilian.underAttack.timeLeftBeforeFindingNewRandPosition = 2;
                }
                else
                {
                    aiState.civilian.underAttack.timeLeftBeforeFindingNewRandPosition -= Time.deltaTime;
                    //nmAgent.SetDestination(nmAgent.desiredVelocity);
                    aiController.MoveToPosition(aiState.civilian.underAttack.currMoveVector, 0);
                }
            }
        }
    }
}
