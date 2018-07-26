using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Torchlight : MonoBehaviour, IUsableObject
{
    [SerializeField] private Light lightSource;

    public void UseObject(NetworkInstanceId networkInstanceId)
    {
        lightSource.enabled = !lightSource.enabled;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UseObject(new NetworkInstanceId(1));
        }
    }
}
