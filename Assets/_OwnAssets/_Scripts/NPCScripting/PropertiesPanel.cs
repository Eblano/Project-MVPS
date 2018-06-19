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

        private NPCSpawnData_RTEStorage ref_npcSpawnData;
        private List<NPCSchedule_RTEStorage> ref_schedules;

        private bool spawnMarkerDropdownSetup = false;

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

        private void Setup_ScheduleSlots
            (
            List<Marker> targetMarkers,
            List<Marker> areaMarkers
            )
        {
            foreach (NPCSchedule_RTEStorage schedule in ref_schedules)
            {
                NPCScheduleSlot npcScheduleSlot =
                    Instantiate(npcScheduleSlot_Prefab, Vector3.zero, Quaternion.identity)
                    .GetComponent<NPCScheduleSlot>();

                npcScheduleSlot.transform.SetParent(schedulesPanel.transform);
                npcScheduleSlot.Setup(this, schedule, targetMarkers, areaMarkers);

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
            npcScheduleSlot.Setup(this, schedule, targetMarkers, areaMarkers);

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
                string newSpawnMarkerName = spawnMarkerDropdown.options[spawnMarkerDropdown.value].text;
                ref_npcSpawnData.spawnMarkerName = newSpawnMarkerName;
            }
            else
                spawnMarkerDropdownSetup = true;
        }

        public void UpdateScheduleType(NPCSchedule_RTEStorage sourceNPCSchedule_RTEStorage, string newScheduleType)
        {
            Debug.Log(ref_schedules[0].GetHashCode() + " " + sourceNPCSchedule_RTEStorage.GetHashCode());
            ref_schedules.Find(x => x == sourceNPCSchedule_RTEStorage).scheduleType = newScheduleType;
        }

        public string GetNPCName()
        {
            return ref_npcSpawnData.npcName;
        }
	}
}