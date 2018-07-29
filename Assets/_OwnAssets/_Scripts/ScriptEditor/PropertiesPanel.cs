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
        // t - Terrorist

        [Header("General")]
		[SerializeField] private TMP_Dropdown a_SpawnMarkerDropdown;
        [SerializeField] private TMP_Dropdown a_NPCOutfitDropdown;
        [SerializeField] private TMP_Dropdown a_AITypeDropdown;
        [SerializeField] private GameObject a_SchedulesPanel;
        [SerializeField] private TextMeshProUGUI a_PropertiesSectionText;
        [SerializeField] private Toggle a_ActivateAtSpawnToggle;
        [SerializeField] private TMP_InputField a_MovementSpdInputField;
        
        [Header("Civillain Specific")]
        [SerializeField] private GameObject c_PropertiesPanel;
        [SerializeField] private TMP_Dropdown c_threatRespondBehaviourDropdown;

        [Header("Terrorist Specific")]
        [SerializeField] private GameObject t_PropertiesPanel;
        [SerializeField] private TMP_InputField t_DWPPrefixInputField;

        // Color when there is error
        [SerializeField] private Color errorColor = Color.red;
        private Color origColor;

        // BG of inputs
        private Image spawnMarkerDropdownBGImg;
        private Image movementSpdInputFieldBGImg;

        // Prefab of NPC Schedule Slot
        private GameObject npcScheduleSlot_Prefab;

        // List of schedule slots reference that belongs to this properties panel
        [Battlehub.SerializeIgnore] [HideInInspector]
        public List<NPCScheduleSlot> npcScheduleSlotList = new List<NPCScheduleSlot>();

        // Data handled by this properties panel
        private NPCSpawnData_SStorage ref_npcSpawnData;
        private List<NPCSchedule_SStorage> ref_schedules;

        public void Setup
            (
            List<NPCSpawnMarker> npcSpawnMarkers,
            List<WaypointMarker> waypointMarkers,
            List<AreaMarker> areaMarkers,

            ref NPCSpawnData_SStorage ref_npcSpawnData,
            ref List<NPCSchedule_SStorage> ref_schedules,
            
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
            a_MovementSpdInputField.onValueChanged.AddListener(delegate { OnValueChanged_MovementSpdInputField(); });

            Setup_SpawnMarkerDropdown(npcSpawnMarkers);
            Setup_NPCOutfitDropdown();
            Setup_AITypeDropdown();
            Setup_MovementSpeedInputField();
            Setup_ActivateOnSpawnToggle();
            Setup_ScheduleSlots(waypointMarkers, areaMarkers);

            // Setup Sub Properties Panel
            Setup_C_PropertiesPanel();
            Setup_T_PropertiesPanel();
            
            spawnMarkerDropdownBGImg = a_SpawnMarkerDropdown.GetComponent<Image>();
            origColor = spawnMarkerDropdownBGImg.color;
            movementSpdInputFieldBGImg = a_MovementSpdInputField.GetComponent<Image>();
            origColor = movementSpdInputFieldBGImg.color;

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

            if(ref_npcSpawnData.movementSpdMultiplier < 1f || ref_npcSpawnData.movementSpdMultiplier > 5f)
            {
                movementSpdInputFieldBGImg.color = errorColor;
                dataIsComplete = false;
            }
            else
                movementSpdInputFieldBGImg.color = origColor;

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

        private void Setup_T_PropertiesPanel()
        {
            Setup_T_DWPPrefixInputField();
            
            if (a_AITypeDropdown.options[a_AITypeDropdown.value].text == "Terrorist")
                t_PropertiesPanel.SetActive(true);
            else
                t_PropertiesPanel.SetActive(false);
        }

        private void Setup_T_DWPPrefixInputField()
        {
            t_DWPPrefixInputField.onValueChanged.AddListener(delegate { OnValueChanged_T_DWPPrefixInputField(); });

            t_DWPPrefixInputField.text = ref_npcSpawnData.dynamicWaypointPrefix;
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

            string selectedThreatResponseBehaviour = ref_npcSpawnData.civillianThreatResponse;

            // Setting dropdown value
            int dropdownValue = c_threatRespondBehaviourDropdown.options.FindIndex((i) => { return i.text.Equals(selectedThreatResponseBehaviour); });
            c_threatRespondBehaviourDropdown.value = dropdownValue;
        }

        private void Setup_ScheduleSlots(List<WaypointMarker> waypointMarkers, List<AreaMarker> areaMarkers)
        {
            foreach (NPCSchedule_SStorage schedule in ref_schedules)
            {
                NPCScheduleSlot npcScheduleSlot =
                    Instantiate(npcScheduleSlot_Prefab, Vector3.zero, Quaternion.identity)
                    .GetComponent<NPCScheduleSlot>();

                npcScheduleSlot.transform.SetParent(a_SchedulesPanel.transform);
                npcScheduleSlot.Setup(schedule, waypointMarkers, areaMarkers, this);

                npcScheduleSlotList.Add(npcScheduleSlot);
            }
        }

        public void AddNewScheduleSlot(NPCSchedule_SStorage schedule, List<WaypointMarker> waypointMarkers, List<AreaMarker> areaMarkers)
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

        private void Setup_MovementSpeedInputField()
        {
            a_MovementSpdInputField.text = ref_npcSpawnData.movementSpdMultiplier.ToString();
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

        private void OnValueChanged_NPCOutfitDropdown()
        {
            int dropdownValue = a_NPCOutfitDropdown.value;

            if (dropdownValue != 0)
            {
                string newNPCOutfitName = a_NPCOutfitDropdown.options[dropdownValue].text;
                ref_npcSpawnData.npcOutfit = newNPCOutfitName;
            }
        }

        private void OnValueChanged_ActivateOnSpawnToggle()
        {
            ref_npcSpawnData.activateOnStart = a_ActivateAtSpawnToggle.isOn;
        }

        private void OnValueChanged_MovementSpdInputField()
        {
            if(a_MovementSpdInputField.text.Length > 0)
            {
                float movementSpd = float.Parse(a_MovementSpdInputField.text);
                ref_npcSpawnData.movementSpdMultiplier = movementSpd;
            }
            else
                ref_npcSpawnData.movementSpdMultiplier = 1;
        }

        private void OnValueChanged_AITypeDropdown()
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
                    t_PropertiesPanel.SetActive(false);
                    break;

                case "Terrorist":
                    t_PropertiesPanel.SetActive(true);
                    c_PropertiesPanel.SetActive(false);
                    break;

                case "VIP":
                    t_PropertiesPanel.SetActive(false);
                    c_PropertiesPanel.SetActive(false);
                    break;
            }
        }

        private void OnValueChanged_SpawnMarkerDropdown()
        {
            int dropdownValue = a_SpawnMarkerDropdown.value;
            ref_npcSpawnData.spawnMarkerName = a_SpawnMarkerDropdown.options[dropdownValue].text;
        }

        private void OnValueChanged_C_ThreatRespondBehaviourDropdown()
        {
            int dropdownValue = c_threatRespondBehaviourDropdown.value;
            ref_npcSpawnData.civillianThreatResponse = c_threatRespondBehaviourDropdown.options[dropdownValue].text;
        }

        private void OnValueChanged_T_DWPPrefixInputField()
        {
            ref_npcSpawnData.dynamicWaypointPrefix = t_DWPPrefixInputField.text;
        }

        public string GetNPCName()
        {
            return ref_npcSpawnData.npcName;
        }

        public void AddNewNPCSchedule()
        {
            RTEScriptEditor.instance.AddNewNPCSchedule(this);
        }

        public void DeleteScheduleSlot(NPCScheduleSlot sourceNPCScheduleSlot, NPCSchedule_SStorage targetSchedule)
        {
            npcScheduleSlotList.Remove(sourceNPCScheduleSlot);
            Destroy(sourceNPCScheduleSlot.gameObject);

            RTEScriptEditor.instance.DeleteSchedule(targetSchedule);
        }

        public void MoveScheduleOrder(NPCSchedule_SStorage scheduleToMove, ScriptStorage.SCHEDULE_MOVE_DIRECTION moveDir)
        {
            ScriptStorage.instance.MoveScheduleOrder(ref_npcSpawnData.npcName, scheduleToMove, moveDir);
        }
	}
}