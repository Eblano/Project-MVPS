using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace SealTeam4
{
    public class NPCListButton : MonoBehaviour
    {
        [SerializeField] private TMP_InputField labelText;
        private string oldText;

        public void Setup(string npcName)
        {
            oldText = npcName;
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

        public void ChangeName()
        {
            bool changeSuccess = NpcScriptStorage.instance.ChangeName(oldText, labelText.text);

            if (changeSuccess)
            {
                oldText = labelText.text;
            }
            else
            {
                labelText.text = oldText;
            }
        }
    }
}