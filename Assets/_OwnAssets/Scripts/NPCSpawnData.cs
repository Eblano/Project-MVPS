using ProtoBuf;
using System.Collections.Generic;

namespace SealTeam4
{
    [System.Serializable]
    public class NpcSpawnData
    {
        public enum NPC_TYPE { NULL, TYPE0, TYPE1 };
        public NPC_TYPE nPC_TYPE = NPC_TYPE.NULL;
        public string spawnLocation;

        public AIStats aiStats;
        public List<Schedule> nPC_Schedules = new List<Schedule>();
    }
}