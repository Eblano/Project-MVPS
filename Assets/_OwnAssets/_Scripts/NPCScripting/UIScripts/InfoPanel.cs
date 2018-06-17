using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace SealTeam4
{
	public class InfoPanel : MonoBehaviour 
	{
		[SerializeField] private TMP_Dropdown spawnMarkerDropdown;
		private string npcName;

		public void PopulateInfo(string npcName, List<Marker> npcSpawnMarkers)
		{
			List<TMP_Dropdown.OptionData> npcSpawnMarkersDropdownOptions = new List<TMP_Dropdown.OptionData>();
			foreach(Marker npcSpawnMarker in npcSpawnMarkers)
			{
				TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData();
				optionData.text = npcSpawnMarker.markerName;

				npcSpawnMarkersDropdownOptions.Add(optionData);
			}
			if(npcSpawnMarkersDropdownOptions.Count > 0)
				spawnMarkerDropdown.AddOptions(npcSpawnMarkersDropdownOptions);
		}

		public void OnValueChanged_SpawnMarkerDropdown()
		{
			
		}
	}
}