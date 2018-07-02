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
                    bool facingTarget = aiController.RotateTowardsTargetDirection(aiState.general.currConvoNPCTarget.transform.position);
                    if (facingTarget)
                    {
                        aiController.StopRotation();
                        actionStage++;
                    }
                    break;
                case 1:
                    if (aiState.general.inConversation)
                        actionStage++;
                    break;
                case 2:
                    aiState.general.timeInConvo++;
                    if (!aiState.general.inConversation)
                        actionStage = 0;
                    break;
            }
        }
    }
}
