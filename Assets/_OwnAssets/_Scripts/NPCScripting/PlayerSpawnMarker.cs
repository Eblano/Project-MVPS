using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    public class PlayerSpawnMarker : PointMarker, IMarkerBehaviours
    {
        private new void Start()
        {
            base.Start();
            RegisterMarkerOnGameManager(GameManager.MARKER_TYPE.PLAYER_SPAWN_MARKER);
        }
    }
}