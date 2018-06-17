using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    public class NpcScripting : MonoBehaviour
    {
        public static NpcScripting instance;
        
        [Header("Existing UI Components")]
        [SerializeField] private GameObject npcScriptingUIroot;
        public GameObject npcList;
        public GameObject rightPanel;

        [Header("Spawning UI Components")]
        [SerializeField] private GameObject npcList_NPCButton_Prefab;
        public GameObject infoPanel_Prefab;

        public List<Marker> npcSpawnMarkers;

		public class NpcScriptingUIData
		{
			public NPCListButton npcListButton;
			public InfoPanel infoPanel;

			public NpcSpawnData npcSpawnData;
		}
		private List<NpcScriptingUIData> npcScriptingUIDataList = new List<NpcScriptingUIData>();

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
            npcSpawnMarkers = GameManager.instance.GetAllNPCSpawnMarker();

            if(NpcScriptStorage.instance != null)
            {
                npcScriptingUIroot.SetActive(true);
                PopulateDataOnUI(NpcScriptStorage.instance.GetAllNPCSpawnData());
            }
        }

        public void HideNPCScriptingUI()
        {
            npcScriptingUIroot.SetActive(false);
        }

        public void PopulateDataOnUI(List<NpcSpawnData> npcSpawnDataList)
        {
            foreach(NpcSpawnData npcSpawnData in npcSpawnDataList)
            {
				GameObject npcListButton = Instantiate(npcList_NPCButton_Prefab, Vector3.zero, Quaternion.identity);
                
				NpcScriptingUIData npsScriptingUIData = new NpcScriptingUIData();
                
				npsScriptingUIData.npcListButton = npcListButton.GetComponent<NPCListButton>();
                npsScriptingUIData.npcListButton.SetButtonText(npcSpawnData.name);
				npsScriptingUIData.npcListButton.transform.SetParent(npcList.transform);

				GameObject infoPanel = Instantiate(NpcScripting.instance.infoPanel_Prefab, Vector3.zero, Quaternion.identity);
				npsScriptingUIData.infoPanel = infoPanel.GetComponent<InfoPanel>();
				npsScriptingUIData.infoPanel.transform.SetParent(NpcScripting.instance.rightPanel.transform);

                npcScriptingUIDataList.Add(npsScriptingUIData);
            }
        }

        public void AddNPCEntry()
        {
            GameObject npcSpawnButton = Instantiate(npcList_NPCButton_Prefab, Vector3.zero, Quaternion.identity);
            npcSpawnButton.transform.SetParent(npcList.transform);
            string npcName = NpcScriptStorage.instance.AddNewNPCSpawnData();

			NPCListButton button = GetComponent<NPCListButton>();
        }
    }
}