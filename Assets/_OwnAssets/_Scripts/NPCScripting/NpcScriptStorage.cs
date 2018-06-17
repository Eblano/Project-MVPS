using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
	public class NpcScriptStorage : MonoBehaviour 
	{
        private readonly string baseNPCName = "NPC";

		[Battlehub.SerializeIgnore] public static NpcScriptStorage instance;
        
        [SerializeField] private List<NpcSpawnData> npcSpawnDataList = new List<NpcSpawnData>();

        [SerializeField] private bool editNPCScripts = false;

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

        public List<NpcSpawnData> GetAllNPCSpawnData()
        {
            return npcSpawnDataList;
        }

        public NpcSpawnData AddNewNPCSpawnData()
        {
            NpcSpawnData newData = new NpcSpawnData
            {
                name = GetUniqueNPCName()
            };
            npcSpawnDataList.Add(newData);

            return newData;
        }

        public void UpdateNpcSpawnData(NpcSpawnData targetNpcSpawnData, NpcSpawnData newNpcSpawnData)
        {
            npcSpawnDataList[npcSpawnDataList.IndexOf(targetNpcSpawnData)] = newNpcSpawnData;
        }

        public string GetUniqueNPCName()
        {
            int increment = 0;
            
            if(npcSpawnDataList.Count >= 1)
            {
                do
                {
                    increment++;
                }
                while (npcSpawnDataList.Exists(x => x.name == baseNPCName + increment));
            }

            return baseNPCName + increment;
        }
    }
}