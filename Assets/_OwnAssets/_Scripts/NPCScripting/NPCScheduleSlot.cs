using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

        public void Setup
            (
            NPCSchedule_RTEStorage ref_schedule,
            List<Marker> targetMarkers,
            List<Marker> areaMarkers
            )
        {
            this.ref_schedule = ref_schedule;

            Setup_ScheduleTypeDropdown(ref_schedule.scheduleType, ref_schedule.GetDefScheduleTypes());
            Setup_MoveToPosDropdown(targetMarkers);
            Setup_SitInAreaDropdown(areaMarkers);

            moveToPosPanel.gameObject.SetActive(false);
            idlePanel.gameObject.SetActive(false);
            sitInAreaPanel.gameObject.SetActive(false);

            //{ "IDLE", "MOVE_TO_POS", "MOVE_TO_POS_WITH_ROT", "SIT_IN_AREA", "TALK_TO_OTHER_NPC" };
            switch (ref_schedule.scheduleType)
            {
                case "IDLE":
                    idleInputField.text = ref_schedule.argument;
                    idlePanel.gameObject.SetActive(true);
                    break;
                case "MOVE_TO_POS":
                    SetValue_MoveToPosDropdown(ref_schedule.argument);
                    moveToPosPanel.gameObject.SetActive(true);
                    break;
                case "SIT_IN_AREA":
                    SetValue_SitInAreaDropdown(ref_schedule.argument);
                    sitInAreaPanel.gameObject.SetActive(true);
                    break;
            }
        }

        private void Setup_SitInAreaDropdown(List<Marker> markers)
        {
            if (markers.Count > 0)
            {
                List<TMP_Dropdown.OptionData> areaDropdownOptions = new List<TMP_Dropdown.OptionData>();
                foreach (Marker marker in markers)
                {
                    TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData
                    {
                        text = marker.markerName
                    };

                    areaDropdownOptions.Add(optionData);
                }

                sitInAreaDropdown.ClearOptions();
                if (areaDropdownOptions.Count > 0)
                    sitInAreaDropdown.AddOptions(areaDropdownOptions);
            }
        }

        private void Setup_MoveToPosDropdown(List<Marker> markers)
        {
            if(markers.Count > 0)
            {
                List<TMP_Dropdown.OptionData> moveToPosDropdownOptions = new List<TMP_Dropdown.OptionData>();
                foreach (Marker marker in markers)
                {
                    TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData
                    {
                        text = marker.markerName
                    };

                    moveToPosDropdownOptions.Add(optionData);
                }

                moveToPosDropdown.ClearOptions();
                if (moveToPosDropdownOptions.Count > 0)
                    moveToPosDropdown.AddOptions(moveToPosDropdownOptions);
            }
        }

        private void Setup_ScheduleTypeDropdown(string selectedSchedule, string[] options)
        {
            List<TMP_Dropdown.OptionData> scheduleDropdownOptions = new List<TMP_Dropdown.OptionData>();
            foreach (string option in options)
            {
                TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData
                {
                    text = option
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
                case "IDLE":
                    moveToPosPanel.gameObject.SetActive(false);
                    idlePanel.gameObject.SetActive(true);
                    sitInAreaPanel.gameObject.SetActive(false);
                    break;
                case "MOVE_TO_POS":
                    moveToPosPanel.gameObject.SetActive(true);
                    idlePanel.gameObject.SetActive(false);
                    sitInAreaPanel.gameObject.SetActive(false);
                    break;
                case "MOVE_TO_POS_WITH_ROT":
                    moveToPosPanel.gameObject.SetActive(true);
                    idlePanel.gameObject.SetActive(false);
                    sitInAreaPanel.gameObject.SetActive(false);
                    break;
                case "SIT_IN_AREA":
                    moveToPosPanel.gameObject.SetActive(false);
                    idlePanel.gameObject.SetActive(false);
                    sitInAreaPanel.gameObject.SetActive(true);
                    break;
                case "TALK_TO_OTHER_NPC":
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

            if (dropdownValue != 0)
            {
                string targetMarkerName = moveToPosDropdown.options[dropdownValue].text;
                ref_schedule.argument = targetMarkerName;
            }
        }

        public void OnValueChanged_SitInAreaDropdown()
        {
            int dropdownValue = sitInAreaDropdown.value;

            if (dropdownValue != 0)
            {
                string newAreaMarkerName = sitInAreaDropdown.options[dropdownValue].text;
                ref_schedule.argument = newAreaMarkerName;
            }
        }

        public void DeleteSchedule()
        {
            NpcScripting.instance.DeleteSchedule(ref_schedule);
            Destroy(this.gameObject);
        }
    }
}
