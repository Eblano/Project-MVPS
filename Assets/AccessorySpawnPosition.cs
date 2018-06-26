using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AccessorySpawnPosition
{
    [SerializeField] private AccessoriesHandler.Position position = AccessoriesHandler.Position.L_HAND;
    [SerializeField] private bool taken = false;

    public void SetPosition(AccessoriesHandler.Position pos)
    {
        position = pos;
    }

    public void SetTakenState(bool state)
    {
        taken = state;
    }

    public bool IsTaken()
    {
        return taken;
    }

    public string GetPositionAsString()
    {
        return Enum.GetName(typeof(AccessoriesHandler.Position), position);
    }

    public AccessoriesHandler.Position GetPositionEnum()
    {
        return position;
    }
}
