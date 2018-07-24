using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDoorTrigger : MonoBehaviour
{
    [SerializeField] Door[] doors;

    private void OnTriggerEnter(Collider other)
    {
        foreach (Door door in doors)
        {
            door.EnableDoorRot();
        }
    }
}
