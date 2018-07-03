using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable]
public class Accessory
{
    [SerializeField] private AccessoriesHandler.Item spawnItem = AccessoriesHandler.Item.PISTOL;
    [SerializeField] private AccessoriesHandler.Position spawnPosition = AccessoriesHandler.Position.L_HAND;

    public Accessory(int item, int pos)
    {
        spawnItem = (AccessoriesHandler.Item)item;
        spawnPosition = (AccessoriesHandler.Position)pos;
    }

    public int GetItem()
    {
        return (int)spawnItem;
    }

    public int GetPosition()
    {
        return (int)spawnPosition;
    }

    public void SetItem(int item)
    {
        spawnItem = (AccessoriesHandler.Item)item;
    }

    public void SetPosition(int pos)
    {
        spawnPosition = (AccessoriesHandler.Position)pos;
    }

    public void SetEnumPosition(AccessoriesHandler.Position pos)
    {
        spawnPosition = pos;
    }

    public string GetPositionName()
    {
        return Enum.GetName(typeof(AccessoriesHandler.Position), spawnPosition);
    }
}
