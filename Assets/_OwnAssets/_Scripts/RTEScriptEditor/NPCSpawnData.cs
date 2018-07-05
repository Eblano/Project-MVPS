using ProtoBuf;
using System.Collections.Generic;

namespace SealTeam4
{
    [ProtoContract]
    [System.Serializable]
    public class NpcSpawnData
    {
        public string npcName;
        public enum NPCOutfit { TYPE0, TYPE1 };
        public NPCOutfit npcOutfit = NPCOutfit.TYPE0;
        public string spawnMarkerName;

        public AIStats aiStats = new AIStats();
        public List<NPCSchedule> npcSchedules = new List<NPCSchedule>();
    }
}