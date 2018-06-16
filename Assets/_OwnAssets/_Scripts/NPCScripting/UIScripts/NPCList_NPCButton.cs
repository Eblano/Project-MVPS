using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace SealTeam4
{
    public class NPCList_NPCButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI labelText;
        [SerializeField] private GameObject infoPanel_Prefab;
        private GameObject infoPanel;

        public void SetText(string text)
        {
            labelText.text = text;
        }

        public void ShowInfoPanel()
        {
            NpcScriptingInterface.instance.DeleteAllInfoPanels();

            infoPanel = Instantiate(infoPanel_Prefab, Vector3.zero, Quaternion.identity);
            infoPanel.transform.SetParent(NpcScriptingInterface.instance.rightPanel.transform);
        }
    }
}