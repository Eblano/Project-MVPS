using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
	public class NpcSpawnInfoStorage : MonoBehaviour 
	{
        [SerializeField] private string baseNPCName = "NPC";

		public static NpcSpawnInfoStorage instance;

        private List<NpcSpawnData> npcSpawnDataList = new List<NpcSpawnData>();

        private void OnDestroy()
        {
            instance = null;
        }

        private void OnDisable()
        {
            instance = null;
        }

        private void OnEnable()
        {
            if (instance == null)
                instance = this;
            else
            {
                Destroy(instance.gameObject);
                instance = this;
                Debug.Log("Duplicated NpcSpawnInfoStorage, deteling original...");
            }
        }

        public List<NpcSpawnData> GetAllNPCSpawnData()
        {
            return npcSpawnDataList;
        }

        public string AddNewNPCSpawnData()
        {
            NpcSpawnData newData = new NpcSpawnData
            {
                name = GetUniqueNPCName()
            };
            npcSpawnDataList.Add(newData);

            return newData.name;
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