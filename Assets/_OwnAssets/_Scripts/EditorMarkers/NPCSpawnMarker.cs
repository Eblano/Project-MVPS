using SealTeam4;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace SealTeam4
{
    /// <summary>
    /// Spawn Marker for NPC
    /// </summary>
    public class NPCSpawnMarker : PointMarker
    {
        private new void Start()
        {
            base.Start();
            gameObject.name = RegisterMarkerOnGameManager(GameManager.MARKER_TYPE.NPCSPAWN);
        }
    }
}
