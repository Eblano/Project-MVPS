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

        private PropertiesPanel sourcePropertiesPanel;
        private NPCSchedule_RTEStorage localNPCSchedule_RTEStorage;
        private bool scheduleTypeDropdownSetup = false;

        public void Setup
            (
            PropertiesPanel sourcePropertiesPanel,
            NPCSchedule_RTEStorage schedule,
            List<Marker> targetMarkers,
            List<Marker> areaMarkers
            )
        {
            this.sourcePropertiesPanel = sourcePropertiesPanel;
            localNPCSchedule_RTEStorage = schedule;
            Setup_ScheduleTypeDropdown(schedule.scheduleType, schedule.defScheduleTypes);

            Setup_MoveToPosDropdown(targetMarkers);
            Setup_SitInAreaDropdown(areaMarkers);

            //{ "IDLE", "MOVE_TO_POS", "MOVE_TO_POS_WITH_ROT", "SIT_IN_AREA", "TALK_TO_OTHER_NPC" };
            switch (schedule.scheduleType)
            {
                case "IDLE":
                    idleInputField.text = schedule.argument;

                    moveToPosPanel.gameObject.SetActive(false);
                    idlePanel.gameObject.SetActive(true);
                    sitInAreaPanel.gameObject.SetActive(false);
                    break;
                case "MOVE_TO_POS":
                    SetValue_MoveToPosDropdown(schedule.argument);

                    moveToPosPanel.gameObject.SetActive(true);
                    idlePanel.gameObject.SetActive(false);
                    sitInAreaPanel.gameObject.SetActive(false);
                    break;
                case "SIT_IN_AREA":
                    SetValue_SitInAreaDropdown(schedule.argument);

                    moveToPosPanel.gameObject.SetActive(false);
                    idlePanel.gameObject.SetActive(false);
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

                if (areaDropdownOptions.Count > 0)
                    sitInAreaDropdown.AddOptions(areaDropdownOptions);
            }
        }

        private void SetValue_SitInAreaDropdown(string selectedArea)
        {
            int dropdownValue = sitInAreaDropdown.options.FindIndex((i) => { return i.text.Equals(selectedArea); });
            sitInAreaDropdown.value = dropdownValue;
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

                if (moveToPosDropdownOptions.Count > 0)
                    moveToPosDropdown.AddOptions(moveToPosDropdownOptions);
            }
        }

        private void SetValue_MoveToPosDropdown(string selectedWaypoint)
        {
            int dropdownValue = moveToPosDropdown.options.FindIndex((i) => { return i.text.Equals(selectedWaypoint); });
            moveToPosDropdown.value = dropdownValue;
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

            if (scheduleDropdownOptions.Count > 0)
                scheduleTypeDropdown.AddOptions(scheduleDropdownOptions);

            int dropdownValue = scheduleTypeDropdown.options.FindIndex((i) => { return i.text.Equals(selectedSchedule); });
            scheduleTypeDropdown.value = dropdownValue;
        }

        public void OnValueChanged_ScheduleTypeDropdown()
        {
            string newScheduleType = scheduleTypeDropdown.options[scheduleTypeDropdown.value].text;

            if (scheduleTypeDropdownSetup)
            {
                sourcePropertiesPanel.UpdateScheduleType(localNPCSchedule_RTEStorage, newScheduleType);
            }
            else
                scheduleTypeDropdownSetup = true;

            // Update Display Panel
            switch(newScheduleType)
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
                    break;
                case "SIT_IN_AREA":
                    moveToPosPanel.gameObject.SetActive(false);
                    idlePanel.gameObject.SetActive(false);
                    sitInAreaPanel.gameObject.SetActive(true);
                    break;
                case "TALK_TO_OTHER_NPC":
                    break;
            }

            idleInputField.text = "";
            sitInAreaDropdown.value = 0;
            moveToPosDropdown.value = 0;
        }
    }
}
