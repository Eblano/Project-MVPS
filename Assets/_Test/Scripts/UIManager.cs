using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SealTeam4;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private GameObject connectedPlayerUI;
    private Text txtConnectedPlayer;

    private void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("NO U");
            Destroy(this);
        }

        txtConnectedPlayer = connectedPlayerUI.GetComponent<Text>();
    }

    public void SetUIPanelState(bool state)
    {
        uiPanel.SetActive(state);
    }

    public void SetConnectPlayerTxtState(bool state)
    {
        connectedPlayerUI.SetActive(state);
    }

    public void SetConnectPlayerTxt(string txt)
    {
        txtConnectedPlayer.text = txt;
    }

    public void BtnStartToBroadcast()
    {
        CustomNetworkDiscovery.instance.StartToBroadcast();
    }

    public void BtnScanForBroadcasts()
    {
        CustomNetworkDiscovery.instance.ScanForGames();
    }
}
