using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace SealTeam4
{
    public class NPCScheduleSlot : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown scheduleTypeDropdown;

        [Space(10)]

        [SerializeField] private GameObject moveToPosPanel;
        [SerializeField] private GameObject idlePanel;
        [SerializeField] private GameObject sitInAreaPanel;

        [Space(10)]

        [SerializeField] private TMP_Dropdown moveToPosDropdown;
        [SerializeField] private TMP_InputField idleInputField;
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
            List<Marker> targetMarkers,
            List<Marker> areaMarkers,
            PropertiesPanel parentPropertiesPanel
            )
        {
            this.ref_schedule = ref_schedule;

            Setup_ScheduleTypeDropdown(ref_schedule.scheduleType, ref_schedule.GetAllScheduleTypes());
            Setup_MoveToPosDropdown(targetMarkers);
            Setup_SitInAreaDropdown(areaMarkers);

            moveToPosPanel.gameObject.SetActive(false);
            idlePanel.gameObject.SetActive(false);
            sitInAreaPanel.gameObject.SetActive(false);
            
            switch (ref_schedule.scheduleType)
            {
                case "Idle":
                    idleInputField.text = ref_schedule.argument;
                    idlePanel.gameObject.SetActive(true);
                    break;
                case "Move to Waypoint":
                    SetValue_MoveToPosDropdown(ref_schedule.argument);
                    moveToPosPanel.gameObject.SetActive(true);
                    break;
                case "Sit in Area":
                    SetValue_SitInAreaDropdown(ref_schedule.argument);
                    sitInAreaPanel.gameObject.SetActive(true);
                    break;
            }

            scheduleSlotBGImg = GetComponent<Image>();
            origColor = scheduleSlotBGImg.color;

            this.parentPropertiesPanel = parentPropertiesPanel;
        }

        private void Setup_SitInAreaDropdown(List<Marker> markers)
        {
            if (markers.Count > 0)
            {
                List<TMP_Dropdown.OptionData> areaDropdownOptions = new List<TMP_Dropdown.OptionData>();

                TMP_Dropdown.OptionData noneOption = new TMP_Dropdown.OptionData
                {
                    text = "None"
                };
                areaDropdownOptions.Add(noneOption);

                foreach (Marker marker in markers)
                {
                    TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData
                    {
                        text = marker.markerName
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

        private void Setup_MoveToPosDropdown(List<Marker> markers)
        {
            if(markers.Count > 0)
            {
                List<TMP_Dropdown.OptionData> moveToPosDropdownOptions = new List<TMP_Dropdown.OptionData>();

                TMP_Dropdown.OptionData noneOption = new TMP_Dropdown.OptionData
                {
                    text = "None"
                };
                moveToPosDropdownOptions.Add(noneOption);

                foreach (Marker marker in markers)
                {
                    TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData
                    {
                        text = marker.markerName
                    };

                    moveToPosDropdownOptions.Add(option);
                }

                moveToPosDropdown.ClearOptions();
                if (moveToPosDropdownOptions.Count > 0)
                    moveToPosDropdown.AddOptions(moveToPosDropdownOptions);
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
            int dropdownValue = moveToPosDropdown.options.FindIndex((i) => { return i.text.Equals(selectedWaypoint); });
            moveToPosDropdown.value = dropdownValue;
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
                    moveToPosPanel.gameObject.SetActive(false);
                    idlePanel.gameObject.SetActive(true);
                    sitInAreaPanel.gameObject.SetActive(false);
                    break;
                case "Move to Waypoint":
                    moveToPosPanel.gameObject.SetActive(true);
                    idlePanel.gameObject.SetActive(false);
                    sitInAreaPanel.gameObject.SetActive(false);
                    break;
                case "Move to Waypoint + Rotate":
                    moveToPosPanel.gameObject.SetActive(true);
                    idlePanel.gameObject.SetActive(false);
                    sitInAreaPanel.gameObject.SetActive(false);
                    break;
                case "Sit in Area":
                    moveToPosPanel.gameObject.SetActive(false);
                    idlePanel.gameObject.SetActive(false);
                    sitInAreaPanel.gameObject.SetActive(true);
                    break;
                case "Talk to other NPC":
                    moveToPosPanel.gameObject.SetActive(false);
                    idlePanel.gameObject.SetActive(false);
                    sitInAreaPanel.gameObject.SetActive(false);
                    break;
            }

            // Update schedule data
            ref_schedule.scheduleType = newScheduleType;

            // Reset all input from all panels
            idleInputField.text = "";
            sitInAreaDropdown.value = 0;
            moveToPosDropdown.value = 0;
        }

        public void OnValueChanged_IdleInputField()
        {
            string inputFieldText = idleInputField.text;
            ref_schedule.argument = inputFieldText;
        }

        public void OnValueChanged_TargetMarkerDropdown()
        {
            int dropdownValue = moveToPosDropdown.value;
            
            string targetMarkerName = moveToPosDropdown.options[dropdownValue].text;
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
