using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SealTeam4
{
    public class PlayerContainer : MonoBehaviour
    {
        [Header("UI Components")]
        public TextMeshProUGUI playerNameTxt;
        [SerializeField] private Button vipFollowTargetBtn;
        [SerializeField] private bool vipFollowTargetBtn_ToggleOn = false;

        [Header("Colors")]
        [SerializeField] private Color unselectedColor = Color.grey;
        [SerializeField] private Color vipFollowTargetBtnColor = Color.red;

        public void Setup(string playerName)
        {
            playerNameTxt.text = playerName;

            vipFollowTargetBtn.image.color = unselectedColor;

            vipFollowTargetBtn.onClick.AddListener(delegate { OnClick_VIPFollowTargetBtn(); });
        }

        public void SetVIPFollowButtonState(bool state)
        {
            if (state)
            {
                vipFollowTargetBtn.image.color = vipFollowTargetBtnColor;
                InterfaceManager.instance.UnToggleAllPlayerContainerVIPFollowTarget(this);
                GameManager.instance.SetVIPFollowTarget(playerNameTxt.text);
            }
            else
                vipFollowTargetBtn.image.color = unselectedColor;

            vipFollowTargetBtn_ToggleOn = state;
        }

        private void OnClick_VIPFollowTargetBtn()
        {
            if(!vipFollowTargetBtn_ToggleOn)
            {
                vipFollowTargetBtn_ToggleOn = true;
                SetVIPFollowButtonState(true);
            }
        }
    }
}