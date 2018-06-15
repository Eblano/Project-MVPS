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
        [SerializeField] private float resetEventBoolFreq;
        private float currResetCD;

        [Header("Event Booleans")]
        [HideInInspector] public bool sitting_Completed;
        [HideInInspector] public bool standing_Completed;

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
        }

        public void AnimEvent_Sitting_Completed()
        {
            sitting_Completed = true;
        }

        public void AnimEvent_StandingFromSeat()
        {
            standing_Completed = true;
        }
        #endregion
    }
}