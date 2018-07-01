using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    public class AIFSM_Schedule_ParticipateConvo : AIFSM_Base
    {
        public void FSM_Update()
        {
            aiController.RotateTowardsTargetRotation(aiState.general.currConvoNPCTarget.transform.rotation, true);

            if (aiState.general.inConversation)
            {
                aiState.general.timeInConvo++;
            }
        }
    }
}
