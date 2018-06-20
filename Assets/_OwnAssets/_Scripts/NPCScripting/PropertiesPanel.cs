using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace SealTeam4
{
	public class PropertiesPanel : MonoBehaviour 
	{
		[SerializeField] private TMP_Dropdown spawnMarkerDropdown;
        [SerializeField] private GameObject schedulesPanel;

        private GameObject npcScheduleSlot_Prefab;

        // List of schedule slots reference that belongs to this properties panel
        public List<NPCScheduleSlot> npcScheduleSlotList = new List<NPCScheduleSlot>();

        private NPCSpawnData_RTEStorage ref_npcSpawnData;
        private List<NPCSchedule_RTEStorage> ref_schedules;

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

            Setup_SpawnMarkerDropdown(ref_npcSpawnData.spawnMarkerName, npcSpawnMarkers);
            Setup_ScheduleSlots(targetMarkers, areaMarkers);
            
            gameObject.SetActive(false);
        }

        private void Setup_ScheduleSlots(List<Marker> targetMarkers, List<Marker> areaMarkers)
        {
            foreach (NPCSchedule_RTEStorage schedule in ref_schedules)
            {
                NPCScheduleSlot npcScheduleSlot =
                    Instantiate(npcScheduleSlot_Prefab, Vector3.zero, Quaternion.identity)
                    .GetComponent<NPCScheduleSlot>();

                npcScheduleSlot.transform.SetParent(schedulesPanel.transform);
                npcScheduleSlot.Setup(schedule, targetMarkers, areaMarkers);

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
            npcScheduleSlot.Setup(schedule, targetMarkers, areaMarkers);

            npcScheduleSlotList.Add(npcScheduleSlot);
        }

        private void Setup_SpawnMarkerDropdown(string selectedSpawnMarker, List<Marker> npcSpawnMarkers)
        {
            List<TMP_Dropdown.OptionData> npcSpawnMarkersDropdownOptions = new List<TMP_Dropdown.OptionData>();

            // Add marker texts to dropdown options
            foreach (Marker npcSpawnMarker in npcSpawnMarkers)
            {
                TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData
                {
                    text = npcSpawnMarker.markerName
                };

                // Add option to dropdown
                npcSpawnMarkersDropdownOptions.Add(optionData);
            }

            // Add options to dropdown if there is 1 or more options
            if (npcSpawnMarkersDropdownOptions.Count > 0)
            {
                spawnMarkerDropdown.AddOptions(npcSpawnMarkersDropdownOptions);
            }

            // Setting dropdown value
            int dropdownValue = spawnMarkerDropdown.options.FindIndex((i) => { return i.text.Equals(selectedSpawnMarker); });
            spawnMarkerDropdown.value = dropdownValue;
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
	}
}