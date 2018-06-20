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

        public List<NPCSpawnData_RTEStorage> GetAllNPCSpawnData()
        {
            return npcSpawnDataList_RTEStorage;
        }

        public List<NPCSchedule_RTEStorage> GetAllNPCScheduleData()
        {
            return npcScheduleList_RTEStorage;
        }

        public NPCSpawnData_RTEStorage AddNewNPCSpawnData()
        {
            NPCSpawnData_RTEStorage newData = new NPCSpawnData_RTEStorage
            {
                npcName = GetUniqueNPCName()
            };
            npcSpawnDataList_RTEStorage.Add(newData);

            return newData;
        }

        public NPCSchedule_RTEStorage AddNewNPCScheduleData(string npcName)
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

        public void DeleteAllTargetNPCSpawnData(string npcName)
        {
            npcSpawnDataList_RTEStorage.RemoveAll(x => x.npcName == npcName);
        }

        public void DeleteAllTargetNPCScheduleData(string npcName)
        {
            npcScheduleList_RTEStorage.RemoveAll(x => x.npcName == npcName);
        }

        public void DeleteTargetNPCScheduleData(NPCSchedule_RTEStorage targetNPCSchedule)
        {
            npcScheduleList_RTEStorage.Remove(targetNPCSchedule);
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