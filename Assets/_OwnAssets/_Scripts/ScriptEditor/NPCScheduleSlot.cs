using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

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
        [SerializeField] private TMP_Dropdown waypointMarkerDropdown;
        [SerializeField] private TMP_InputField idleInputField;
        [SerializeField] private TMP_Dropdown talkToNPCPartnerDropdown;
        [SerializeField] private TMP_InputField talkToNPCDurationInputField;
        [SerializeField] private TMP_Dropdown sitInAreaDropdown;
        [SerializeField] private TMP_InputField sitInAreaDurationInputField;

        [Space(10)]

        [SerializeField] private TextMeshProUGUI scheduleOrderTxt;
        [SerializeField] private Button moveUpButton;
        [SerializeField] private Button moveDownButton;

        // Schedule this Schedule Slot is managing
        private NPCSchedule_SStorage ref_schedule;

        private PropertiesPanel parentPropertiesPanel;

        private Image scheduleSlotBGImg;
        [SerializeField] private Color errorColor = Color.grey;
        private Color origColor;

        private void Update()
        {
            UpdateScheduleOrder();
            UpdateTalkToNPCPartnerDropdown();
        }

        public void Setup
            (
            NPCSchedule_SStorage ref_schedule,
            List<WaypointMarker> waypointMarkers,
            List<AreaMarker> areaMarkers,
            PropertiesPanel parentPropertiesPanel
            )
        {
            this.parentPropertiesPanel = parentPropertiesPanel;
            this.ref_schedule = ref_schedule;

            // Add listeners
            scheduleTypeDropdown.onValueChanged.AddListener(delegate { OnValueChanged_ScheduleTypeDropdown(); });
            waypointMarkerDropdown.onValueChanged.AddListener(delegate { OnValueChanged_WaypointMarkerDropdown(); });
            idleInputField.onValueChanged.AddListener(delegate { OnValueChanged_IdleInputField(); });
            talkToNPCDurationInputField.onValueChanged.AddListener(delegate { OnValueChanged_TalkToNPCInputField(); });
            talkToNPCPartnerDropdown.onValueChanged.AddListener(delegate { OnValueChanged_TalkToNPCPartnerDropdown(); });
            sitInAreaDropdown.onValueChanged.AddListener(delegate { OnValueChanged_SitInAreaDropdown(); });
            sitInAreaDurationInputField.onValueChanged.AddListener(delegate { OnValueChanged_SitInAreaDurationInputField(); });
            moveUpButton.onClick.AddListener(delegate { OnButtonClick_MoveUpButton(); });
            moveDownButton.onClick.AddListener(delegate { OnButtonClick_MoveDownButton(); });
            
            // Setup dropdowns
            Setup_ScheduleTypeDropdown(ref_schedule.scheduleType, ref_schedule.GetAllScheduleTypes());
            Setup_WaypointMarkerDropdown(waypointMarkers);
            Setup_SitInAreaDropdown(areaMarkers);
            UpdateTalkToNPCPartnerDropdown();

            // Set panel visibility
            waypointMarkerPanel.gameObject.SetActive(false);
            idlePanel.gameObject.SetActive(false);
            talkToNPCPanel.gameObject.SetActive(false);
            sitInAreaPanel.gameObject.SetActive(false);
            switch (ref_schedule.scheduleType)
            {
                case "Idle":
                    idleInputField.text = ref_schedule.argument_1;
                    idlePanel.gameObject.SetActive(true);
                    break;
                case "Move to Waypoint":
                    SetValue_MoveToPosDropdown(ref_schedule.argument_1);
                    waypointMarkerPanel.gameObject.SetActive(true);
                    break;
                case "Move to Waypoint + Rotate":
                    SetValue_MoveToPosDropdown(ref_schedule.argument_1);
                    waypointMarkerPanel.gameObject.SetActive(true);
                    break;
                case "Sit in Area":
                    SetValue_SitInAreaDropdown(ref_schedule.argument_1);
                    sitInAreaDurationInputField.text = ref_schedule.argument_2;
                    sitInAreaPanel.gameObject.SetActive(true);
                    break;
                case "Talk to other NPC":
                    SetValue_TalkToNPCPartnerDropdown(ref_schedule.argument_1);
                    talkToNPCDurationInputField.text = ref_schedule.argument_2;
                    talkToNPCPanel.gameObject.SetActive(true);
                    break;
            }

            // Backup original bg color
            scheduleSlotBGImg = GetComponent<Image>();
            origColor = scheduleSlotBGImg.color;
        }

        private void UpdateScheduleOrder()
        {
            scheduleOrderTxt.text = (transform.GetSiblingIndex() + 1).ToString();
        }

        private void Setup_SitInAreaDropdown(List<AreaMarker> markers)
        {
            if (markers.Count > 0)
            {
                List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

                TMP_Dropdown.OptionData noneOption = new TMP_Dropdown.OptionData
                {
                    text = "None"
                };
                options.Add(noneOption);

                foreach (AreaMarker marker in markers)
                {
                    TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData
                    {
                        text = marker.name
                    };
                    options.Add(option);
                }

                sitInAreaDropdown.ClearOptions();
                if (options.Count > 0)
                    sitInAreaDropdown.AddOptions(options);
            }
        }

        private void UpdateTalkToNPCPartnerDropdown()
        {
            List<string> npcNames = ScriptStorage.instance.GetAllNPCNames();
            npcNames.Remove(ref_schedule.npcName);

            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

            TMP_Dropdown.OptionData noneOption = new TMP_Dropdown.OptionData
            {
                text = "None"
            };
            options.Add(noneOption);

            foreach (string npcName in npcNames)
            {
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData
                {
                    text = npcName
                };
                options.Add(option);
            }

            talkToNPCPartnerDropdown.ClearOptions();
            if (options.Count > 0)
            {
                talkToNPCPartnerDropdown.AddOptions(options);
                SetValue_TalkToNPCPartnerDropdown(ref_schedule.argument_1);
            }
        }

        public bool CheckData()
        {
            bool dataIsComplete = false;

            if (ref_schedule.scheduleType == "Idle")
            {
                // Checking argument
                if (ref_schedule.argument_1 == null ||
                    ref_schedule.argument_1 == "" ||
                    ref_schedule.argument_1 == "None" ||
                    ref_schedule.argument_1.StartsWith("-") ||
                    float.Parse(ref_schedule.argument_1) > float.MaxValue - 1)
                {
                    scheduleSlotBGImg.color = errorColor;
                    dataIsComplete =  false;
                }
                else
                {
                    scheduleSlotBGImg.color = origColor;
                    dataIsComplete = true;
                }
            }
            else if (ref_schedule.scheduleType == "Move to Waypoint" ||
                ref_schedule.scheduleType == "Move to Waypoint + Rotate")
            {
                // Checking argument
                if (ref_schedule.argument_1 == null ||
                    ref_schedule.argument_1 == "" ||
                    ref_schedule.argument_1 == "None")
                {
                    scheduleSlotBGImg.color = errorColor;
                    dataIsComplete = false;
                }
                else
                {
                    scheduleSlotBGImg.color = origColor;
                    dataIsComplete = true;
                }
            }
            else if(ref_schedule.scheduleType == "Sit in Area" ||
                ref_schedule.scheduleType == "Talk to other NPC")
            {
                // Checking argument
                if (ref_schedule.argument_1 == null ||
                    ref_schedule.argument_2 == null ||

                    ref_schedule.argument_1 == "" ||
                    ref_schedule.argument_2 == "" ||

                    ref_schedule.argument_1 == "None" ||
                    ref_schedule.argument_2 == "None" ||

                    ref_schedule.argument_2.StartsWith("-") ||
                    float.Parse(ref_schedule.argument_2) > float.MaxValue - 1)

                {
                    scheduleSlotBGImg.color = errorColor;
                    dataIsComplete = false;
                }
                else
                {
                    scheduleSlotBGImg.color = origColor;
                    dataIsComplete = true;
                }
            }
            return dataIsComplete;
        }

        private void Setup_WaypointMarkerDropdown(List<WaypointMarker> markers)
        {
            if(markers.Count > 0)
            {
                List<TMP_Dropdown.OptionData> dropdownOptions = new List<TMP_Dropdown.OptionData>();

                TMP_Dropdown.OptionData noneOption = new TMP_Dropdown.OptionData
                {
                    text = "None"
                };
                dropdownOptions.Add(noneOption);

                foreach (WaypointMarker marker in markers)
                {
                    TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData
                    {
                        text = marker.name
                    };

                    dropdownOptions.Add(option);
                }

                waypointMarkerDropdown.ClearOptions();
                if (dropdownOptions.Count > 0)
                    waypointMarkerDropdown.AddOptions(dropdownOptions);
            }
        }

        private void Setup_ScheduleTypeDropdown(string selectedSchedule, string[] scheduleTypes)
        {
            List<TMP_Dropdown.OptionData> dropdownOptions = new List<TMP_Dropdown.OptionData>();

            foreach (string scheduleType in scheduleTypes)
            {
                TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData
                {
                    text = scheduleType
                };
                
                dropdownOptions.Add(optionData);
            }

            scheduleTypeDropdown.ClearOptions();
            
            if (dropdownOptions.Count > 0)
                scheduleTypeDropdown.AddOptions(dropdownOptions);

            int dropdownValue = scheduleTypeDropdown.options.FindIndex((i) => { return i.text.Equals(selectedSchedule); });
            scheduleTypeDropdown.value = dropdownValue;
        }

        private void SetValue_MoveToPosDropdown(string selectedWaypoint)
        {
            int dropdownValue = waypointMarkerDropdown.options.FindIndex((i) => { return i.text.Equals(selectedWaypoint); });
            waypointMarkerDropdown.value = dropdownValue;
        }

        private void SetValue_SitInAreaDropdown(string selectedArea)
        {
            int dropdownValue = sitInAreaDropdown.options.FindIndex((i) => { return i.text.Equals(selectedArea); });
            sitInAreaDropdown.value = dropdownValue;
        }

        private void SetValue_TalkToNPCPartnerDropdown(string selectedNPCPartner)
        {
            int dropdownValue = talkToNPCPartnerDropdown.options.FindIndex((i) => { return i.text.Equals(selectedNPCPartner); });
            talkToNPCPartnerDropdown.value = dropdownValue;
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
            talkToNPCDurationInputField.text = "";
            sitInAreaDropdown.value = 0;
            sitInAreaDurationInputField.text = "";
            waypointMarkerDropdown.value = 0;
        }

        public void OnValueChanged_IdleInputField()
        {
            string inputFieldText = idleInputField.text;

            if (inputFieldText.Length > 4)
            {
                inputFieldText = new string(inputFieldText.Take(4).ToArray());
                idleInputField.text = inputFieldText;
            }

            ref_schedule.argument_1 = inputFieldText;
        }

        public void OnValueChanged_SitInAreaDurationInputField()
        {
            string inputFieldText = sitInAreaDurationInputField.text;
            
            if(inputFieldText.Length > 4)
            {
                inputFieldText = new string(inputFieldText.Take(4).ToArray());
                sitInAreaDurationInputField.text = inputFieldText;
            }

            ref_schedule.argument_2 = inputFieldText;
        }

        public void OnValueChanged_TalkToNPCPartnerDropdown()
        {
            int dropdownValue = talkToNPCPartnerDropdown.value;

            string partnerNPCName = talkToNPCPartnerDropdown.options[dropdownValue].text;

            if(ref_schedule.scheduleType == "Talk to other NPC")
                ref_schedule.argument_1 = partnerNPCName;
        }

        public void OnValueChanged_TalkToNPCInputField()
        {
            string inputFieldText = talkToNPCDurationInputField.text;
            ref_schedule.argument_2 = inputFieldText;
        }

        public void OnValueChanged_WaypointMarkerDropdown()
        {
            int dropdownValue = waypointMarkerDropdown.value;
            
            string targetMarkerName = waypointMarkerDropdown.options[dropdownValue].text;
            ref_schedule.argument_1 = targetMarkerName;
        }

        public void OnValueChanged_SitInAreaDropdown()
        {
            int dropdownValue = sitInAreaDropdown.value;

            string newAreaMarkerName = sitInAreaDropdown.options[dropdownValue].text;
            ref_schedule.argument_1 = newAreaMarkerName;
        }

        public void DeleteSchedule()
        {
            parentPropertiesPanel.DeleteScheduleSlot(this, ref_schedule);
        }

        public void OnButtonClick_MoveUpButton()
        {
            if(transform.GetSiblingIndex() > 0)
            {
                parentPropertiesPanel.MoveScheduleOrder(ref_schedule, ScriptStorage.SCHEDULE_MOVE_DIRECTION.UP);
                transform.SetSiblingIndex(transform.GetSiblingIndex() - 1);
            }
        }

        public void OnButtonClick_MoveDownButton()
        {
            if (transform.parent.childCount - 1 != transform.GetSiblingIndex())
            {
                parentPropertiesPanel.MoveScheduleOrder(ref_schedule, ScriptStorage.SCHEDULE_MOVE_DIRECTION.DOWN);
                transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);
            }
        }
    }
}
