using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchListPanel : MonoBehaviour
{
    [SerializeField] private ServerJoinButton buttonPref;

    private void Awake()
    {
        AvailableLanList.OnAvailableLANGamesChanged += AvailableLanList_OnAvailableLANGamesChanged;
    }

    private void AvailableLanList_OnAvailableLANGamesChanged(List<LanConnectionInfo> lanGames)
    {
        ClearButtons();
        CreateButtons(lanGames);
    }

    private void ClearButtons()
    {
        ServerJoinButton[] serverJoinButtons = GetComponentsInChildren<ServerJoinButton>();

        foreach(ServerJoinButton button in serverJoinButtons)
        {
            Destroy(button.gameObject);
        }
    }

    private void CreateButtons(List<LanConnectionInfo> lanGames)
    {
        foreach (LanConnectionInfo lanGame in lanGames)
        {
            ServerJoinButton button = Instantiate(buttonPref);
            button.Initialise(lanGame, transform);
        }
    }
}
