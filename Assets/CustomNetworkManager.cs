using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private GameObject networkUIPanel;
    [SerializeField] private GameObject connectedPlayerUI;
    private Text txtConnectedPlayer;
    private int numOfPlayers = -1;

    private void Start()
    {
        txtConnectedPlayer = connectedPlayerUI.GetComponent<Text>();
    }

    public override void OnStartClient(NetworkClient client)
    {
        base.OnStartClient(client);
        Debug.Log("OnClientDisconnect");
        networkUIPanel.SetActive(false);
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        Debug.Log("OnStopClient");
        networkUIPanel.SetActive(true);
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("OnStartServer");
        numOfPlayers = -1;
        connectedPlayerUI.SetActive(true);
        networkUIPanel.SetActive(false);
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        Debug.Log("OnStopServer");
        numOfPlayers = -1;
        connectedPlayerUI.SetActive(false);
        networkUIPanel.SetActive(true);
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        numOfPlayers++;
        txtConnectedPlayer.text = "Players Connected: " + numOfPlayers;
        Debug.Log("OnServerConnect");
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        numOfPlayers--;
        txtConnectedPlayer.text = "Players Connected: " + numOfPlayers;
        Debug.Log("OnServerDisconnect");
    }
}
