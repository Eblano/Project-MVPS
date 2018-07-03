using Battlehub.RTSaveLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SealTeam4;
using System;

/// <summary>
/// Waypoint Marker for NPC to move to
/// </summary>
namespace SealTeam4
{
    public class WaypointMarker : PointMarker
    {
        private new void Start()
        {
            base.Start();
            //gameObject.name = RegisterMarkerOnGameManager(GameManager.MARKER_TYPE.WAYPOINT);
            RegisterMarkerOnGameManager(this);
        }
    }
}
