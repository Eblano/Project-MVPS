using ProtoBuf;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
	public class NpcScriptStorage : MonoBehaviour
    {
        [Battlehub.SerializeIgnore] public static NpcScriptStorage instance;
        [SerializeField] private bool editNPCScripts = false;

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

        private void Update()
        {
            if (editNPCScripts)
            {
                editNPCScripts = false;
                NpcScripting.instance.ShowNPCScriptingUI();
            }
        }

        public List<NPCSpawnData_RTEStorage> GetAllNPCSpawnData_RTEStorage()
        {
            return npcSpawnDataList_RTEStorage;
        }

        public List<NPCSchedule_RTEStorage> GetAllNPCScheduleData_RTEStorage()
        {
            return npcScheduleList_RTEStorage;
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

        public List<NpcSpawnData> GetAllNPCSpawnData()
        {
            List<NpcSpawnData> npcSpawnDataList = new List<NpcSpawnData>();

            foreach(NPCSpawnData_RTEStorage npcSpawnData_RTEStorage in npcSpawnDataList_RTEStorage)
            {
                // Setting NPC Outfit
                NpcSpawnData.NPCOutfit npcOutfit;
                switch (npcSpawnData_RTEStorage.aiType)
                {
                    case "TYPE0":
                        npcOutfit = NpcSpawnData.NPCOutfit.TYPE0;
                        break;
                    case "TYPE1":
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
                    case "TERRORIST":
                        npcType = AIStats.NPCType.TERRORIST;
                        break;
                    case "VIP":
                        npcType = AIStats.NPCType.VIP;
                        break;
                    case "CIVILLIAN":
                        npcType = AIStats.NPCType.CIVILLIAN;
                        break;
                    default:
                        npcType = AIStats.NPCType.CIVILLIAN;
                        break;
                }

                // Setting schedules
                List<NPCSchedule> npcSchedule = new List<NPCSchedule>();
                foreach (NPCSchedule_RTEStorage npcSchedule_RTEStorage in npcScheduleList_RTEStorage)
                {
                    NPCSchedule schedule = new NPCSchedule();
                    NPCSchedule.SCHEDULE_TYPE scheduleType;
                    switch (npcSchedule_RTEStorage.scheduleType)
                    {
                        case "IDLE":
                            scheduleType = NPCSchedule.SCHEDULE_TYPE.IDLE;
                            break;
                        case "MOVE_TO_POS":
                            scheduleType = NPCSchedule.SCHEDULE_TYPE.MOVE_TO_POS;
                            break;
                        case "MOVE_TO_POS_WITH_ROT":
                            scheduleType = NPCSchedule.SCHEDULE_TYPE.MOVE_TO_POS_WITH_ROT;
                            break;
                        case "SIT_IN_AREA":
                            scheduleType = NPCSchedule.SCHEDULE_TYPE.SIT_IN_AREA;
                            break;
                        case "TALK_TO_OTHER_NPC":
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

                NpcSpawnData npcSpawnData = new NpcSpawnData();
                npcSpawnData.npcOutfit = npcOutfit;
                npcSpawnData.npcSchedules = npcSchedule;
                npcSpawnData.aiStats.npcType = npcType;
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

    // NPCSpawnData Properties
    [HideInInspector] public string[] defNPCOutfit = { "TYPE0", "TYPE1" };
    public string npcOutfit = "TYPE0";
    public string spawnMarkerName;

    // AI Stats Properties
    [HideInInspector] public string[] defAITypes = { "TERRORIST", "VIP", "CIVILLIAN" };
    public string aiType = "CIVILLIAN";
}

[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllPublic)]
[System.Serializable]
public class NPCSchedule_RTEStorage
{
    public string npcName;

    // NPCSchedule Properties
    [HideInInspector] public string[] defScheduleTypes = { "IDLE", "MOVE_TO_POS", "MOVE_TO_POS_WITH_ROT", "SIT_IN_AREA", "TALK_TO_OTHER_NPC" };
    public string scheduleType = "IDLE";
    public string argument;
}