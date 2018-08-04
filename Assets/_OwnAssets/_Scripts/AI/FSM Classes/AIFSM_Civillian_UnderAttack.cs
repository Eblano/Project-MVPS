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
            aiState.civilian.underAttack.nextScream -= Time.deltaTime;

            switch (aiState.civilian.underAttack.mode)
            {
                case AIState.Civilian.UnderAttack.Mode.SETUP:
                    CivilianUnderAttack_Setup();
                    break;

                case AIState.Civilian.UnderAttack.Mode.FREEZE:
                    BraceOnSpot();
                    break;

                case AIState.Civilian.UnderAttack.Mode.RUNTOEXIT:
                    RunToExit();
                    break;
            }
        }
        private void CivilianUnderAttack_Setup()
        {
            if (aiState.seated)
            {
                bool leftSeat = aiController.LeaveSeat();

                if (!leftSeat)
                    return;
            }

            if (aiState.inConversation)
            {
                aiAnimController.Anim_StopStandTalking();
                aiState.inConversation = false;
            }

            switch (aiStats.threatResponseMode)
            {
                case AIStats.CivillianStressResponseMode.FREEZE:
                    aiState.civilian.underAttack.mode = AIState.Civilian.UnderAttack.Mode.FREEZE;
                    break;
                case AIStats.CivillianStressResponseMode.RUNTOEXIT:
                    aiState.civilian.underAttack.mode = AIState.Civilian.UnderAttack.Mode.RUNTOEXIT;
                    break;
                case AIStats.CivillianStressResponseMode.RANDOM:
                    switch (Random.Range(0, 1))
                    {
                        case 0:
                            aiState.civilian.underAttack.mode = AIState.Civilian.UnderAttack.Mode.FREEZE;
                            break;
                        case 1:
                            aiState.civilian.underAttack.mode = AIState.Civilian.UnderAttack.Mode.RUNTOEXIT;
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
                aiController.SetNMAgentDestination(aiController.transform.position);
            }
            aiController.PlayScreamSFX();
        }

        private void RunToExit()
        {
            switch (actionStage)
            {
                case 0:
                    aiState.civilian.underAttack.currMoveVector = 
                        GameManager.instance.GetNearestExitMarkerVector(aiController.gameObject);

                    if (aiState.civilian.underAttack.currMoveVector == aiController.gameObject.transform.position)
                    {
                        aiState.civilian.underAttack.mode = AIState.Civilian.UnderAttack.Mode.FREEZE;
                        aiController.PlayScreamSFX();
                        break;
                    }

                    aiController.SetNMAgentDestination(aiState.civilian.underAttack.currMoveVector);
                    actionStage++;
                    break;
                case 1:
                    aiController.MoveAITowardsNMAgentDestination(aiStats.runningSpeed);
                    aiController.PlayScreamSFX();

                    if (aiController.ReachedDestination(aiState.civilian.underAttack.currMoveVector, 0))
                        actionStage++;
                    break;
                case 2:
                    aiController.FadeAway();
                    break;
            }
        }
    }
}
