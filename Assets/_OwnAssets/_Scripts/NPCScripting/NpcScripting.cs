using System;
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
        [SerializeField] private GameObject npcListButton_Prefab;
        [SerializeField] private GameObject propertiesPanel_Prefab;
        [SerializeField] private GameObject npcScheduleSlot_Prefab;

        private GameObject currActivePropertiesPanel;

        private List<Marker> allNPCSpawnMarkers;
        private List<Marker> allTargetMarkers;
        private List<Marker> allAreaMarkers;

        private class NpcScriptingUIData
        {
            public NPCListButton npcListButton;
            public PropertiesPanel propertiesPanel;

            public NPCSpawnData_RTEStorage npcSpawnData;
            public List<NPCSchedule_RTEStorage> npcSchedules;
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
            allNPCSpawnMarkers = GameManager.instance.GetAllSpecificMarker(GameManager.MARKER_TYPE.NPC_SPAWN);
            allTargetMarkers = GameManager.instance.GetAllSpecificMarker(GameManager.MARKER_TYPE.TARGET);
            allAreaMarkers = GameManager.instance.GetAllSpecificMarker(GameManager.MARKER_TYPE.AREA);

            npcScriptingUIroot.SetActive(true);

            if (NpcScriptStorage.instance != null)
            {
                npcScriptingUIroot.SetActive(true);

                List<NPCSpawnData_RTEStorage> npcSpawnDataList = NpcScriptStorage.instance.GetAllNPCSpawnData();
                List<NPCSchedule_RTEStorage> npcScheduleList = NpcScriptStorage.instance.GetAllNPCScheduleData();
                PopulateDataOnUI(npcSpawnDataList, npcScheduleList);
            }
        }

        public void HideNPCScriptingUI()
        {
            foreach(NpcScriptingUIData npcScriptingUIData in npcScriptingUIDataList)
            {
                Destroy(npcScriptingUIData.npcListButton.gameObject);
                Destroy(npcScriptingUIData.propertiesPanel.gameObject);
            }

            npcScriptingUIDataList.Clear();
            npcScriptingUIroot.SetActive(false);
        }

        private void PopulateDataOnUI(List<NPCSpawnData_RTEStorage> allNPCSpawnDataList, List<NPCSchedule_RTEStorage> allNPCScheduleList)
        {
            foreach (NPCSpawnData_RTEStorage npcSpawnData in allNPCSpawnDataList)
            {
                List<NPCSchedule_RTEStorage> npcScheduleList = new List<NPCSchedule_RTEStorage>();

                foreach (NPCSchedule_RTEStorage npcSchedule in npcScheduleList)
                {
                    if (npcSchedule.name == npcSpawnData.npcName)
                        npcScheduleList.Add(npcSchedule);
                }

                AddNewNPCUIData(npcSpawnData, npcScheduleList);
            }
        }

        public void AddNewNPCEntry()
        {
            AddNewNPCUIData(NpcScriptStorage.instance.AddNewNPCSpawnData(), new List<NPCSchedule_RTEStorage>());
        }

        public void AddNewNPCSchedule()
        {
            PropertiesPanel targetPanel = currActivePropertiesPanel.GetComponent<PropertiesPanel>();
            string npcName = targetPanel.npcName;

            NPCSchedule_RTEStorage newSchedule = NpcScriptStorage.instance.AddNewNPCScheduleData(npcName);

            targetPanel.AddNewSchedule(newSchedule, allTargetMarkers, allAreaMarkers);
        }

        private void AddNewNPCUIData(NPCSpawnData_RTEStorage newNpcSpawnData, List<NPCSchedule_RTEStorage> newNpcSchedulesData)
        {
            // Spawn npcListButton, PropertiesPanel
            GameObject npcListButton = Instantiate(npcListButton_Prefab, Vector3.zero, Quaternion.identity);
            GameObject propertiesPanel = Instantiate(propertiesPanel_Prefab, Vector3.zero, Quaternion.identity);
            // Set parent for above gameobjects
            npcListButton.transform.SetParent(npcList.transform);
            propertiesPanel.transform.SetParent(rightPanel.transform);

            // Create new UIData
            NpcScriptingUIData npsScriptingUIData = new NpcScriptingUIData
            {
                npcListButton = npcListButton.GetComponent<NPCListButton>(),
                propertiesPanel = propertiesPanel.GetComponent<PropertiesPanel>(),
                npcSpawnData = newNpcSpawnData,
                npcSchedules = newNpcSchedulesData
            };

            // Setup various UI components
            npsScriptingUIData.npcListButton.Setup(newNpcSpawnData.npcName);
            npsScriptingUIData.propertiesPanel.Setup(
                allNPCSpawnMarkers,
                allTargetMarkers,
                allAreaMarkers,
                newNpcSpawnData,
                newNpcSchedulesData,
                npcScheduleSlot_Prefab
                );

            // Add UIData to List
            npcScriptingUIDataList.Add(npsScriptingUIData);
        }

        public void ShowPropertiesPanel(NPCListButton sourceButton)
        {
            if(currActivePropertiesPanel)
                currActivePropertiesPanel.SetActive(false);

            currActivePropertiesPanel =
                npcScriptingUIDataList
                .Find(x => x.npcListButton == sourceButton)
                .propertiesPanel.gameObject;

            currActivePropertiesPanel.SetActive(true);
        }

        public void UpdateNpcSpawnData_NPCSchedule(NPCScheduleSlot sourceNPCScheduleSlot, string newScheduleType)
        {
            //NpcScriptStorage.instance.UpdateNpcScheduleData(targetNpcSpawnData, newNpcSpawnData);
        }

        public void UpdateNpcSpawnData_SpawnMarker(PropertiesPanel sourcePropertiesPanel, string newSpawnMarkerValue)
        {
            NPCSpawnData_RTEStorage targetNpcSpawnData =
                npcScriptingUIDataList.Find(x => x.propertiesPanel == sourcePropertiesPanel).npcSpawnData;

            NPCSpawnData_RTEStorage newNpcSpawnData = targetNpcSpawnData;
            newNpcSpawnData.spawnMarkerName = newSpawnMarkerValue;

            NpcScriptStorage.instance.UpdateNpcSpawnData(targetNpcSpawnData, newNpcSpawnData);
        }
    }
}