using Battlehub.RTCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        [Header("Status Colors")]
        [SerializeField] private Color errorColor;
        private Color origColor = Color.white;

        private GameObject currActivePropertiesPanel;

        private List<Marker> npcSpawnMarkers;
        private List<Marker> targetMarkers;
        private List<Marker> areaMarkers;

        private bool allDataIsComplete;

        private class NPCScriptingUIData
        {
            public NPCListButton button;
            public PropertiesPanel propertiesPanel;
        }
        // Storing Instantiated UI components
        private List<NPCScriptingUIData> npcScriptingUIDataList = new List<NPCScriptingUIData>();

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

        private void Update()
        {
            CheckData();
        }

        private void CheckData()
        {
            allDataIsComplete = true;

            foreach (NPCScriptingUIData npcScriptingUIData in npcScriptingUIDataList)
            {
                bool dataIsComplete = npcScriptingUIData.propertiesPanel.CheckData();

                if(!dataIsComplete)
                {
                    allDataIsComplete = false;
                    npcScriptingUIData.button.SetBtnColor(errorColor);
                }
                else
                    npcScriptingUIData.button.SetBtnColor();
            }
        }

        public void ShowNPCScriptingUI()
        {
            // Get all markers on scene
            npcSpawnMarkers = GameManager.instance.GetAllSpecificMarker(GameManager.MARKER_TYPE.NPC_SPAWN);
            targetMarkers = GameManager.instance.GetAllSpecificMarker(GameManager.MARKER_TYPE.TARGET);
            areaMarkers = GameManager.instance.GetAllSpecificMarker(GameManager.MARKER_TYPE.AREA);

            npcScriptingUIroot.SetActive(true);
            
            PopulateData
                (
                NpcScriptStorage.instance.GetAllNPCSpawnData_RTEStorage(),
                NpcScriptStorage.instance.GetAllNPCScheduleData_RTEStorage()
                );
        }

        public void HideNPCScriptingUI()
        {
            // Destroy all UI components on UI
            foreach(NPCScriptingUIData data in npcScriptingUIDataList)
            {
                Destroy(data.button.gameObject);
                Destroy(data.propertiesPanel.gameObject);
            }

            currActivePropertiesPanel = null;
            npcScriptingUIDataList.Clear();
            npcScriptingUIroot.SetActive(false);
            RuntimeUndo.RecordSelection();
        }

        private void PopulateData(List<NPCSpawnData_RTEStorage> allNPCSpawnDataList, List<NPCSchedule_RTEStorage> allNPCScheduleList)
        {
            foreach (NPCSpawnData_RTEStorage npcSpawnData in allNPCSpawnDataList)
            {
                List<NPCSchedule_RTEStorage> npcScheduleList = new List<NPCSchedule_RTEStorage>();

                foreach (NPCSchedule_RTEStorage npcSchedule in allNPCScheduleList)
                {
                    if (npcSchedule.npcName == npcSpawnData.npcName)
                        npcScheduleList.Add(npcSchedule);
                }

                AddNewNPCData(npcSpawnData, npcScheduleList);
            }
        }

        public void AddNewNPCEntry()
        {
            AddNewNPCData(NpcScriptStorage.instance.AddNewNPCSpawnData_RTEStorage(), new List<NPCSchedule_RTEStorage>());
        }

        public void AddNewNPCSchedule(PropertiesPanel sourcePanel)
        {
            string npcName = sourcePanel.GetNPCName();
            NPCSchedule_RTEStorage newSchedule = NpcScriptStorage.instance.AddNewNPCScheduleData_RTEStorage(npcName);
            sourcePanel.AddNewScheduleSlot(newSchedule, targetMarkers, areaMarkers);
        }

        private void AddNewNPCData(NPCSpawnData_RTEStorage newNpcSpawnData, List<NPCSchedule_RTEStorage> newNpcSchedulesData)
        {
            // Spawn npcListButton, PropertiesPanel
            GameObject npcListButton = Instantiate(npcListButton_Prefab, Vector3.zero, Quaternion.identity);
            GameObject propertiesPanel = Instantiate(propertiesPanel_Prefab, Vector3.zero, Quaternion.identity);
            // Set parent for above gameobjects
            npcListButton.transform.SetParent(npcList.transform);
            propertiesPanel.transform.SetParent(rightPanel.transform);

            // Create new UIData
            NPCScriptingUIData npsScriptingUIData = new NPCScriptingUIData
            {
                button = npcListButton.GetComponent<NPCListButton>(),
                propertiesPanel = propertiesPanel.GetComponent<PropertiesPanel>()
            };

            // Setup various UI components
            npsScriptingUIData.button.Setup(newNpcSpawnData.npcName);
            npsScriptingUIData.propertiesPanel.Setup(
                npcSpawnMarkers,
                targetMarkers,
                areaMarkers,
                ref newNpcSpawnData,
                ref newNpcSchedulesData,
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
                .Find(x => x.button == sourceButton)
                .propertiesPanel.gameObject;

            currActivePropertiesPanel.SetActive(true);
        }

        public void DeleteNPCEntry(NPCListButton sourceButton)
        {
            NPCScriptingUIData targetUIData = npcScriptingUIDataList.Find(x => x.button == sourceButton);

            // If active properties panel is target panel
            // Set active panel to null
            if(targetUIData.propertiesPanel == currActivePropertiesPanel)
            {
                currActivePropertiesPanel = null;
            }

            // Delete NPC Data from storage
            NpcScriptStorage.instance.DeleteAllTargetNPCSpawnData_RTEStorage(targetUIData.propertiesPanel.GetNPCName());
            NpcScriptStorage.instance.DeleteNPCScheduleData_RTEStorage(targetUIData.propertiesPanel.GetNPCName());

            // Destroy all UI elements for target NPC
            Destroy(targetUIData.propertiesPanel.gameObject);
            Destroy(targetUIData.button.gameObject);

            // Delete targetUIData
            npcScriptingUIDataList.Remove(targetUIData);
        }

        public void DeleteSchedule(NPCSchedule_RTEStorage targetSchedule)
        {
            NpcScriptStorage.instance.DeleteNPCScheduleData_RTEStorage(targetSchedule);
        }

        public bool DataIsComplete()
        {
            return allDataIsComplete;
        }
    }
}