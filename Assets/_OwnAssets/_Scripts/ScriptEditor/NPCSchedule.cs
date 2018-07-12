using ProtoBuf;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ProtoContract]
[System.Serializable]
public class NPCSchedule
{
    public enum SCHEDULE_TYPE
    {
        IDLE, MOVE_TO_WAYPT_ROT, MOVE_TO_WAYPT, SIT_IN_AREA, TALK_TO_OTHER_NPC
    }

    [ProtoMember(1)]
    public SCHEDULE_TYPE scheduleType;
    [ProtoMember(2)]
    public string argument_1;
    [ProtoMember(3)]
    public string argument_2;
}