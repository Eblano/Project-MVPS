using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AvailableLanList
{
    public static event Action<List<LanConnectionInfo>> OnAvailableLANGamesChanged = delegate { };

    public static List<LanConnectionInfo> lanGames = new List<LanConnectionInfo>();

    public static void HandleLanList(List<LanConnectionInfo> lanList)
    {
        lanGames = lanList;
        OnAvailableLANGamesChanged(lanGames);
    }
}
