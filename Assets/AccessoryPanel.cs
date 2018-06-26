using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class AccessoryPanel : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown drpItem;
    [SerializeField] private TMP_Dropdown drpPosition;
    [SerializeField] private Button btnDelete;

    private Accessory accessory;
    private List<AccessorySpawnPosition> accessorySpawnPositions;
    private List<string> items;

    private int prevVal = 0;

    public void Initialise(ref Accessory acc, ref List<AccessorySpawnPosition> accSpwnPos, ref List<string> accItems, bool savedData)
    {
        // Set reference values
        accessory = acc;
        accessorySpawnPositions = accSpwnPos;
        items = accItems;

        // Add listeners
        drpItem.onValueChanged.AddListener(OnDrpItemChanged);
        drpPosition.onValueChanged.AddListener(OnDrpPosChanged);
        btnDelete.onClick.AddListener(OnBtnDelClicked);

        if (!savedData)
        {
            foreach (AccessorySpawnPosition position in accessorySpawnPositions)
            {
                if (!position.IsTaken())
                {
                    accessory.SetEnumPosition(position.GetPositionEnum());
                    break;
                }
            }
        }
        else
        {
            drpItem.value = accessory.GetItem();
        }

        RefreshDropDown();

        OnDrpItemChanged(1);
        OnDrpItemChanged(0);
        OnDrpPosChanged(1);
        OnDrpPosChanged(0);
    }

    public void RefreshDropDown()
    {
        drpItem.ClearOptions();
        drpPosition.ClearOptions();

        foreach (string item in items)
        {
            drpItem.options.Add(new TMP_Dropdown.OptionData() { text = item });
        }

        ProcessUntakenPositions();
        drpPosition.value = 0;
        drpPosition.RefreshShownValue();
        drpItem.RefreshShownValue();
    }

    private void ProcessUntakenPositions()
    {
        string selectedPosition = accessory.GetPositionName();
        // Add currently selected option
        drpPosition.options.Add(new TMP_Dropdown.OptionData() { text = selectedPosition });
        // Set pos as a taken position
        SetTakenStateOfPos(selectedPosition, true);

        foreach (AccessorySpawnPosition position in accessorySpawnPositions)
        {
            // Check if position is taken
            if (position.IsTaken())
            {
                continue;
            }

            drpPosition.options.Add(new TMP_Dropdown.OptionData() { text = position.GetPositionAsString() });
        }
    }

    private void SetTakenStateOfPos(string takenPosName, bool setState)
    {
        foreach (AccessorySpawnPosition position in accessorySpawnPositions)
        {
            // Check if position is same as taken pos name
            if (position.GetPositionAsString() == takenPosName)
            {
                position.SetTakenState(setState);
                return;
            }
        }
    }

    private void OnDrpItemChanged(int value)
    {
        accessory.SetItem(value);
        
        if (AccessoriesHandler.instance.setupDone)
            AccessoriesHandler.instance.RefreshAllDropdown();
    }

    private void OnDrpPosChanged(int value)
    {
        SetTakenStateOfPos(drpPosition.options[0].text, false);
        Debug.Log("Set false, " + drpPosition.options[0].text);
        accessory.SetEnumPosition(AccessoriesHandler.instance.GetPositionEnumFromName(drpPosition.options[drpPosition.value].text));
        SetTakenStateOfPos(drpPosition.options[drpPosition.value].text, true);
        Debug.Log("Set true, " + drpPosition.options[drpPosition.value].text);

        if(AccessoriesHandler.instance.setupDone)
            AccessoriesHandler.instance.RefreshAllDropdown();
    }

    private void OnBtnDelClicked()
    {
        // Add back entry
        string selectedPosition = accessory.GetPositionName();
        // Set pos as a not taken position
        SetTakenStateOfPos(selectedPosition, false);

        AccessoryPanel ap = this;
        AccessoriesHandler.instance.DeletePanelEntry(ref accessory, ref ap);


        if (AccessoriesHandler.instance.setupDone)
            AccessoriesHandler.instance.RefreshAllDropdown();

        Destroy(this.gameObject);
    }
}
