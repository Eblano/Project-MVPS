using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace SealTeam4
{
    public class AccessoryEntry : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown accessoryMarkerDropdown;
        [SerializeField] private TMP_Dropdown accessoryItemDropdown;

        private AccessoryData_SStorage ref_AccessoryData;

        private Image bgImg;
        [SerializeField] private Color errorColor = Color.grey;
        private Color origColor;

        public bool CheckData()
        {
            // Checking argument
            if (
                ref_AccessoryData.accessoryMarker == "" ||
                ref_AccessoryData.accessoryMarker == "None"
                )
            {
                bgImg.color = errorColor;
                return false;
            }
            else
            {
                bgImg.color = origColor;
                return true;
            }
        }

        public void Setup(ref AccessoryData_SStorage ref_AccessoryData, List<AccessoryMarker> accessoryMarkers)
        {
            this.ref_AccessoryData = ref_AccessoryData;

            accessoryMarkerDropdown.onValueChanged.AddListener(delegate { OnValueChanged_AccessoryMarkerDropdown(); });
            accessoryItemDropdown.onValueChanged.AddListener(delegate { OnValueChanged_AccessoryItemDropdown(); });

            Setup_AccessoryMarkerDropdown(accessoryMarkers);
            Setup_AccessoryItemDropdown();

            SetValue_AccessoryMarkerDropdown(ref_AccessoryData.accessoryMarker);
            SetValue_AccessoryItemDropdown(ref_AccessoryData.accessoryItem);

            bgImg = GetComponent<Image>();
            origColor = bgImg.color;
        }

        private void Setup_AccessoryMarkerDropdown(List<AccessoryMarker> markers)
        {
            if (markers.Count > 0)
            {
                List<TMP_Dropdown.OptionData> dropdownOptions = new List<TMP_Dropdown.OptionData>();

                TMP_Dropdown.OptionData noneOption = new TMP_Dropdown.OptionData
                {
                    text = "None"
                };
                dropdownOptions.Add(noneOption);

                foreach (AccessoryMarker marker in markers)
                {
                    TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData
                    {
                        text = marker.name
                    };

                    dropdownOptions.Add(option);
                }

                accessoryMarkerDropdown.ClearOptions();
                if (dropdownOptions.Count > 0)
                    accessoryMarkerDropdown.AddOptions(dropdownOptions);
            }
        }

        private void Setup_AccessoryItemDropdown()
        {
            List<TMP_Dropdown.OptionData> dropdownOptions = new List<TMP_Dropdown.OptionData>();

            foreach (string item in ref_AccessoryData.GetAllAccessoryTypes())
            {
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData
                {
                    text = item
                };

                dropdownOptions.Add(option);
            }

            accessoryItemDropdown.ClearOptions();
            if (dropdownOptions.Count > 0)
                accessoryItemDropdown.AddOptions(dropdownOptions);
        }

        private void SetValue_AccessoryMarkerDropdown(string selectedAccessoryMarker)
        {
            int dropdownValue = accessoryMarkerDropdown.options.FindIndex((i) => { return i.text.Equals(selectedAccessoryMarker); });
            accessoryMarkerDropdown.value = dropdownValue;
        }

        private void SetValue_AccessoryItemDropdown(string selectedAccessoryItem)
        {
            int dropdownValue = accessoryItemDropdown.options.FindIndex((i) => { return i.text.Equals(selectedAccessoryItem); });
            accessoryItemDropdown.value = dropdownValue;
        }

        private void OnValueChanged_AccessoryMarkerDropdown()
        {
            int dropdownValue = accessoryMarkerDropdown.value;

            string markerName = accessoryMarkerDropdown.options[dropdownValue].text;
            ref_AccessoryData.accessoryMarker = markerName;
        }

        private void OnValueChanged_AccessoryItemDropdown()
        {
            int dropdownValue = accessoryItemDropdown.value;

            string item = accessoryItemDropdown.options[dropdownValue].text;
            ref_AccessoryData.accessoryItem = item;
        }

        public void DeleteAccessoryEntry()
        {
            RTEScriptEditor.instance.DeleteAccessoryEntry(this, ref_AccessoryData);
        }
    }
}
