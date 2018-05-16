using SealTeam4;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    /// <summary>
    /// Spawn Marker for NPC
    /// </summary>
    public class SpawnMarker : PointMarker
    {
        private new void Start()
        {
            base.Start();
            RegisterMarkerOnGameManager(GameManager.MARKER_TYPE.NPC_SPAWN);
        }

        /// <summary>
        /// Spawns the NPC on the position of the Marker
        /// </summary>
        public GameObject SpawnNPC(GameObject npc, List<Schedule> npcSchedule, AIStats aiStats)
        {
            GameObject npcGO = Instantiate(npc, transform.position, transform.rotation);
            AIController npcGOAIController = npcGO.GetComponent<AIController>();
            npcGOAIController.SetAIStats(aiStats);
            npcGOAIController.SetSchedule(npcSchedule);
            npcGOAIController.ActivateNPC();
            return npcGO;
        }
    }
}
