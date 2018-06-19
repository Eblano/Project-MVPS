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
        public List<NPCScheduleSlot> npcScheduleSlotList = new List<NPCScheduleSlot>();
        public string npcName;

        private bool spawnMarkerDropdownSetup = false;

        public void Setup
            (
            List<Marker> npcSpawnMarkers,
            List<Marker> targetMarkers,
            List<Marker> areaMarkers,
            NPCSpawnData_RTEStorage npcSpawnData,
            List<NPCSchedule_RTEStorage> schedules,
            GameObject npcScheduleSlot_Prefab
            )
        {
            this.npcScheduleSlot_Prefab = npcScheduleSlot_Prefab;
            npcName = npcSpawnData.npcName;

            Setup_SpawnMarkerDropdown(npcSpawnData.spawnMarkerName, npcSpawnMarkers);
            Setup_ScheduleSlots(schedules, targetMarkers, areaMarkers);

            gameObject.SetActive(false);
        }

        private void Setup_ScheduleSlots
            (
            List<NPCSchedule_RTEStorage> schedules,
            List<Marker> targetMarkers,
            List<Marker> areaMarkers
            )
        {
            foreach(NPCSchedule_RTEStorage schedule in schedules)
            {
                NPCScheduleSlot npcScheduleSlot =
                    Instantiate(npcScheduleSlot_Prefab, Vector3.zero, Quaternion.identity)
                    .GetComponent<NPCScheduleSlot>();

                npcScheduleSlot.transform.SetParent(schedulesPanel.transform);
                npcScheduleSlot.Setup(schedule, targetMarkers, areaMarkers);

                npcScheduleSlotList.Add(npcScheduleSlot);
            }
        }

        public void AddNewSchedule
            (
            NPCSchedule_RTEStorage schedule,
            List<Marker> targetMarkers,
            List<Marker> areaMarkers
            )
        {
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
            foreach (Marker npcSpawnMarker in npcSpawnMarkers)
            {
                TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData
                {
                    text = npcSpawnMarker.markerName
                };

                npcSpawnMarkersDropdownOptions.Add(optionData);
            }
            if (npcSpawnMarkersDropdownOptions.Count > 0)
            {
                spawnMarkerDropdown.AddOptions(npcSpawnMarkersDropdownOptions);
            }

            int dropdownValue = spawnMarkerDropdown.options.FindIndex((i) => { return i.text.Equals(selectedSpawnMarker); });
            spawnMarkerDropdown.value = dropdownValue;
        }

        public void OnValueChanged_SpawnMarkerDropdown()
        {
            if (spawnMarkerDropdownSetup)
            {
                NpcScripting.instance
                    .UpdateNpcSpawnData_SpawnMarker(this, spawnMarkerDropdown.options[spawnMarkerDropdown.value].text);
            }
            else
                spawnMarkerDropdownSetup = true;
        }
	}
}