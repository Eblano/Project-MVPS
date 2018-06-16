using ProtoBuf;
using System.Collections.Generic;

namespace SealTeam4
{
    [System.Serializable]
    public class NpcSpawnData
    {
        public string name;
        public enum NPC_TYPE { NULL, TYPE0, TYPE1 };
        public NPC_TYPE nPC_TYPE = NPC_TYPE.NULL;
        public string spawnMarkerName;

        public AIStats aiStats;
        public List<NPCSchedule> npcSchedules = new List<NPCSchedule>();
    }
}