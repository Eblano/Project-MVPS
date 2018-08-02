using ProtoBuf;
using System.Collections.Generic;

namespace SealTeam4
{
    [ProtoContract]
    [System.Serializable]
    public class NpcSpawnData
    {
        public string npcName;
        public enum NPCOutfit { MALE_VIP, MALE_TYPE1, MALE_TYPE2, MALE_TYPE3, FEMALE_TYPE1, FEMALE_TYPE2, FEMALE_TYPE3 };
        public NPCOutfit npcOutfit = NPCOutfit.MALE_TYPE1;
        public string spawnMarkerName;

        public AIStats aiStats = new AIStats();
        public List<NPCSchedule> npcSchedules = new List<NPCSchedule>();
    }
}