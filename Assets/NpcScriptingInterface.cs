using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    public class NpcScriptingInterface : MonoBehaviour
    {
        [SerializeField] private GameObject npcSpawnInfoStorage_Prefab;
        [SerializeField] private GameObject npcScriptingUIroot;

        [Header("UI Components")]
        [SerializeField] private GameObject npcList;
        [SerializeField] private GameObject npcList_NPCButton_Prefab;

        private void OnEnable()
        {
            Instantiate(npcSpawnInfoStorage_Prefab, Vector3.zero, Quaternion.identity);
        }

        public void ShowNPCScriptingUI()
        {
            Instantiate(npcSpawnInfoStorage_Prefab, Vector3.zero, Quaternion.identity);
            npcScriptingUIroot.SetActive(true);

            PopulateDataOnUI(NpcSpawnInfoStorage.instance.GetAllNPCSpawnData());
        }

        public void HideNPCScriptingUI()
        {
            npcScriptingUIroot.SetActive(false);
        }

        public void PopulateDataOnUI(List<NpcSpawnData> npcSpawnDataList)
        {
            foreach(NpcSpawnData npcSpawnData in npcSpawnDataList)
            {
                GameObject npcSpawnButton = Instantiate(npcList_NPCButton_Prefab, Vector3.zero, Quaternion.identity);
                npcSpawnButton.transform.SetParent(npcList.transform);
                npcSpawnButton.GetComponent<NPCList_NPCButton>().SetText(npcSpawnData.name);
            }
        }
    }
}
