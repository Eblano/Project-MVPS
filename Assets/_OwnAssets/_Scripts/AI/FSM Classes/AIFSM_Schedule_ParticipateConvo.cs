using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    public class AIFSM_Schedule_ParticipateConvo : AIFSM_Base
    {
        int actionStage = 0;

        public void FSM_Update()
        {
            switch(actionStage)
            {
                case 0:
                    aiController.RotateTowardsTargetDirection(aiState.currConvoNPCTarget.transform.position);
                    if (aiController.LookingAtTarget(aiState.currConvoNPCTarget.transform.position, aiStats.lookAngleMarginOfError))
                    {
                        aiController.StopRotation();
                        actionStage++;
                    }
                    break;
                case 1:
                    if (aiState.inConversation)
                        actionStage++;
                    break;
                case 2:
                    aiState.timeInConvo++;
                    if (!aiState.inConversation)
                        actionStage = 0;
                    break;
            }
        }
    }
}
