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
        [SerializeField] private GameObject npcList;
        [SerializeField] private GameObject rightPanel;

        [Header("Spawning UI Components")]
        [SerializeField] private GameObject npcList_NPCButton_Prefab;
        [SerializeField] private GameObject infoPanel_Prefab;
        
        private GameObject currActiveInfoPanel;

        public List<Marker> npcSpawnMarkers;

        //private class NpcScriptingUIData
        //{
        //	public NPCListButton npcListButton;
        //	public InfoPanel infoPanel;

        //  public NpcSpawnData npcSpawnData;
        //}
        private class NpcScriptingUIData
        {
            public NPCListButton npcListButton;
            public InfoPanel infoPanel;

            public NPCSpawnData_RTEStorage npcSpawnData;
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

        public void ShowNPCScriptingUI()
        {
            npcSpawnMarkers = GameManager.instance.GetAllNPCSpawnMarker();

            npcScriptingUIroot.SetActive(true);

            if (NpcScriptStorage.instance != null)
            {
                npcScriptingUIroot.SetActive(true);
                PopulateDataOnUI(NpcScriptStorage.instance.GetAllNPCSpawnData());
            }
        }

        public void HideNPCScriptingUI()
        {
            foreach(NpcScriptingUIData npcScriptingUIData in npcScriptingUIDataList)
            {
                Destroy(npcScriptingUIData.npcListButton.gameObject);
                Destroy(npcScriptingUIData.infoPanel.gameObject);
            }

            npcScriptingUIDataList.Clear();
            npcScriptingUIroot.SetActive(false);
        }

        private void PopulateDataOnUI(List<NPCSpawnData_RTEStorage> npcSpawnDataList)
        {
            foreach (NPCSpawnData_RTEStorage npcSpawnData in npcSpawnDataList)
            {
                AddNewNPCUIData(npcSpawnData);
            }
        }

        //private void PopulateDataOnUI(List<NpcSpawnData> npcSpawnDataList)
        //{
        //    foreach(NpcSpawnData npcSpawnData in npcSpawnDataList)
        //    {
        //        AddNewNPCUIData(npcSpawnData);
        //    }
        //}

        public void AddNPCEntry()
        {
            AddNewNPCUIData(NpcScriptStorage.instance.AddNewNPCSpawnData());
        }

        private void AddNewNPCUIData(NPCSpawnData_RTEStorage newNpcSpawnData)
        {
            // Spawn npcListButton and InfoPanel
            GameObject npcListButton = Instantiate(npcList_NPCButton_Prefab, Vector3.zero, Quaternion.identity);
            GameObject infoPanel = Instantiate(infoPanel_Prefab, Vector3.zero, Quaternion.identity);
            // Set parent 
            npcListButton.transform.SetParent(npcList.transform);
            infoPanel.transform.SetParent(rightPanel.transform);
            infoPanel.SetActive(false);

            // Create new UIData
            NpcScriptingUIData npsScriptingUIData = new NpcScriptingUIData
            {
                npcListButton = npcListButton.GetComponent<NPCListButton>(),
                infoPanel = infoPanel.GetComponent<InfoPanel>(),
                npcSpawnData = newNpcSpawnData
            };

            // Setup various UI components
            npsScriptingUIData.npcListButton.Setup(newNpcSpawnData.name);
            npsScriptingUIData.infoPanel.Setup(newNpcSpawnData.spawnMarkerName, npcSpawnMarkers);

            // Add UIData to List
            npcScriptingUIDataList.Add(npsScriptingUIData);
        }

        //private void AddNewNPCUIData(NpcSpawnData newNpcSpawnData)
        //{
        //    // Spawn npcListButton and InfoPanel
        //    GameObject npcListButton = Instantiate(npcList_NPCButton_Prefab, Vector3.zero, Quaternion.identity);
        //    GameObject infoPanel = Instantiate(infoPanel_Prefab, Vector3.zero, Quaternion.identity);
        //    // Set parent 
        //    npcListButton.transform.SetParent(npcList.transform);
        //    infoPanel.transform.SetParent(rightPanel.transform);
        //    infoPanel.SetActive(false);

        //    // Create new UIData
        //    NpcScriptingUIData npsScriptingUIData = new NpcScriptingUIData
        //    {
        //        npcListButton = npcListButton.GetComponent<NPCListButton>(),
        //        infoPanel = infoPanel.GetComponent<InfoPanel>(),
        //        npcSpawnData = newNpcSpawnData
        //    };

        //    // Setup various UI components
        //    npsScriptingUIData.npcListButton.Setup(newNpcSpawnData.name);
        //    npsScriptingUIData.infoPanel.Setup(newNpcSpawnData.spawnMarkerName, npcSpawnMarkers);

        //    // Add UIData to List
        //    npcScriptingUIDataList.Add(npsScriptingUIData);
        //}

        public void ShowInfoPanel(NPCListButton sourceButton)
        {
            if(currActiveInfoPanel)
                currActiveInfoPanel.SetActive(false);

            currActiveInfoPanel =
                npcScriptingUIDataList
                .Find(x => x.npcListButton == sourceButton)
                .infoPanel.gameObject;

            currActiveInfoPanel.SetActive(true);
        }

        public void UpdateNpcSpawnData_SpawnMarker(InfoPanel sourceInfoPanel, string newSpawnMarkerValue)
        {
            NPCSpawnData_RTEStorage targetNpcSpawnData =
                npcScriptingUIDataList.Find(x => x.infoPanel == sourceInfoPanel).npcSpawnData;

            NPCSpawnData_RTEStorage newNpcSpawnData = targetNpcSpawnData;
            newNpcSpawnData.spawnMarkerName = newSpawnMarkerValue;

            NpcScriptStorage.instance.UpdateNpcSpawnData(targetNpcSpawnData, newNpcSpawnData);
        }
    }
}