using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerContainer : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI playerNameTxt;
    [SerializeField] private Button vipFollowTargetBtn;
    private bool vipFollowTargetBtn_ToggleOn = false;

    [Header("Colors")]
    [SerializeField] private Color unselectedColor = Color.grey;
    [SerializeField] private Color vipFollowTargetBtnColor = Color.red;

    public void Setup(string playerName)
    {
        playerNameTxt.text = playerName;

        vipFollowTargetBtn.image.color = unselectedColor;

        vipFollowTargetBtn.onClick.AddListener(delegate { OnClick_VIPFollowTargetBtn(); });
    }

    private void OnClick_VIPFollowTargetBtn()
    {
        if(vipFollowTargetBtn_ToggleOn)
            vipFollowTargetBtn.image.color = vipFollowTargetBtnColor;
        else
            vipFollowTargetBtn.image.color = unselectedColor;

        vipFollowTargetBtn_ToggleOn = !vipFollowTargetBtn_ToggleOn;
    }
}
