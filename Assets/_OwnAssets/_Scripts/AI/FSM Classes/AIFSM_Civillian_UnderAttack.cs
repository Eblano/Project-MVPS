using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    public class AIFSM_Civillian_UnderAttack : AIFSM_Base
    {
        int actionStage = 0;

        public void FSM_Update()
        {
            switch (aiState.civilian.underAttack.Civilian_UnderAttack)
            {
                case AIState.Civilian.UnderAttack.AI_Civilian_UnderAttack.SETUP:
                    CivilianUnderAttack_Setup();
                    break;

                case AIState.Civilian.UnderAttack.AI_Civilian_UnderAttack.FREEZE:
                    BraceOnSpot();
                    break;

                case AIState.Civilian.UnderAttack.AI_Civilian_UnderAttack.RUNTOEXIT:
                    RunToExit();
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

            switch (aiStats.stressResponseMode)
            {
                case AIStats.CivillianStressResponseMode.FREEZE:
                    aiState.civilian.underAttack.Civilian_UnderAttack = AIState.Civilian.UnderAttack.AI_Civilian_UnderAttack.FREEZE;
                    break;
                case AIStats.CivillianStressResponseMode.RUNTOEXIT:
                    aiState.civilian.underAttack.Civilian_UnderAttack = AIState.Civilian.UnderAttack.AI_Civilian_UnderAttack.RUNTOEXIT;
                    break;
                case AIStats.CivillianStressResponseMode.RANDOM:
                    switch (Random.Range(0, 1))
                    {
                        case 0:
                            aiState.civilian.underAttack.Civilian_UnderAttack = AIState.Civilian.UnderAttack.AI_Civilian_UnderAttack.FREEZE;
                            break;
                        case 1:
                            aiState.civilian.underAttack.Civilian_UnderAttack = AIState.Civilian.UnderAttack.AI_Civilian_UnderAttack.RUNTOEXIT;
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        private void BraceOnSpot()
        {
            if (!aiState.civilian.underAttack.bracing)
            {
                aiAnimController.Anim_Brace();
                aiState.civilian.underAttack.bracing = true;
                aiController.StopNMAgentMovement();
            }
        }

        private void RunToExit()
        {
            switch (actionStage)
            {
                case 0:
                    aiState.civilian.underAttack.currMoveVector = 
                        GameManager.instance.GetNearestExitMarkerVector(aiController.gameObject);

                    aiController.SetNMAgentDestination(aiState.civilian.underAttack.currMoveVector);
                    actionStage++;
                    break;
                case 1:
                    bool reachedWaypoint = aiController.MoveToPosition(aiState.civilian.underAttack.currMoveVector, 0);

                    if (reachedWaypoint)
                        actionStage++;
                    break;
                case 2:
                    aiController.FadeAway();
                    break;
            }
        }
    }
}
