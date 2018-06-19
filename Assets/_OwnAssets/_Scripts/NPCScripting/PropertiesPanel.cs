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

        private bool spawnMarkerDropdownSetup = false;

		public void Setup(string selectedSpawnMarker, List<Marker> npcSpawnMarkers)
		{
            Setup_SpawnMarkerDropdown(selectedSpawnMarker, npcSpawnMarkers);
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
                spawnMarkerDropdown.AddOptions(npcSpawnMarkersDropdownOptions);

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