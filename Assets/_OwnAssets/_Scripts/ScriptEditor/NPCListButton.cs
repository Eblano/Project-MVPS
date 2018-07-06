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
        [SerializeField] private Image buttonBGImg;
        private Color origColor;
        private string oldText;

        public void Setup(string npcName)
        {
            origColor = buttonBGImg.color;
            oldText = npcName;
            labelText.text = npcName;
        }

        public void ShowPropertiesPanel()
        {
            RTEScriptEditor.instance.ShowPropertiesPanel(this);
        }

        public void DeleteNPC()
        {
            RTEScriptEditor.instance.DeleteNPCEntry(this);
        }

        public void ChangeName()
        {
            bool changeSuccess = ScriptStorage.instance.ChangeName(oldText, labelText.text);

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