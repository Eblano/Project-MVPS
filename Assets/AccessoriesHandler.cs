using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class AccessoriesHandler : MonoBehaviour
{
    public static AccessoriesHandler instance;

    [SerializeField] private GameObject accPnlPrefab;

    public enum Item { PISTOL, PISTOL_FANNY, RIFLE_HANDBAG, MAGAZINE }
    public enum Position { L_HAND, R_HAND, L_THIGH, R_THIGH, L_THIGHSIDE, R_THIGHSIDE, STOMACH }

    [SerializeField] private List<Accessory> accessories = new List<Accessory>();
    [SerializeField] private List<AccessoryPanel> accessoryPanels = new List<AccessoryPanel>();
    [SerializeField] private List<AccessorySpawnPosition> accessorySpawnPositions = new List<AccessorySpawnPosition>();
    [SerializeField] private List<string> items = new List<string>();

    public bool setupDone = false;

    private void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        PopulateAccPosSpawnList();
        PopulateAccItemSpawnList();
        ProcessSavedData();
        setupDone = true;
    }

    public List<Accessory> GetPlayerAccessories()
    {
        return accessories;
    }

    public void ProcessSavedData()
    {
        if (!PlayerPrefs.HasKey("NumOfEntries"))
        {
            PlayerPrefs.SetInt("NumOfEntries", 0);
        }

        for (int i = 0; i < PlayerPrefs.GetInt("NumOfEntries"); i++)
        {
            AddAccessoryPanel(PlayerPrefs.GetInt("Entry" + i + "Item"), PlayerPrefs.GetInt("Entry" + i + "Pos"), true);
        }
    }

    public void SaveData()
    {
        PlayerPrefs.SetInt("NumOfEntries", accessories.Count);

        for (int i = 0; i < accessories.Count; i++)
        {
            PlayerPrefs.SetInt("Entry" + i + "Item", accessories[i].GetItem());
            PlayerPrefs.SetInt("Entry" + i + "Pos", accessories[i].GetPosition());
        }
    }

    public void AddAccBtn()
    {
        AddAccessoryPanel(0, 0, false);
    }

    public void AddAccessoryPanel(int item, int position, bool savedData)
    {
        if (accessoryPanels.Count > Enum.GetNames(typeof(Position)).Length - 1)
        {
            return;
        }

        Accessory accessory = new Accessory(item, position);
        GameObject accPanelGO = Instantiate(accPnlPrefab, transform);
        AccessoryPanel accessoryPanel = accPanelGO.GetComponent<AccessoryPanel>();

        accessoryPanel.Initialise(ref accessory, ref accessorySpawnPositions, ref items, savedData);
        accessories.Add(accessory);
        accessoryPanels.Add(accessoryPanel);
    }

    public void RefreshAllDropdown()
    {
        foreach (AccessoryPanel accPnl in accessoryPanels)
        {
            accPnl.RefreshDropDown();
        }

        SaveData();
    }

    private void PopulateAccPosSpawnList()
    {
        // Loops through all entries of Position enum and adds an entry into the accessory spawn position list
        for (int i = 0; i < Enum.GetNames(typeof(Position)).Length; i++)
        {
            // Initialise spawn position for accessory
            AccessorySpawnPosition accessorySpawnPosition = new AccessorySpawnPosition();

            // Set the position enum based on current int value
            accessorySpawnPosition.SetPosition((Position)i);

            // Add the entry into the list
            accessorySpawnPositions.Add(accessorySpawnPosition);
        }
    }

    private void PopulateAccItemSpawnList()
    {
        // Loops through all entries of Item enum and adds an entry into the accessory spawn item list
        for (int i = 0; i < Enum.GetNames(typeof(Item)).Length; i++)
        {
            // Add the entry into the list
            items.Add(Enum.GetName(typeof(Item), (Item)i));
        }
    }

    public void DeletePanelEntry(ref Accessory acc, ref AccessoryPanel accPanel)
    {
        accessories.Remove(acc);
        accessoryPanels.Remove(accPanel);
    }

    public Position GetPositionEnumFromName(string name)
    {
        // Loops through all entries of Position enum and finds an entry that matches the enum name
        for (int i = 0; i < Enum.GetNames(typeof(Position)).Length; i++)
        {
            if (name == Enum.GetName(typeof(Position), (Position)i))
            {
                return (Position)i;
            }
        }

        return Position.L_HAND;
    }
}
