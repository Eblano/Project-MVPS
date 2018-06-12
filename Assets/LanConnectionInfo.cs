using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct LanConnectionInfo
{
    public string ipAddress;
    public int port;

    public LanConnectionInfo(string fromAddress, string data)
    {
        ipAddress = fromAddress.Substring(fromAddress.LastIndexOf(":") + 1, fromAddress.Length - (fromAddress.LastIndexOf(":") + 1));
        port = 7777;
    }
}
