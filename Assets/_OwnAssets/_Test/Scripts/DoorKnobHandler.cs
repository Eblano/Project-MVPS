﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SealTeam4;

public class DoorKnobHandler : MonoBehaviour
{
    private List<DoorKnob> doorKnobs = new List<DoorKnob>();
    public static DoorKnobHandler instance;

    private void Start()
    {
        if (instance == null)
            instance = this;
    }

    public void AddKnob(DoorKnob doorKnob)
    {
        doorKnobs.Add(doorKnob);
    }

    public void SendKnobState(DoorKnob doorKnobSync, bool state)
    {
        GameManagerAssistant.instance.RelaySenderCmdOpenDoor(doorKnobs.IndexOf(doorKnobSync), state);
    }

    public void SyncKnob(int index, bool isOpen)
    {
        if (isOpen)
        {
            doorKnobs[index].OpenDoors();
        }
        else
        {
            doorKnobs[index].CloseDoors();
        }
    }
}
