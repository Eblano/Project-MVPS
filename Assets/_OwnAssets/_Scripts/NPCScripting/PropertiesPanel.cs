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
		[SerializeField] private TMP_Dropdown spawnMarkerDropdown;
        private Image spawnMarkerDropdownBGImg;
        [SerializeField] private TMP_Dropdown npcOutfitDropdown;
        [SerializeField] private TMP_Dropdown aiTypeDropdown;
        [SerializeField] private GameObject schedulesPanel;
        [SerializeField] private TextMeshProUGUI propertiesSectionText;

        private GameObject npcScheduleSlot_Prefab;

        // List of schedule slots reference that belongs to this properties panel
        [Battlehub.SerializeIgnore] [HideInInspector]
        public List<NPCScheduleSlot> npcScheduleSlotList = new List<NPCScheduleSlot>();

        private NPCSpawnData_RTEStorage ref_npcSpawnData;
        private List<NPCSchedule_RTEStorage> ref_schedules;

        [SerializeField] private Color errorColor = Color.red;
        private Color origColor;

        public void Setup
            (
            List<Marker> npcSpawnMarkers,
            List<Marker> targetMarkers,
            List<Marker> areaMarkers,

            ref NPCSpawnData_RTEStorage ref_npcSpawnData,
            ref List<NPCSchedule_RTEStorage> ref_schedules,
            
            GameObject npcScheduleSlot_Prefab
            )
        {
            this.npcScheduleSlot_Prefab = npcScheduleSlot_Prefab;

            this.ref_npcSpawnData = ref_npcSpawnData;
            this.ref_schedules = ref_schedules;

            Setup_SpawnMarkerDropdown(npcSpawnMarkers);
            Setup_NPCOutfitDropdown();
            Setup_AITypeDropdown();
            Setup_ScheduleSlots(targetMarkers, areaMarkers);

            gameObject.SetActive(false);

            spawnMarkerDropdownBGImg = spawnMarkerDropdown.GetComponent<Image>();
            origColor = spawnMarkerDropdownBGImg.color;
        }

        private void Update()
        {
            // Update properties panel label
            propertiesSectionText.text = ref_npcSpawnData.npcName + " Properties";
        }

        public bool CheckData()
        {
            bool dataIsComplete = true;

            // Checking SpawnMarkerDropdown
            if (spawnMarkerDropdown.value == 0)
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

        private void Setup_ScheduleSlots(List<Marker> targetMarkers, List<Marker> areaMarkers)
        {
            foreach (NPCSchedule_RTEStorage schedule in ref_schedules)
            {
                NPCScheduleSlot npcScheduleSlot =
                    Instantiate(npcScheduleSlot_Prefab, Vector3.zero, Quaternion.identity)
                    .GetComponent<NPCScheduleSlot>();

                npcScheduleSlot.transform.SetParent(schedulesPanel.transform);
                npcScheduleSlot.Setup(schedule, targetMarkers, areaMarkers, this);

                npcScheduleSlotList.Add(npcScheduleSlot);
            }
        }

        public void AddNewScheduleSlot(NPCSchedule_RTEStorage schedule, List<Marker> targetMarkers, List<Marker> areaMarkers)
        {
            // Create Schedule Slot Object
            NPCScheduleSlot npcScheduleSlot =
                Instantiate(npcScheduleSlot_Prefab, Vector3.zero, Quaternion.identity)
                .GetComponent<NPCScheduleSlot>();

            npcScheduleSlot.transform.SetParent(schedulesPanel.transform);
            npcScheduleSlot.Setup(schedule, targetMarkers, areaMarkers, this);

            npcScheduleSlotList.Add(npcScheduleSlot);
        }

        private void Setup_SpawnMarkerDropdown(List<Marker> npcSpawnMarkers)
        {
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

            // Add marker texts to dropdown options
            foreach (Marker npcSpawnMarker in npcSpawnMarkers)
            {
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData
                {
                    text = npcSpawnMarker.markerName
                };

                // Add option to dropdown
                options.Add(option);
            }

            // Add options to dropdown if there is 1 or more options
            if (options.Count > 0)
            {
                spawnMarkerDropdown.AddOptions(options);
            }

            string selectedSpawnMarker = ref_npcSpawnData.spawnMarkerName;

            // Setting dropdown value
            int dropdownValue = spawnMarkerDropdown.options.FindIndex((i) => { return i.text.Equals(selectedSpawnMarker); });
            spawnMarkerDropdown.value = dropdownValue;
        }

        private void Setup_NPCOutfitDropdown()
        {
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

            // Add marker texts to dropdown options
            foreach (string outfit in ref_npcSpawnData.GetDefNPCOutfit())
            {
                TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData
                {
                    text = outfit
                };

                // Add option to dropdown
                options.Add(optionData);
            }

            npcOutfitDropdown.ClearOptions();
            // Add options to dropdown if there is 1 or more options
            if (options.Count > 0)
                npcOutfitDropdown.AddOptions(options);

            string selectedNPCOutfit = ref_npcSpawnData.npcOutfit;

            // Setting dropdown value
            int dropdownValue = npcOutfitDropdown.options.FindIndex((i) => { return i.text.Equals(selectedNPCOutfit); });
            npcOutfitDropdown.value = dropdownValue;
        }

        private void Setup_AITypeDropdown()
        {
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

            // Add marker texts to dropdown options
            foreach (string aiType in ref_npcSpawnData.GetDefAITypes())
            {
                TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData
                {
                    text = aiType
                };

                // Add option to dropdown
                options.Add(optionData);
            }

            aiTypeDropdown.ClearOptions();
            // Add options to dropdown if there is 1 or more options
            if (options.Count > 0)
                aiTypeDropdown.AddOptions(options);

            string selectedAIType = ref_npcSpawnData.aiType;

            // Setting dropdown value
            int dropdownValue = aiTypeDropdown.options.FindIndex((i) => { return i.text.Equals(selectedAIType); });
            aiTypeDropdown.value = dropdownValue;
        }

        public void OnValueChanged_NPCOutfitDropdown()
        {
            int dropdownValue = npcOutfitDropdown.value;

            if (dropdownValue != 0)
            {
                string newNPCOutfitName = npcOutfitDropdown.options[dropdownValue].text;
                ref_npcSpawnData.npcOutfit = newNPCOutfitName;
            }
        }

        public void OnValueChanged_AITypeDropdown()
        {
            int dropdownValue = aiTypeDropdown.value;

            if (dropdownValue != 0)
            {
                string newAiType = aiTypeDropdown.options[dropdownValue].text;
                ref_npcSpawnData.aiType = newAiType;
            }
        }

        public void OnValueChanged_SpawnMarkerDropdown()
        {
            int dropdownValue = spawnMarkerDropdown.value;

            if (dropdownValue != 0)
            {
                string newSpawnMarkerName = spawnMarkerDropdown.options[dropdownValue].text;
                ref_npcSpawnData.spawnMarkerName = newSpawnMarkerName;
            }
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
	}
}