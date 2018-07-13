using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    /// <summary>
    /// Reciever for Animation Events
    /// </summary>
    public class AIAnimEventReciever : MonoBehaviour
    {
        private float resetEventBoolFreq = 0.1f;
        private float currResetCD;

        [Header("Event Booleans")]
        [HideInInspector] public bool sitting_Completed;
        [HideInInspector] public bool standing_Completed;
        [HideInInspector] public bool gunDraw_Completed;
        [HideInInspector] public bool gunKeep_Completed;

        private void Start()
        {
            currResetCD = resetEventBoolFreq;
        }

        private void Update()
        {
            if (currResetCD <= 0)
            {
                SetAllToFalse();
                currResetCD = resetEventBoolFreq;
            }
            else
            {
                currResetCD -= Time.deltaTime;
            }
        }

        #region Animation Event Methods
        private void SetAllToFalse()
        {
            sitting_Completed = false;
            standing_Completed = false;
            gunDraw_Completed = false;
            gunKeep_Completed = false;
        }

        public void AnimEvent_Sitting_Completed()
        {
            sitting_Completed = true;
        }

        public void AnimEvent_StandingFromSeat()
        {
            standing_Completed = true;
        }

        public void AnimEvent_GunDrawn()
        {
            gunDraw_Completed = true;
        }

        public void AnimEvent_GunKept()
        {
            gunKeep_Completed = true;
        }
        #endregion
    }
}