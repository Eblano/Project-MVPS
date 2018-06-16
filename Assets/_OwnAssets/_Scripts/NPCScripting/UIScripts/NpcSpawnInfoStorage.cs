using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
	public class NpcSpawnInfoStorage : MonoBehaviour 
	{
		public static NpcSpawnInfoStorage instance;

        private List<NpcSpawnData> npcSpawnDataList;

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
                Debug.Log("Duplicated NpcSpawnInfoStorage, deteling..");
            }
        }

        public List<NpcSpawnData> GetAllNPCSpawnData()
        {
            return npcSpawnDataList;
        }
    }
}