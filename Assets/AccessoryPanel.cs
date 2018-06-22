using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class AccessoryPanel : MonoBehaviour
{
    private Accessory accessory;
    [SerializeField] private TMP_Dropdown drpItem;
    [SerializeField] private TMP_Dropdown drpPosition;
    [SerializeField] private Button btnDelete;

    public void InitialisePanel(ref Accessory accRef, Transform parentTrans)
    {
        transform.SetParent(parentTrans);
        accessory = accRef;
        PopulateDropdownOptions();
        btnDelete.onClick.AddListener(DestroyPanel);
        drpItem.onValueChanged.AddListener(UpdateItemDrpDown);
        drpPosition.onValueChanged.AddListener(UpdatePositionDrpDown);
    }

    private void UpdateItemDrpDown(int value)
    {
        drpItem.value = value;
        accessory.SetItem(value);
    }

    private void UpdatePositionDrpDown(int value)
    {
        drpPosition.value = value;
        accessory.SetPosition(value);
    }

    private void PopulateDropdownOptions()
    {
        drpItem.ClearOptions();
        drpPosition.ClearOptions();

        foreach (string item in accessory.GetItemNames())
        {
            TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData
            {
                text = item
            };

            drpItem.options.Add(optionData);
        }

        foreach (string position in accessory.GetPositionNames())
        {
            TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData
            {
                text = position
            };

            drpPosition.options.Add(optionData);
        }

        drpItem.value = 1;
        drpPosition.value = 1;

        drpItem.value = 0;
        drpPosition.value = 0;
    }

    private void DestroyPanel()
    {
        AccessoriesHandler.instance.OnDeleteAccessory(accessory);
        Destroy(this.gameObject);
    }
}
