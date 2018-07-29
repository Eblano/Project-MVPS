using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDoorTrigger : MonoBehaviour
{
    [SerializeField] private Door[] doors;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            return;
        }

        foreach (Door door in doors)
        {
            door.EnableDoorRot();
        }
    }
}
