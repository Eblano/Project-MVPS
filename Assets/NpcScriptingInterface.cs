using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    public class NpcScriptingInterface : MonoBehaviour
    {
        public static NpcScriptingInterface instance;

        [SerializeField] private GameObject npcSpawnInfoStorage_Prefab;

        [Header("Existing UI Components")]
        [SerializeField] private GameObject npcScriptingUIroot;
        public GameObject npcList;
        public GameObject rightPanel;

        [Header("Spawning UI Components")]
        [SerializeField] private GameObject npcList_NPCButton_Prefab;

        private void Start()
        {
            if (instance == null)
                instance = this;
            else
            {
                Debug.Log("Duplicate NpcScriptingInterface detected, deleting..");
                Destroy(gameObject);
            }

            npcScriptingUIroot.SetActive(false);
        }

        private void OnEnable()
        {
            Instantiate(npcSpawnInfoStorage_Prefab, Vector3.zero, Quaternion.identity);
        }

        public void DeleteAllInfoPanels()
        {
            foreach (Transform child in rightPanel.GetComponentsInChildren<Transform>())
            {
                if (child.gameObject != rightPanel)
                {
                    Destroy(child.gameObject);
                }
            }
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

        public void AddNPCEntry()
        {
            GameObject npcSpawnButton = Instantiate(npcList_NPCButton_Prefab, Vector3.zero, Quaternion.identity);
            npcSpawnButton.transform.SetParent(npcList.transform);
            string npcName = NpcSpawnInfoStorage.instance.AddNewNPCSpawnData();
            npcSpawnButton.GetComponent<NPCList_NPCButton>().SetText(npcName);
        }
    }
}
