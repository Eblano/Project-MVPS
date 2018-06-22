using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace SealTeam4
{
    public class NPCListButton : MonoBehaviour
    {
        [SerializeField] private TMP_InputField labelText;
        private Image buttonBGImg;
        private string oldText;
        private Color origColor;

        private void Start()
        {
            buttonBGImg = GetComponentInChildren<Image>();
            origColor = buttonBGImg.color;
        }

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

        public void SetBtnColor()
        {
            buttonBGImg.color = origColor;
        }

        public void SetBtnColor(Color newColor)
        {
            buttonBGImg.color = newColor;
        }
    }
}