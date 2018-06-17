using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace SealTeam4
{
	public class InfoPanel : MonoBehaviour 
	{
		[SerializeField] private TMP_Dropdown spawnMarkerDropdown;

		public void Setup(List<Marker> npcSpawnMarkers)
		{
            Setup_SpawnMarkerDropdown(npcSpawnMarkers);
        }

        private void Setup_SpawnMarkerDropdown(List<Marker> npcSpawnMarkers)
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
        }

		public void OnValueChanged_SpawnMarkerDropdown()
		{
            NpcScripting.instance.UpdateNpcSpawnData_SpawnMarker(this, spawnMarkerDropdown.itemText.text);
        }
	}
}