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
                case AIState.VIP.State.FOLLOWING_PLAYER:
                    break;
                case AIState.VIP.State.GRABBED:
                    break;
                default:
                    break;
            }
        }
    }
}
