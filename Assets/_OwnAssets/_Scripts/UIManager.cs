using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using SealTeam4;
using System.Net;
using System.Net.Sockets;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private GameObject connectedPlayerUI;
    [SerializeField] private GameObject disconnectedPopup;
    private Text txtConnectedPlayer;

    [SerializeField] private TextMeshProUGUI ipaddressTxt;
    [SerializeField] private TextMeshProUGUI sceneNameTxt;
    [SerializeField] private TextMeshProUGUI sceneHastTxt;

    private readonly float ipRefreshRate = 5.0f;
    private float currIPrefreshCD = 0;

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

        SetSceneInfoTxt();
    }

    private void Update()
    {
        UpdateIPAddressText();
    }

    private void SetSceneInfoTxt()
    {
        sceneNameTxt.text = GameManager.instance.GetSceneName();
        sceneHastTxt.text = GameManager.instance.GetSceneHash();
    }

    private void UpdateIPAddressText()
    {
        if (currIPrefreshCD <= 0)
        {
            currIPrefreshCD = ipRefreshRate;
            ipaddressTxt.text = "Host Address: " + GetLocalIPAddress();
        }
        else
            currIPrefreshCD -= Time.deltaTime;
    }

    public string GetLocalIPAddress()
    {
        IPHostEntry host;
        string localIP = "";
        host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
                break;
            }
        }
        return localIP;
    }

    public void SetUIPanelState(bool state)
    {
        uiPanel.SetActive(state);
    }

    public void SetConnectPlayerTxtState(bool state)
    {
        //connectedPlayerUI.SetActive(state);
    }

    public void ShowDisconnectedPopup()
    {
        disconnectedPopup.SetActive(true);
    }

    public void SetConnectPlayerTxt(string txt)
    {
        txtConnectedPlayer.text = txt;
    }

    public void BtnHost()
    {
        CustomNetworkDiscovery.instance.StartToBroadcast();
    }

    public void BtnScanForBroadcasts()
    {
        CustomNetworkDiscovery.instance.ScanForGames();
    }

    public void OnBackToEditorBtnClick()
    {
        GameManager.instance.RestartScene();
    }
}
