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

        private bool scheduleTypeDropdownSetup = false;

        public void Setup()
        {
            moveToPosPanel.gameObject.SetActive(false);
            idlePanel.gameObject.SetActive(false);
            sitInAreaPanel.gameObject.SetActive(false);
        }

        private void Setup_ScheduleTypeDropdow(string selectedSchedule, List<string> options)
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
            string scheduleType = scheduleTypeDropdown.options[scheduleTypeDropdown.value].text;

            if (scheduleTypeDropdownSetup)
            {
                NpcScripting.instance
                    .UpdateNpcSpawnData_NPCSchedule(this, scheduleType);
            }
            else
                scheduleTypeDropdownSetup = true;

            switch(scheduleType)
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
        }
    }
}
