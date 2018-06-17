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
        private GameObject infoPanel;

        public void SetButtonText(string text)
        {
            labelText.tag = text;
        }

        public void Setup(string npcName)
        {
            labelText.text = npcName;

            // Setup info panel
            NpcScripting.instance.DeleteAllInfoPanels();

			infoPanel = Instantiate(NpcScripting.instance.infoPanel_Prefab, Vector3.zero, Quaternion.identity);
			infoPanel.transform.SetParent(NpcScripting.instance.rightPanel.transform);;
            PopulateInfoPanel(npcName, infoPanel.GetComponent<InfoPanel>());
        }

        private void PopulateInfoPanel(string npcName, InfoPanel infoPanel)
        {
            infoPanel.PopulateInfo(
                npcName,
				NpcScripting.instance.npcSpawnMarkers
                );
        }
    }
}