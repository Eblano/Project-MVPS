using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

namespace SealTeam4
{
	public class PropertiesPanel : MonoBehaviour 
	{
        // a - All NPC Types
        // c - Civillian

        // All Inputs
		[SerializeField] private TMP_Dropdown a_SpawnMarkerDropdown;
        [SerializeField] private TMP_Dropdown a_NPCOutfitDropdown;
        [SerializeField] private TMP_Dropdown a_AITypeDropdown;
        [SerializeField] private GameObject a_SchedulesPanel;
        [SerializeField] private TextMeshProUGUI a_PropertiesSectionText;
        [SerializeField] private Toggle a_ActivateAtSpawnToggle;
        // Civillian Specific
        [SerializeField] private GameObject c_PropertiesPanel;
        [SerializeField] private TMP_Dropdown c_threatRespondBehaviourDropdown;

        // Color when there is error
        [SerializeField] private Color errorColor = Color.red;
        private Color origColor;

        // BG of inputs
        private Image spawnMarkerDropdownBGImg;

        // Prefab of NPC Schedule Slot
        private GameObject npcScheduleSlot_Prefab;

        // List of schedule slots reference that belongs to this properties panel
        [Battlehub.SerializeIgnore] [HideInInspector]
        public List<NPCScheduleSlot> npcScheduleSlotList = new List<NPCScheduleSlot>();

        // Data handled by this properties panel
        private NPCSpawnData_RTEStorage ref_npcSpawnData;
        private List<NPCSchedule_RTEStorage> ref_schedules;

        public void Setup
            (
            List<NPCSpawnMarker> npcSpawnMarkers,
            List<WaypointMarker> waypointMarkers,
            List<AreaMarker> areaMarkers,

            ref NPCSpawnData_RTEStorage ref_npcSpawnData,
            ref List<NPCSchedule_RTEStorage> ref_schedules,
            
            GameObject npcScheduleSlot_Prefab
            )
        {
            this.npcScheduleSlot_Prefab = npcScheduleSlot_Prefab;
            this.ref_npcSpawnData = ref_npcSpawnData;
            this.ref_schedules = ref_schedules;

            // Add listerners
            a_SpawnMarkerDropdown.onValueChanged.AddListener(delegate { OnValueChanged_SpawnMarkerDropdown(); });
            a_SpawnMarkerDropdown.onValueChanged.AddListener(delegate { OnValueChanged_NPCOutfitDropdown(); });
            a_AITypeDropdown.onValueChanged.AddListener(delegate { OnValueChanged_AITypeDropdown(); });
            a_ActivateAtSpawnToggle.onValueChanged.AddListener(delegate { OnValueChanged_ActivateOnSpawnToggle(); });

            Setup_SpawnMarkerDropdown(npcSpawnMarkers);
            Setup_NPCOutfitDropdown();
            Setup_AITypeDropdown();
            Setup_ActivateOnSpawnToggle();
            Setup_ScheduleSlots(waypointMarkers, areaMarkers);

            Setup_C_PropertiesPanel();
            
            spawnMarkerDropdownBGImg = a_SpawnMarkerDropdown.GetComponent<Image>();
            origColor = spawnMarkerDropdownBGImg.color;

            gameObject.SetActive(false);
        }

        private void Update()
        {
            // Update properties panel label
            a_PropertiesSectionText.text = ref_npcSpawnData.npcName + " Properties";
        }

        public bool CheckData()
        {
            bool dataIsComplete = true;

            // Checking SpawnMarkerDropdown
            if (a_SpawnMarkerDropdown.value == 0)
            {
                spawnMarkerDropdownBGImg.color = errorColor;
                dataIsComplete = false;
            }
            else
                spawnMarkerDropdownBGImg.color = origColor;

            // Checking all NPC Schedule slots under this properties panel
            foreach (NPCScheduleSlot slot in npcScheduleSlotList)
            {
                bool npcDataIsComplete = slot.CheckData();
                if (!npcDataIsComplete)
                    dataIsComplete = false;
            }
            if (dataIsComplete)
                return true;
            else
                return false;
        }

        private void Setup_C_PropertiesPanel()
        {
            Setup_C_ThreatRespondBehaviourDropdown();

            if(a_AITypeDropdown.options[a_AITypeDropdown.value].text == "Civillian")
                c_PropertiesPanel.SetActive(true);
            else
                c_PropertiesPanel.SetActive(false);
        }

        private void Setup_C_ThreatRespondBehaviourDropdown()
        {
            c_threatRespondBehaviourDropdown.onValueChanged.AddListener(delegate { OnValueChanged_C_ThreatRespondBehaviourDropdown(); });
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

            // Add threat behaviour texts to dropdown options
            foreach (string option in ref_npcSpawnData.GetAllCivillianStressResponses())
            {
                TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData
                {
                    text = option
                };

                // Add option to dropdown
                options.Add(optionData);
            }

            c_threatRespondBehaviourDropdown.ClearOptions();
            // Add options to dropdown if there is 1 or more options
            if (options.Count > 0)
                c_threatRespondBehaviourDropdown.AddOptions(options);

            string selectedThreatResponseBehaviour = ref_npcSpawnData.threatResponse;

            // Setting dropdown value
            int dropdownValue = c_threatRespondBehaviourDropdown.options.FindIndex((i) => { return i.text.Equals(selectedThreatResponseBehaviour); });
            c_threatRespondBehaviourDropdown.value = dropdownValue;
        }

        private void Setup_ScheduleSlots(List<WaypointMarker> waypointMarkers, List<AreaMarker> areaMarkers)
        {
            foreach (NPCSchedule_RTEStorage schedule in ref_schedules)
            {
                NPCScheduleSlot npcScheduleSlot =
                    Instantiate(npcScheduleSlot_Prefab, Vector3.zero, Quaternion.identity)
                    .GetComponent<NPCScheduleSlot>();

                npcScheduleSlot.transform.SetParent(a_SchedulesPanel.transform);
                npcScheduleSlot.Setup(schedule, waypointMarkers, areaMarkers, this);

                npcScheduleSlotList.Add(npcScheduleSlot);
            }
        }

        public void AddNewScheduleSlot(NPCSchedule_RTEStorage schedule, List<WaypointMarker> waypointMarkers, List<AreaMarker> areaMarkers)
        {
            // Create Schedule Slot Object
            NPCScheduleSlot npcScheduleSlot =
                Instantiate(npcScheduleSlot_Prefab, Vector3.zero, Quaternion.identity)
                .GetComponent<NPCScheduleSlot>();

            npcScheduleSlot.transform.SetParent(a_SchedulesPanel.transform);
            npcScheduleSlot.Setup(schedule, waypointMarkers, areaMarkers, this);

            npcScheduleSlotList.Add(npcScheduleSlot);
        }

        private void Setup_SpawnMarkerDropdown(List<NPCSpawnMarker> npcSpawnMarkers)
        {
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

            // Add marker texts to dropdown options
            foreach (NPCSpawnMarker npcSpawnMarker in npcSpawnMarkers)
            {
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData
                {
                    text = npcSpawnMarker.name
                };

                // Add option to dropdown
                options.Add(option);
            }

            // Add options to dropdown if there is 1 or more options
            if (options.Count > 0)
            {
                a_SpawnMarkerDropdown.AddOptions(options);
            }

            string selectedSpawnMarker = ref_npcSpawnData.spawnMarkerName;

            // Setting dropdown value
            int dropdownValue = a_SpawnMarkerDropdown.options.FindIndex((i) => { return i.text.Equals(selectedSpawnMarker); });
            a_SpawnMarkerDropdown.value = dropdownValue;
        }

        private void Setup_NPCOutfitDropdown()
        {
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

            // Add marker texts to dropdown options
            foreach (string outfit in ref_npcSpawnData.GetAllNPCOutfit())
            {
                TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData
                {
                    text = outfit
                };

                // Add option to dropdown
                options.Add(optionData);
            }

            a_NPCOutfitDropdown.ClearOptions();
            // Add options to dropdown if there is 1 or more options
            if (options.Count > 0)
                a_NPCOutfitDropdown.AddOptions(options);

            string selectedNPCOutfit = ref_npcSpawnData.npcOutfit;

            // Setting dropdown value
            int dropdownValue = a_NPCOutfitDropdown.options.FindIndex((i) => { return i.text.Equals(selectedNPCOutfit); });
            a_NPCOutfitDropdown.value = dropdownValue;
        }

        private void Setup_ActivateOnSpawnToggle()
        {
            a_ActivateAtSpawnToggle.isOn = ref_npcSpawnData.activateOnStart;
        }

        private void Setup_AITypeDropdown()
        {
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

            // Add marker texts to dropdown options
            foreach (string aiType in ref_npcSpawnData.GetAllAITypes())
            {
                TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData
                {
                    text = aiType
                };

                // Add option to dropdown
                options.Add(optionData);
            }

            a_AITypeDropdown.ClearOptions();
            // Add options to dropdown if there is 1 or more options
            if (options.Count > 0)
                a_AITypeDropdown.AddOptions(options);

            string selectedAIType = ref_npcSpawnData.aiType;

            // Setting dropdown value
            int dropdownValue = a_AITypeDropdown.options.FindIndex((i) => { return i.text.Equals(selectedAIType); });
            a_AITypeDropdown.value = dropdownValue;
        }

        public void OnValueChanged_NPCOutfitDropdown()
        {
            int dropdownValue = a_NPCOutfitDropdown.value;

            if (dropdownValue != 0)
            {
                string newNPCOutfitName = a_NPCOutfitDropdown.options[dropdownValue].text;
                ref_npcSpawnData.npcOutfit = newNPCOutfitName;
            }
        }

        public void OnValueChanged_ActivateOnSpawnToggle()
        {
            ref_npcSpawnData.activateOnStart = a_ActivateAtSpawnToggle.isOn;
        }

        public void OnValueChanged_AITypeDropdown()
        {
            int dropdownValue = a_AITypeDropdown.value;

            string newAiType = a_AITypeDropdown.options[dropdownValue].text;
            ref_npcSpawnData.aiType = newAiType;

            // Toggling Specific NPC Type Properties Panel
            c_PropertiesPanel.SetActive(false);
            switch (a_AITypeDropdown.options[dropdownValue].text)
            {
                case "Civillian":
                    c_PropertiesPanel.SetActive(true);
                    break;
            }
        }

        public void OnValueChanged_SpawnMarkerDropdown()
        {
            int dropdownValue = a_SpawnMarkerDropdown.value;
            ref_npcSpawnData.spawnMarkerName = a_SpawnMarkerDropdown.options[dropdownValue].text;
        }

        public void OnValueChanged_C_ThreatRespondBehaviourDropdown()
        {
            int dropdownValue = c_threatRespondBehaviourDropdown.value;
            ref_npcSpawnData.threatResponse = c_threatRespondBehaviourDropdown.options[dropdownValue].text;
        }

        public string GetNPCName()
        {
            return ref_npcSpawnData.npcName;
        }

        public void AddNewNPCSchedule()
        {
            NpcScripting.instance.AddNewNPCSchedule(this);
        }

        public void DeleteScheduleSlot(NPCScheduleSlot sourceNPCScheduleSlot, NPCSchedule_RTEStorage targetSchedule)
        {
            npcScheduleSlotList.Remove(sourceNPCScheduleSlot);
            Destroy(sourceNPCScheduleSlot.gameObject);

            NpcScripting.instance.DeleteSchedule(targetSchedule);
        }

        public void MoveScheduleOrder(NPCSchedule_RTEStorage scheduleToMove, NpcScriptStorage.SCHEDULE_MOVE_DIRECTION moveDir)
        {
            NpcScriptStorage.instance.MoveScheduleOrder(ref_npcSpawnData.npcName, scheduleToMove, moveDir);
        }
	}
}