using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace SealTeam4
{
    public class NPCListButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI labelText;

        public void Setup(string npcName)
        {
            labelText.text = npcName;
        }

        public void ShowPropertiesPanel()
        {
            NpcScripting.instance.ShowPropertiesPanel(this);
        }

        public void DeleteNPC()
        {
            NpcScripting.instance.DeleteNPCEntry(this);
        }

    }
}