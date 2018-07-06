using Battlehub.RTSaveLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SealTeam4;
using System;

/// <summary>
/// Accessory Marker for accessory spawn point
/// </summary>
namespace SealTeam4
{
    public class AccessoryMarker : PointMarker
    {
        private new void Start()
        {
            base.Start();
            RegisterMarkerOnGameManager(this);
        }
    }
}
