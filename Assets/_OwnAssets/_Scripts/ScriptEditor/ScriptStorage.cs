using ProtoBuf;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
	public class ScriptStorage : MonoBehaviour
    {
        [Battlehub.SerializeIgnore] public static ScriptStorage instance;

        private readonly string baseNPCName = "NPC";
        public enum SCHEDULE_MOVE_DIRECTION { UP, DOWN };
        
        [SerializeField] private List<NPCSpawnData_SStorage> npcSpawnDataList_SStorage;
        [SerializeField] private List<NPCSchedule_SStorage> npcScheduleList_SStorage;
        [SerializeField] private List<AccessoryData_SStorage> accessoryDataList_SStorage;

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

        public List<NPCSpawnData_SStorage> GetAllNPCSpawnData_SStorage()
        {
            return npcSpawnDataList_SStorage;
        }

        public List<NPCSchedule_SStorage> GetAllNPCScheduleData_SStorage()
        {
            return npcScheduleList_SStorage;
        }

        public List<AccessoryData_SStorage> GetAllAccessory_SStorage()
        {
            return accessoryDataList_SStorage;
        }

        public List<NPCSchedule_SStorage> GetSpecificNPCScheduleData_SStorage(string npcName)
        {
            return npcScheduleList_SStorage.FindAll(x => x.npcName == npcName);
        }

        public NPCSpawnData_SStorage AddNewNPCSpawnData_SStorage()
        {
            NPCSpawnData_SStorage newData = new NPCSpawnData_SStorage
            {
                npcName = GetUniqueNPCName()
            };
            npcSpawnDataList_SStorage.Add(newData);

            return newData;
        }

        public AccessoryData_SStorage AddNewAccessoryData_SStorage()
        {
            AccessoryData_SStorage newData = new AccessoryData_SStorage();
            accessoryDataList_SStorage.Add(newData);

            return newData;
        }

        public NPCSchedule_SStorage AddNewNPCScheduleData_SStorage(string npcName)
        {
            NPCSchedule_SStorage newData = new NPCSchedule_SStorage
            {
                npcName = npcName
            };

            npcScheduleList_SStorage.Add(newData);
            return newData;
        }

        public string GetUniqueNPCName()
        {
            int increment = 1;

            if (npcSpawnDataList_SStorage.Count >= 1)
            {
                do
                {
                    increment++;
                }
                while (npcSpawnDataList_SStorage.Exists(x => x.npcName == baseNPCName + " " + increment));
            }

            return baseNPCName + " " + increment;
        }

        public void DeleteAllTargetNPCSpawnData_SStorage(string npcName)
        {
            npcSpawnDataList_SStorage.RemoveAll(x => x.npcName == npcName);
        }

        public void DeleteNPCScheduleData_SStorage(string npcName)
        {
            npcScheduleList_SStorage.RemoveAll(x => x.npcName == npcName);
        }

        public void DeleteAccessoryData_SStorage(AccessoryData_SStorage targetAccessoryData)
        {
            accessoryDataList_SStorage.Remove(targetAccessoryData);
        }

        public void DeleteNPCScheduleData_SStorage(NPCSchedule_SStorage targetNPCSchedule)
        {
            npcScheduleList_SStorage.Remove(targetNPCSchedule);
        }

        private bool CheckIfNameIsUnique(string name)
        {
            return !npcSpawnDataList_SStorage.Exists(x => x.npcName == name);
        }

        public bool ChangeName(string oldName, string newName)
        {
            if (CheckIfNameIsUnique(newName))
            {
                npcSpawnDataList_SStorage.Find(x => x.npcName == oldName).npcName = newName;
                npcScheduleList_SStorage.FindAll(x => x.npcName == oldName).ForEach(x => x.npcName = newName);
                return true;
            }
            else
                return false;
        }
        
        public void MoveScheduleOrder(string npcName, NPCSchedule_SStorage scheduleToMove, SCHEDULE_MOVE_DIRECTION moveDir)
        {
            List<NPCSchedule_SStorage> targetNPCScheduleList_Copy = npcScheduleList_SStorage.FindAll(x => x.npcName == npcName);
            NPCSchedule_SStorage targetScheduleToMove = targetNPCScheduleList_Copy.Find(x => x == npcScheduleList_SStorage.Find(y => y == scheduleToMove));

            foreach (NPCSchedule_SStorage item in targetNPCScheduleList_Copy)
                npcScheduleList_SStorage.Remove(item);

            int itemIndex = targetNPCScheduleList_Copy.FindIndex(x => x == targetScheduleToMove);
            targetNPCScheduleList_Copy.RemoveAt(itemIndex);

            if(moveDir == SCHEDULE_MOVE_DIRECTION.UP)
                targetNPCScheduleList_Copy.Insert(itemIndex - 1, targetScheduleToMove);
            if(moveDir == SCHEDULE_MOVE_DIRECTION.DOWN)
                targetNPCScheduleList_Copy.Insert(itemIndex + 1, targetScheduleToMove);

            npcScheduleList_SStorage.AddRange(targetNPCScheduleList_Copy);
        }

        public List<AccessoryData_SStorage> GetAllAccessoryData_SStorage()
        {
            return accessoryDataList_SStorage;
        }

        public List<NpcSpawnData> GetAllNPCSpawnData()
        {
            List<NpcSpawnData> npcSpawnDataList = new List<NpcSpawnData>();

            foreach(NPCSpawnData_SStorage npcSpawnData_RTEStorage in npcSpawnDataList_SStorage)
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
                foreach (NPCSchedule_SStorage npcSchedule_RTEStorage in GetSpecificNPCScheduleData_SStorage(npcSpawnData_RTEStorage.npcName))
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
                    schedule.argument_1 = npcSchedule_RTEStorage.argument_1;
                    schedule.argument_2 = npcSchedule_RTEStorage.argument_2;
                    npcSchedule.Add(schedule);
                }

                NpcSpawnData npcSpawnData = new NpcSpawnData
                {
                    npcOutfit = npcOutfit,
                    npcSchedules = npcSchedule
                };
                npcSpawnData.aiStats.npcType = npcType;
                npcSpawnData.aiStats.activateOnSpawn = npcSpawnData_RTEStorage.activateOnStart;
                
                // Setting Civillian Stress Respond Mode
                AIStats.CivillianStressResponseMode civillianThreatResponseMode;
                switch (npcSpawnData_RTEStorage.threatResponse)
                {
                    case "Freeze":
                        civillianThreatResponseMode = AIStats.CivillianStressResponseMode.FREEZE;
                        break;
                    case "Run to Exit":
                        civillianThreatResponseMode = AIStats.CivillianStressResponseMode.RUNTOEXIT;
                        break;
                    case "Random":
                        civillianThreatResponseMode = AIStats.CivillianStressResponseMode.RANDOM;
                        break;
                    default:
                        civillianThreatResponseMode = AIStats.CivillianStressResponseMode.FREEZE;
                        break;
                }
                npcSpawnData.aiStats.threatResponseMode = civillianThreatResponseMode;

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
public class NPCSpawnData_SStorage
{
    public string npcName;

    // NPCSpawnData Properties
    private readonly string[] allNPCOutfits = { "None", "Type 0", "Type 1" };
    public string npcOutfit = "TYPE 0";
    public string spawnMarkerName;

    // AI Stats Properties
    public bool activateOnStart;
    private readonly string[] allAITypes = { "Terrorist", "VIP", "Civillian" };
    public string aiType = "Civillian";
    private readonly string[] allCivillianThreatResponses = { "Freeze", "Run to Exit", "Random" };
    public string threatResponse = "Freeze";

    public string[] GetAllCivillianStressResponses()
    {
        return allCivillianThreatResponses;
    }

    public string[] GetAllNPCOutfit()
    {
        return allNPCOutfits;
    }

    public string[] GetAllAITypes()
    {
        return allAITypes;
    }
}

[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllPublic)]
[System.Serializable]
public class NPCSchedule_SStorage
{
    public string npcName;

    // NPCSchedule Properties
    private readonly string[] allScheduleTypes = 
        { "Idle", "Move to Waypoint", "Move to Waypoint + Rotate", "Sit in Area", "Talk to other NPC" };
    public string scheduleType = "Idle";
    public string argument_1;
    public string argument_2;

    public string[] GetAllScheduleTypes()
    {
        return allScheduleTypes;
    }
}

[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllPublic)]
[System.Serializable]
public class AccessoryData_SStorage
{
    private readonly string[] allAccessoryTypes =
        { "Pistol", "Rifle", "Bag", "Magazine"};
    public string accessoryItem = "Pistol";
    public string accessoryMarker;

    public string[] GetAllAccessoryTypes()
    {
        return allAccessoryTypes;
    }
}