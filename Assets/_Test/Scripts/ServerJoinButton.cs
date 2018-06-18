using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ServerJoinButton : MonoBehaviour
{
    private Text buttonText;
    private LanConnectionInfo game;

    private void Awake()
    {
        buttonText = GetComponentInChildren<Text>();
        GetComponent<Button>().onClick.AddListener(JoinLanGame);
    }

    public void Initialise(LanConnectionInfo lanGame, Transform panelTransform)
    {
        game = lanGame;
        buttonText.text = lanGame.ipAddress;
        transform.SetParent(panelTransform);
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;
        transform.localPosition = Vector3.zero;
    }

    private void JoinLanGame()
    {
        NetworkManager.singleton.networkAddress = game.ipAddress;
        NetworkManager.singleton.StartClient();
    }
}
