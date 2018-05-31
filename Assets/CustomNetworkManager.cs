using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private UIManager uiManager;

    public void StartHosting()
    {
        base.StartHost();
        uiManager.EnableHostBtn(false);
        uiManager.EnableUnhostBtn(true);
        uiManager.EnableInputField(false);
        uiManager.EnableJoinBtn(false);
    }

    public void StopHosting()
    {
        base.StopHost();
        uiManager.EnableHostBtn(true);
        uiManager.EnableUnhostBtn(false);
        uiManager.EnableInputField(true);
        uiManager.EnableJoinBtn(true);
    }

    public void Join(Text InputAddress)
    {
        NetworkManager.singleton.networkAddress = InputAddress.text;
        base.StartClient();
        uiManager.EnableInputField(false);
        uiManager.EnableJoinBtn(false);
        uiManager.EnableUnjoinBtn(true);
    }

    public void StopJoin()
    {
        base.StopClient();
        uiManager.EnableInputField(true);
        uiManager.EnableJoinBtn(true);
        uiManager.EnableUnjoinBtn(false);
    }
}
