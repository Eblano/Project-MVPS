using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;

namespace SealTeam4
{
    public class CustomNetworkManager : NetworkManager
    {
        private int numOfPlayers = -1;

        private NetworkStartPosition[] spawnPoints;
        private int posNum = 0;

        public override void OnStartClient(NetworkClient client)
        {
            base.OnStartClient(client);
            Debug.Log("OnStartClient");
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

        //public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        //{
        //    base.OnServerAddPlayer(conn, playerControllerId);
        //}

        private void Start()
        {
            spawnPoints = FindObjectsOfType<NetworkStartPosition>();
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            int prefNum = 0;
            //base.OnClientConnect(conn);
            if (!GameManager.instance.IsHost())
            {
                GameManager.instance.GM_SwitchToClientMode();
                prefNum = 1;
            }
            IntegerMessage msg = new IntegerMessage(prefNum);
            ClientScene.AddPlayer(conn, 0, msg);
        }

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
        {
            GameObject playerPref;
            Vector3 spawnPosition = Vector3.zero;
            bool isPlayerHost = false;

            if(extraMessageReader != null)
            {
                IntegerMessage stream = extraMessageReader.ReadMessage<IntegerMessage>();
                if(stream.value == 0)
                {
                    isPlayerHost = true;
                }
            }

            if (isPlayerHost)
            {
                playerPref = spawnPrefabs[0];
                Debug.Log("Is Server");
            }
            else
            {
                playerPref = spawnPrefabs[1];
                Debug.Log("Is Client");
            }

            if (spawnPoints.Length > 0)
            {
                spawnPosition = spawnPoints[posNum++ % spawnPoints.Length].transform.position;
            }

            GameObject player = Instantiate(playerPref, spawnPosition, Quaternion.identity);
            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        }
    }
}
