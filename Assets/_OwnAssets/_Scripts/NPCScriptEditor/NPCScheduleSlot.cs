using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace SealTeam4
{
    public class NPCScheduleSlot : MonoBehaviour
    {
        [SerializeField] private GameObject waypointMarkerPanel;
        [SerializeField] private GameObject idlePanel;
        [SerializeField] private GameObject sitInAreaPanel;
        [SerializeField] private GameObject talkToNPCPanel;

        [Space(10)]

        [SerializeField] private TMP_Dropdown scheduleTypeDropdown;
        [SerializeField] private TMP_Dropdown WaypointMarkerDropdown;
        [SerializeField] private TMP_InputField idleInputField;
        [SerializeField] private TMP_InputField talkToNPCInputField;
        [SerializeField] private TMP_Dropdown sitInAreaDropdown;

        // Schedule this Schedule Slot is managing
        private NPCSchedule_RTEStorage ref_schedule;

        private PropertiesPanel parentPropertiesPanel;

        private Image scheduleSlotBGImg;
        [SerializeField] private Color errorColor = Color.grey;
        private Color origColor;

        public void Setup
            (
            NPCSchedule_RTEStorage ref_schedule,
            List<WaypointMarker> waypointMarkers,
            List<AreaMarker> areaMarkers,
            PropertiesPanel parentPropertiesPanel
            )
        {
            this.ref_schedule = ref_schedule;

            // Add listeners
            scheduleTypeDropdown.onValueChanged.AddListener(delegate { OnValueChanged_ScheduleTypeDropdown(); });
            WaypointMarkerDropdown.onValueChanged.AddListener(delegate { OnValueChanged_WaypointMarkerDropdown(); });
            idleInputField.onValueChanged.AddListener(delegate { OnValueChanged_IdleInputField(); });
            talkToNPCInputField.onValueChanged.AddListener(delegate { OnValueChanged_TalkToNPCInputField(); });
            sitInAreaDropdown.onValueChanged.AddListener(delegate { OnValueChanged_SitInAreaDropdown(); });

            Setup_ScheduleTypeDropdown(ref_schedule.scheduleType, ref_schedule.GetAllScheduleTypes());
            Setup_WaypointMarkerDropdown(waypointMarkers);
            Setup_SitInAreaDropdown(areaMarkers);

            waypointMarkerPanel.gameObject.SetActive(false);
            idlePanel.gameObject.SetActive(false);
            talkToNPCPanel.gameObject.SetActive(false);
            sitInAreaPanel.gameObject.SetActive(false);
            
            switch (ref_schedule.scheduleType)
            {
                case "Idle":
                    idleInputField.text = ref_schedule.argument;
                    idlePanel.gameObject.SetActive(true);
                    break;
                case "Move to Waypoint":
                    SetValue_MoveToPosDropdown(ref_schedule.argument);
                    waypointMarkerPanel.gameObject.SetActive(true);
                    break;
                case "Move to Waypoint + Rotate":
                    SetValue_MoveToPosDropdown(ref_schedule.argument);
                    waypointMarkerPanel.gameObject.SetActive(true);
                    break;
                case "Sit in Area":
                    SetValue_SitInAreaDropdown(ref_schedule.argument);
                    sitInAreaPanel.gameObject.SetActive(true);
                    break;
                case "Talk to other NPC":
                    talkToNPCInputField.text = ref_schedule.argument;
                    talkToNPCPanel.gameObject.SetActive(true);
                    break;
            }

            scheduleSlotBGImg = GetComponent<Image>();
            origColor = scheduleSlotBGImg.color;

            this.parentPropertiesPanel = parentPropertiesPanel;
        }

        private void Setup_SitInAreaDropdown(List<AreaMarker> markers)
        {
            if (markers.Count > 0)
            {
                List<TMP_Dropdown.OptionData> areaDropdownOptions = new List<TMP_Dropdown.OptionData>();

                TMP_Dropdown.OptionData noneOption = new TMP_Dropdown.OptionData
                {
                    text = "None"
                };
                areaDropdownOptions.Add(noneOption);

                foreach (AreaMarker marker in markers)
                {
                    TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData
                    {
                        text = marker.name
                    };

                    areaDropdownOptions.Add(option);
                }

                sitInAreaDropdown.ClearOptions();
                if (areaDropdownOptions.Count > 0)
                    sitInAreaDropdown.AddOptions(areaDropdownOptions);
            }
        }

        public bool CheckData()
        {
            // Checking argument
            if (
                ref_schedule.argument == "" || 
                ref_schedule.argument == "None" ||
                ref_schedule.argument.StartsWith("-")
                )
            {
                scheduleSlotBGImg.color = errorColor;
                return false;
            }
            else
            {
                scheduleSlotBGImg.color = origColor;
                return true;
            }
        }

        private void Setup_WaypointMarkerDropdown(List<WaypointMarker> markers)
        {
            if(markers.Count > 0)
            {
                List<TMP_Dropdown.OptionData> moveToPosDropdownOptions = new List<TMP_Dropdown.OptionData>();

                TMP_Dropdown.OptionData noneOption = new TMP_Dropdown.OptionData
                {
                    text = "None"
                };
                moveToPosDropdownOptions.Add(noneOption);

                foreach (WaypointMarker marker in markers)
                {
                    TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData
                    {
                        text = marker.name
                    };

                    moveToPosDropdownOptions.Add(option);
                }

                WaypointMarkerDropdown.ClearOptions();
                if (moveToPosDropdownOptions.Count > 0)
                    WaypointMarkerDropdown.AddOptions(moveToPosDropdownOptions);
            }
        }

        private void Setup_ScheduleTypeDropdown(string selectedSchedule, string[] scheduleTypes)
        {
            List<TMP_Dropdown.OptionData> scheduleDropdownOptions = new List<TMP_Dropdown.OptionData>();

            foreach (string scheduleType in scheduleTypes)
            {
                TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData
                {
                    text = scheduleType
                };
                
                scheduleDropdownOptions.Add(optionData);
            }

            scheduleTypeDropdown.ClearOptions();
            
            if (scheduleDropdownOptions.Count > 0)
                scheduleTypeDropdown.AddOptions(scheduleDropdownOptions);

            int dropdownValue = scheduleTypeDropdown.options.FindIndex((i) => { return i.text.Equals(selectedSchedule); });
            scheduleTypeDropdown.value = dropdownValue;
        }

        private void SetValue_MoveToPosDropdown(string selectedWaypoint)
        {
            int dropdownValue = WaypointMarkerDropdown.options.FindIndex((i) => { return i.text.Equals(selectedWaypoint); });
            WaypointMarkerDropdown.value = dropdownValue;
        }

        private void SetValue_SitInAreaDropdown(string selectedArea)
        {
            int dropdownValue = sitInAreaDropdown.options.FindIndex((i) => { return i.text.Equals(selectedArea); });
            sitInAreaDropdown.value = dropdownValue;
        }

        public void OnValueChanged_ScheduleTypeDropdown()
        {
            string newScheduleType = scheduleTypeDropdown.options[scheduleTypeDropdown.value].text;

            // Update Panel Visibility based on schedule type
            switch (newScheduleType)
            {
                case "Idle":
                    waypointMarkerPanel.gameObject.SetActive(false);
                    idlePanel.gameObject.SetActive(true);
                    sitInAreaPanel.gameObject.SetActive(false);
                    talkToNPCPanel.gameObject.SetActive(false);
                    break;
                case "Move to Waypoint":
                    waypointMarkerPanel.gameObject.SetActive(true);
                    idlePanel.gameObject.SetActive(false);
                    sitInAreaPanel.gameObject.SetActive(false);
                    talkToNPCPanel.gameObject.SetActive(false);
                    break;
                case "Move to Waypoint + Rotate":
                    waypointMarkerPanel.gameObject.SetActive(true);
                    idlePanel.gameObject.SetActive(false);
                    sitInAreaPanel.gameObject.SetActive(false);
                    talkToNPCPanel.gameObject.SetActive(false);
                    break;
                case "Sit in Area":
                    waypointMarkerPanel.gameObject.SetActive(false);
                    idlePanel.gameObject.SetActive(false);
                    sitInAreaPanel.gameObject.SetActive(true);
                    talkToNPCPanel.gameObject.SetActive(false);
                    break;
                case "Talk to other NPC":
                    waypointMarkerPanel.gameObject.SetActive(false);
                    idlePanel.gameObject.SetActive(false);
                    sitInAreaPanel.gameObject.SetActive(false);
                    talkToNPCPanel.gameObject.SetActive(true);
                    break;
            }

            // Update schedule data
            ref_schedule.scheduleType = newScheduleType;

            // Reset all input from all panels
            idleInputField.text = "";
            talkToNPCInputField.text = "";
            sitInAreaDropdown.value = 0;
            WaypointMarkerDropdown.value = 0;
        }

        public void OnValueChanged_IdleInputField()
        {
            string inputFieldText = idleInputField.text;
            ref_schedule.argument = inputFieldText;
        }

        public void OnValueChanged_TalkToNPCInputField()
        {
            string inputFieldText = talkToNPCInputField.text;
            ref_schedule.argument = inputFieldText;
        }

        public void OnValueChanged_WaypointMarkerDropdown()
        {
            int dropdownValue = WaypointMarkerDropdown.value;
            
            string targetMarkerName = WaypointMarkerDropdown.options[dropdownValue].text;
            ref_schedule.argument = targetMarkerName;
        }

        public void OnValueChanged_SitInAreaDropdown()
        {
            int dropdownValue = sitInAreaDropdown.value;

            string newAreaMarkerName = sitInAreaDropdown.options[dropdownValue].text;
            ref_schedule.argument = newAreaMarkerName;
        }

        public void DeleteSchedule()
        {
            parentPropertiesPanel.DeleteScheduleSlot(this, ref_schedule);
        }
    }
}
