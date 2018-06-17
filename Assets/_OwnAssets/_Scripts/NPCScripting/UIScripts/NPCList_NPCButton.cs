using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace SealTeam4
{
    public class NPCList_NPCButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI labelText;
        private GameObject infoPanel;

        public void SetText(string text)
        {
            labelText.text = text;
        }

        public void ShowInfoPanel()
        {
            NpcScriptingInterface.instance.DeleteAllInfoPanels();

            infoPanel = Instantiate(NpcScriptingInterface.instance.infoPanel_Prefab, Vector3.zero, Quaternion.identity);
            infoPanel.transform.SetParent(NpcScriptingInterface.instance.rightPanel.transform);

            PopulateInfoPanel(infoPanel.GetComponent<InfoPanel>());
        }

        private void PopulateInfoPanel(InfoPanel infoPanel)
        {
            infoPanel.PopulateInfo(
                NpcScriptingInterface.instance.npcSpawnMarkers
                );
        }
    }
}