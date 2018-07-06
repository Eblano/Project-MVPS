using Battlehub.RTCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SealTeam4
{
    public class RTEScriptEditor : MonoBehaviour
    {
        public static RTEScriptEditor instance;

        [Header("Editor Components")]
        [SerializeField] private GameObject NPCScriptEditor;
        [SerializeField] private GameObject AccessoriesSetupEditor;
        [SerializeField] private Button NPCScriptEditor_TabBtn;
        [SerializeField] private Button AccessoriesSetupEditor_TabBtn;
        [SerializeField] private GameObject accessoryListPanel;

        [Header("Existing UI Components")]
        [SerializeField] private GameObject scriptEditorRoot;
        [SerializeField] private GameObject npcList;
        [SerializeField] private GameObject rightPanel;

        [Header("Spawning UI Components")]
        [SerializeField] private GameObject npcListButton_Prefab;
        [SerializeField] private GameObject propertiesPanel_Prefab;
        [SerializeField] private GameObject npcScheduleSlot_Prefab;
        [SerializeField] private GameObject accessoryEntry_Prefab;

        [Header("Status Colors")]
        [SerializeField] private Color errorColor;
        private Color origColor = Color.white;

        private GameObject currActivePropertiesPanel;

        private List<NPCSpawnMarker> npcSpawnMarkers;
        private List<WaypointMarker> waypointMarkers;
        private List<AccessoryMarker> accessoryMarkers;
        private List<AreaMarker> areaMarkers;

        private bool allDataIsComplete;

        private class NPCScriptEditorUIData
        {
            public NPCListButton button;
            public PropertiesPanel propertiesPanel;
        }
        // Storing Instantiated UI components
        private List<NPCScriptEditorUIData> npcScriptEditorUIDataList = new List<NPCScriptEditorUIData>();

        private List<AccessoryEntry> accessoryEntryList = new List<AccessoryEntry>();

        private void Start()
        {
            if (instance == null)
                instance = this;
            else
            {
                Debug.Log("Duplicate NpcScriptingInterface detected, deleting..");
                Destroy(gameObject);
            }

            scriptEditorRoot.SetActive(false);

            NPCScriptEditor_TabBtn.onClick.AddListener(delegate { OnNPCScriptEditor_TabBtnClick(); });
            AccessoriesSetupEditor_TabBtn.onClick.AddListener(delegate { OnAccessoriesSetupEditor_TabBtnClick(); });
        }

        private void Update()
        {
            CheckData();
        }

        private void CheckData()
        {
            allDataIsComplete = true;

            foreach (NPCScriptEditorUIData npcScriptingUIData in npcScriptEditorUIDataList)
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

            foreach(AccessoryEntry accessoryEntry in accessoryEntryList)
            {
                bool dataIsComplete = accessoryEntry.CheckData();
                if(!dataIsComplete)
                {
                    allDataIsComplete = false;
                }
            }
        }

        public void ShowScriptEditorUI()
        {
            // Get all markers on scene
            npcSpawnMarkers = GameManager.instance.GetAllNPCSpawnMarkers();
            waypointMarkers = GameManager.instance.GetAllWaypointMarkers();
            areaMarkers = GameManager.instance.GetAllAreaMarkers();
            accessoryMarkers = GameManager.instance.GetAllAccessoryMarkers();

            scriptEditorRoot.SetActive(true);
            
            PopulateData
                (
                ScriptStorage.instance.GetAllNPCSpawnData_SStorage(),
                ScriptStorage.instance.GetAllNPCScheduleData_SStorage(),
                ScriptStorage.instance.GetAllAccessory_SStorage()
                );

            CheckData();
        }

        public void HideScriptEditorUI()
        {
            // Destroy all UI components on UI
            foreach(NPCScriptEditorUIData data in npcScriptEditorUIDataList)
            {
                Destroy(data.button.gameObject);
                Destroy(data.propertiesPanel.gameObject);
            }
            npcScriptEditorUIDataList.Clear();

            foreach(AccessoryEntry accessoryEntry in accessoryEntryList)
            {
                Destroy(accessoryEntry.gameObject);
            }
            accessoryEntryList.Clear();

            currActivePropertiesPanel = null;
            scriptEditorRoot.SetActive(false);

            NPCScriptEditor.transform.SetAsLastSibling();
        }

        public void DeleteAccessoryEntry(AccessoryEntry sourceAccessoryEntry, AccessoryData_SStorage targetAccessoryData)
        {
            accessoryEntryList.Remove(sourceAccessoryEntry);
            Destroy(sourceAccessoryEntry.gameObject);

            ScriptStorage.instance.DeleteAccessoryData_SStorage(targetAccessoryData);
        }

        public void UpdateChanges()
        {
            RuntimeUndo.RecordSelection();
        }

        private void PopulateData(List<NPCSpawnData_SStorage> allNPCSpawnDataList, List<NPCSchedule_SStorage> allNPCScheduleList, List<AccessoryData_SStorage> allAccessoryDataList)
        {
            foreach (NPCSpawnData_SStorage npcSpawnData in allNPCSpawnDataList)
            {
                List<NPCSchedule_SStorage> npcScheduleList = new List<NPCSchedule_SStorage>();

                foreach (NPCSchedule_SStorage npcSchedule in allNPCScheduleList)
                {
                    if (npcSchedule.npcName == npcSpawnData.npcName)
                        npcScheduleList.Add(npcSchedule);
                }

                AddNewNPCData(npcSpawnData, npcScheduleList);
            }

            foreach (AccessoryData_SStorage accessory in allAccessoryDataList)
            {
                AddNewAccessoryData(accessory);
            }
        }

        public void AddNewAccessoryEntry()
        {
            AddNewAccessoryData(ScriptStorage.instance.AddNewAccessoryData_SStorage());
        }

        public void AddNewNPCEntry()
        {
            AddNewNPCData(ScriptStorage.instance.AddNewNPCSpawnData_SStorage(), new List<NPCSchedule_SStorage>());
        }

        public void AddNewNPCSchedule(PropertiesPanel sourcePanel)
        {
            string npcName = sourcePanel.GetNPCName();
            NPCSchedule_SStorage newSchedule = ScriptStorage.instance.AddNewNPCScheduleData_SStorage(npcName);
            sourcePanel.AddNewScheduleSlot(newSchedule, waypointMarkers, areaMarkers);
        }

        private void AddNewAccessoryData(AccessoryData_SStorage accessoryData)
        {
            // Spawn accessory entry panel
            AccessoryEntry accessoryEntryPanel = Instantiate(accessoryEntry_Prefab, accessoryListPanel.transform).GetComponent<AccessoryEntry>();

            // Setup
            accessoryEntryPanel.Setup(ref accessoryData, accessoryMarkers);

            // Add UIData to List
            accessoryEntryList.Add(accessoryEntryPanel);
        }

        private void AddNewNPCData(NPCSpawnData_SStorage newNpcSpawnData, List<NPCSchedule_SStorage> newNpcSchedulesData)
        {
            // Spawn npcListButton, PropertiesPanel
            GameObject npcListButton = Instantiate(npcListButton_Prefab, npcList.transform);
            GameObject propertiesPanel = Instantiate(propertiesPanel_Prefab, rightPanel.transform);

            // Create new UIData
            NPCScriptEditorUIData npsScriptingUIData = new NPCScriptEditorUIData
            {
                button = npcListButton.GetComponent<NPCListButton>(),
                propertiesPanel = propertiesPanel.GetComponent<PropertiesPanel>()
            };

            // Setup various UI components
            npsScriptingUIData.button.Setup(newNpcSpawnData.npcName);
            npsScriptingUIData.propertiesPanel.Setup(
                npcSpawnMarkers,
                waypointMarkers,
                areaMarkers,
                ref newNpcSpawnData,
                ref newNpcSchedulesData,
                npcScheduleSlot_Prefab
                );

            // Add UIData to List
            npcScriptEditorUIDataList.Add(npsScriptingUIData);
        }

        public void ShowPropertiesPanel(NPCListButton sourceButton)
        {
            if(currActivePropertiesPanel)
                currActivePropertiesPanel.SetActive(false);

            currActivePropertiesPanel =
                npcScriptEditorUIDataList
                .Find(x => x.button == sourceButton)
                .propertiesPanel.gameObject;

            currActivePropertiesPanel.SetActive(true);
        }

        public void DeleteNPCEntry(NPCListButton sourceButton)
        {
            NPCScriptEditorUIData targetUIData = npcScriptEditorUIDataList.Find(x => x.button == sourceButton);

            // If active properties panel is target panel
            // Set active panel to null
            if(targetUIData.propertiesPanel == currActivePropertiesPanel)
            {
                currActivePropertiesPanel = null;
            }

            // Delete NPC Data from storage
            ScriptStorage.instance.DeleteAllTargetNPCSpawnData_SStorage(targetUIData.propertiesPanel.GetNPCName());
            ScriptStorage.instance.DeleteNPCScheduleData_SStorage(targetUIData.propertiesPanel.GetNPCName());

            // Destroy all UI elements for target NPC
            Destroy(targetUIData.propertiesPanel.gameObject);
            Destroy(targetUIData.button.gameObject);

            // Delete targetUIData
            npcScriptEditorUIDataList.Remove(targetUIData);
        }

        public void DeleteSchedule(NPCSchedule_SStorage targetSchedule)
        {
            ScriptStorage.instance.DeleteNPCScheduleData_SStorage(targetSchedule);
        }

        public bool DataIsComplete()
        {
            return allDataIsComplete;
        }

        public void OnNPCScriptEditor_TabBtnClick()
        {
            NPCScriptEditor.transform.SetAsLastSibling();
        }

        public void OnAccessoriesSetupEditor_TabBtnClick()
        {
            AccessoriesSetupEditor.transform.SetAsLastSibling();
        }
    }
}