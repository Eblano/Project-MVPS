using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace SealTeam4
{
    public class NPCListButton : MonoBehaviour
    {
        public TextMeshProUGUI labelText;

        public void Setup(string npcName)
        {
            labelText.text = npcName; 
        }

        public void ShowPropertiesPanel()
        {
            NpcScripting.instance.ShowPropertiesPanel(this);
        }
    }
}