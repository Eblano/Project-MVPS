using ProtoBuf;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ProtoContract]
[System.Serializable]
public class Schedule
{
    public enum SCHEDULE_TYPE
    {
        IDLE, MOVE_TO_POS, MOVE_TO_POS_WITH_ROT, SIT_IN_AREA, TALK_TO_OTHER_NPC
    }

    [ProtoMember(1)]
    public SCHEDULE_TYPE scheduleType;
    [ProtoMember(2)]
    public string argument;
}