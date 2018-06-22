using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable]
public class Accessory
{
    public enum Item { DEFAULT, PISTOL, PISTOL_FANNY, RIFLE_HANDBAG, MAGAZINE }
    public enum Position { DEFAULT, L_HAND, R_HAND, L_THIGH, R_THIGH, L_THIGHSIDE, R_THIGHSIDE, STOMACH }

    [SerializeField] private Item spawnItem = Item.DEFAULT;
    [SerializeField] private Position spawnPosition = Position.DEFAULT;

    public Accessory(Item item, Position pos)
    {
        spawnItem = item;
        spawnPosition = pos;
    }

    public int GetItem()
    {
        return (int) spawnItem;
    }

    public int GetPosition()
    {
        return (int) spawnPosition;
    }

    public void SetItem(int item)
    {
        spawnItem = (Item) item;
    }

    public void SetPosition(int pos)
    {
        spawnPosition = (Position) pos;
    }

    public string[] GetItemNames()
    {
        return Enum.GetNames(typeof(Item));
    }

    public string[] GetPositionNames()
    {
        return Enum.GetNames(typeof(Position));
    }
}
