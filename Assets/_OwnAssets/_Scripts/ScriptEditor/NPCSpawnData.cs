using ProtoBuf;
using System.Collections.Generic;

namespace SealTeam4
{
    [ProtoContract]
    [System.Serializable]
    public class NpcSpawnData
    {
        public string npcName;
        public enum NPCOutfit { TYPE_1, TYPE_2, TYPE_3 };
        public NPCOutfit npcOutfit = NPCOutfit.TYPE_1;
        public string spawnMarkerName;

        public AIStats aiStats = new AIStats();
        public List<NPCSchedule> npcSchedules = new List<NPCSchedule>();
    }
}