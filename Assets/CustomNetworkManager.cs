using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace SealTeam4
{
    public class CustomNetworkManager : NetworkManager
    {
        private int numOfPlayers = -1;

        public override void OnStartClient(NetworkClient client)
        {
            base.OnStartClient(client);
            Debug.Log("OnClientDisconnect");
            UIManager.instance.SetUIPanelState(false);
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            Debug.Log("OnStopClient");
            UIManager.instance.SetUIPanelState(true);
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            Debug.Log("OnStartServer");
            numOfPlayers = -1;
            UIManager.instance.SetConnectPlayerTxtState(true);
            UIManager.instance.SetUIPanelState(false);
            GameManager.instance.GM_SwitchToHostMode();
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            if(!GameManager.instance.isHost)
            {
                base.OnClientConnect(conn);
                GameManager.instance.GM_SwitchToClientMode();
            }
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            Debug.Log("OnStopServer");
            numOfPlayers = -1;
            UIManager.instance.SetConnectPlayerTxtState(false);
            UIManager.instance.SetUIPanelState(true);
        }

        public override void OnServerConnect(NetworkConnection conn)
        {
            base.OnServerConnect(conn);
            numOfPlayers++;
            UIManager.instance.SetConnectPlayerTxt("Players Connected: " + numOfPlayers);
            Debug.Log("OnServerConnect");
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnServerDisconnect(conn);
            numOfPlayers--;
            UIManager.instance.SetConnectPlayerTxt("Players Connected: " + numOfPlayers);
            Debug.Log("OnServerDisconnect");
        }
    }
}
