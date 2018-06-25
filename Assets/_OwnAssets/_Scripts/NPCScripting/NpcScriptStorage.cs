using ProtoBuf;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
	public class NpcScriptStorage : MonoBehaviour
    {
        [Battlehub.SerializeIgnore] public static NpcScriptStorage instance;

        private readonly string baseNPCName = "NPC";
        
        [SerializeField] private List<NPCSpawnData_RTEStorage> npcSpawnDataList_RTEStorage;
        [SerializeField] private List<NPCSchedule_RTEStorage> npcScheduleList_RTEStorage;

        private void OnEnable()
        {
            if (instance == null)
                instance = this;
            else
            {
                Debug.Log("Duplicate NpcSpawnInfoStorage detected, destroying duplicate..");
                Destroy(gameObject);
            }
        }

        private void OnDisable()
        {
            if(instance == this)
                instance = null;
        }

        private void OnDestroy()
        {
            if(instance == this)
                instance = null;
        }

        public List<NPCSpawnData_RTEStorage> GetAllNPCSpawnData_RTEStorage()
        {
            return npcSpawnDataList_RTEStorage;
        }

        public List<NPCSchedule_RTEStorage> GetAllNPCScheduleData_RTEStorage()
        {
            return npcScheduleList_RTEStorage;
        }

        public List<NPCSchedule_RTEStorage> GetSpecificNPCScheduleData_RTEStorage(string npcName)
        {
            return npcScheduleList_RTEStorage.FindAll(x => x.npcName == npcName);
        }

        public NPCSpawnData_RTEStorage AddNewNPCSpawnData_RTEStorage()
        {
            NPCSpawnData_RTEStorage newData = new NPCSpawnData_RTEStorage
            {
                npcName = GetUniqueNPCName()
            };
            npcSpawnDataList_RTEStorage.Add(newData);

            return newData;
        }

        public NPCSchedule_RTEStorage AddNewNPCScheduleData_RTEStorage(string npcName)
        {
            NPCSchedule_RTEStorage newData = new NPCSchedule_RTEStorage
            {
                npcName = npcName
            };

            npcScheduleList_RTEStorage.Add(newData);
            return newData;
        }

        public string GetUniqueNPCName()
        {
            int increment = 0;

            if (npcSpawnDataList_RTEStorage.Count >= 1)
            {
                do
                {
                    increment++;
                }
                while (npcSpawnDataList_RTEStorage.Exists(x => x.npcName == baseNPCName + increment));
            }

            return baseNPCName + increment;
        }

        public void DeleteAllTargetNPCSpawnData_RTEStorage(string npcName)
        {
            npcSpawnDataList_RTEStorage.RemoveAll(x => x.npcName == npcName);
        }

        public void DeleteNPCScheduleData_RTEStorage(string npcName)
        {
            npcScheduleList_RTEStorage.RemoveAll(x => x.npcName == npcName);
        }

        public void DeleteNPCScheduleData_RTEStorage(NPCSchedule_RTEStorage targetNPCSchedule)
        {
            npcScheduleList_RTEStorage.Remove(targetNPCSchedule);
        }

        private bool CheckIfNameIsUnique(string name)
        {
            return !npcSpawnDataList_RTEStorage.Exists(x => x.npcName == name);
        }

        public bool ChangeName(string oldName, string newName)
        {
            if (CheckIfNameIsUnique(newName))
            {
                npcSpawnDataList_RTEStorage.Find(x => x.npcName == oldName).npcName = newName;
                npcScheduleList_RTEStorage.FindAll(x => x.npcName == oldName).ForEach(x => x.npcName = newName);
                return true;
            }
            else
                return false;
        }

        public List<NpcSpawnData> GetAllNPCSpawnData()
        {
            List<NpcSpawnData> npcSpawnDataList = new List<NpcSpawnData>();

            foreach(NPCSpawnData_RTEStorage npcSpawnData_RTEStorage in npcSpawnDataList_RTEStorage)
            {
                // Setting NPC Outfit
                NpcSpawnData.NPCOutfit npcOutfit;
                switch (npcSpawnData_RTEStorage.aiType)
                {
                    case "Type 0":
                        npcOutfit = NpcSpawnData.NPCOutfit.TYPE0;
                        break;
                    case "Type 1":
                        npcOutfit = NpcSpawnData.NPCOutfit.TYPE1;
                        break;
                    default:
                        npcOutfit = NpcSpawnData.NPCOutfit.TYPE0;
                        break;
                }

                // Setting AI Stats
                AIStats.NPCType npcType;
                switch (npcSpawnData_RTEStorage.aiType)
                {
                    case "Terrorist":
                        npcType = AIStats.NPCType.TERRORIST;
                        break;
                    case "VIP":
                        npcType = AIStats.NPCType.VIP;
                        break;
                    case "Civillian":
                        npcType = AIStats.NPCType.CIVILLIAN;
                        break;
                    default:
                        npcType = AIStats.NPCType.CIVILLIAN;
                        break;
                }

                // Setting schedules
                List<NPCSchedule> npcSchedule = new List<NPCSchedule>();
                foreach (NPCSchedule_RTEStorage npcSchedule_RTEStorage in GetSpecificNPCScheduleData_RTEStorage(npcSpawnData_RTEStorage.npcName))
                {
                    NPCSchedule schedule = new NPCSchedule();
                    NPCSchedule.SCHEDULE_TYPE scheduleType;
                    switch (npcSchedule_RTEStorage.scheduleType)
                    {
                        case "Idle":
                            scheduleType = NPCSchedule.SCHEDULE_TYPE.IDLE;
                            break;
                        case "Move to Waypoint":
                            scheduleType = NPCSchedule.SCHEDULE_TYPE.MOVE_TO_POS;
                            break;
                        case "Move to Waypoint + Rotate":
                            scheduleType = NPCSchedule.SCHEDULE_TYPE.MOVE_TO_POS_WITH_ROT;
                            break;
                        case "Sit in Area":
                            scheduleType = NPCSchedule.SCHEDULE_TYPE.SIT_IN_AREA;
                            break;
                        case "Talk to other NPC":
                            scheduleType = NPCSchedule.SCHEDULE_TYPE.TALK_TO_OTHER_NPC;
                            break;
                        default:
                            scheduleType = NPCSchedule.SCHEDULE_TYPE.IDLE;
                            break;
                    }
                    schedule.scheduleType = scheduleType;
                    schedule.argument = npcSchedule_RTEStorage.argument;
                    npcSchedule.Add(schedule);
                }

                NpcSpawnData npcSpawnData = new NpcSpawnData
                {
                    npcOutfit = npcOutfit,
                    npcSchedules = npcSchedule
                };
                npcSpawnData.aiStats.npcType = npcType;
                npcSpawnData.aiStats.activateOnStart = npcSpawnData_RTEStorage.activateOnStart;

                npcSpawnData.npcName = npcSpawnData_RTEStorage.npcName;
                npcSpawnData.spawnMarkerName = npcSpawnData_RTEStorage.spawnMarkerName;

                npcSpawnDataList.Add(npcSpawnData);
            }
            return npcSpawnDataList;
        }
    }
}

[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllPublic)]
[System.Serializable]
public class NPCSpawnData_RTEStorage
{
    public string npcName;
    public bool activateOnStart = true;

    // NPCSpawnData Properties
    private readonly string[] defNPCOutfit = { "None", "Type 0", "Type 1" };
    public string npcOutfit = "TYPE 0";
    public string spawnMarkerName;

    // AI Stats Properties
    private readonly string[] defAITypes = { "Terrorist", "VIP", "Civillian" };
    public string aiType = "Civillian";

    public string[] GetDefNPCOutfit()
    {
        return defNPCOutfit;
    }

    public string[] GetDefAITypes()
    {
        return defAITypes;
    }
}

[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllPublic)]
[System.Serializable]
public class NPCSchedule_RTEStorage
{
    public string npcName;

    // NPCSchedule Properties
    private readonly string[] defScheduleTypes = 
        { "Idle", "Move to Waypoint", "Move to Waypoint + Rotate", "Sit in Area", "Talk to other NPC" };
    public string scheduleType = "Idle";
    public string argument;

    public string[] GetDefScheduleTypes()
    {
        return defScheduleTypes;
    }
}