using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    public class NPCScripting : MonoBehaviour
    {
        [SerializeField] private GameObject npcList;
        [SerializeField] private GameObject npcList_Entry_Prefab;

        public void Add_NPCList_Entry()
        {
            GameObject go = Instantiate(npcList_Entry_Prefab, Vector3.zero, Quaternion.identity);
            go.transform.SetParent(npcList.transform);
        }
    }
}