using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
	public class NpcSpawnInfoStorage : MonoBehaviour 
	{
        private string baseNPCName = "NPC";

		[Battlehub.SerializeIgnore] public static NpcSpawnInfoStorage instance;

        private List<NpcSpawnData> npcSpawnDataList = new List<NpcSpawnData>();

        [SerializeField] private bool editNPCScripts = false;
        [SerializeField] private bool haveInstance = false;

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
            instance = null;
        }

        private void OnDestroy()
        {
            if(instance == this)
                instance = null;
        }

        private void Update()
        {
            if (instance == this)
                haveInstance = true;
            else
                haveInstance = false;

            if (editNPCScripts)
            {
                editNPCScripts = false;
                NpcScriptingInterface.instance.ShowNPCScriptingUI();
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