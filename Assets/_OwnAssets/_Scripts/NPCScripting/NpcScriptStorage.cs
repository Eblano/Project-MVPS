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

        public NPCSpawnData_RTEStorage AddNewNPCSpawnData()
        {
            NPCSpawnData_RTEStorage newData = new NPCSpawnData_RTEStorage
            {
                name = GetUniqueNPCName()
            };
            npcSpawnDataList_RTEStorage.Add(newData);

            return newData;
        }

        public void UpdateNpcSpawnData(NPCSpawnData_RTEStorage targetNpcSpawnData, NPCSpawnData_RTEStorage newNpcSpawnData)
        {
            npcSpawnDataList_RTEStorage[npcSpawnDataList_RTEStorage.IndexOf(targetNpcSpawnData)] = newNpcSpawnData;
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
                while (npcSpawnDataList_RTEStorage.Exists(x => x.name == baseNPCName + increment));
            }

            return baseNPCName + increment;
        }
    }
}

[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllPublic)]
[System.Serializable]
public class NPCSpawnData_RTEStorage
{
    public string name;
    public string[] defNPCTypes = { "NULL", "TYPE0", "TYPE1", "SIT_IN_AREA" };
    public string NPCType;
    public string spawnMarkerName;

    public AIStats aiStats;
}

[ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllPublic)]
[System.Serializable]
public class NPCSchedule_RTEStorage
{
    public string name;
    public string[] defScheduleTypes = { "IDLE", "MOVE_TO_POS", "MOVE_TO_POS_WITH_ROT", "SIT_IN_AREA", "TALK_TO_OTHER_NPC" };
    public string scheduleType;
    public string argument;
}