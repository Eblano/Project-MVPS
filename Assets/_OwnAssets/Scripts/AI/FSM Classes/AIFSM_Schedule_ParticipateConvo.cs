using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    public class AIFSM_Schedule_ParticipateConvo : AIFSM_Base
    {
        public void FSM_Update()
        {
            if (!aiController.RotationIsInLine(aiState.general.currConvoNPCTarget.transform))
            {
                aiController.RotateToTargetRotation(aiState.general.currConvoNPCTarget.transform, true);
            }

            if (aiState.general.inConversation)
            {
                aiState.general.timeInConvo++;
            }
        }
    }
}
