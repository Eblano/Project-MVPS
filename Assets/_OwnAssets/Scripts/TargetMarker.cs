using Battlehub.RTSaveLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SealTeam4;
using System;

/// <summary>
/// Target Marker for NPC to move to
/// </summary>
namespace SealTeam4
{
    public class TargetMarker : PointMarker
    {
        private new void Start()
        {
            base.Start();
            RegisterMarkerOnGameManager(GameManager.MARKER_TYPE.TARGET);
        }
    }
}
