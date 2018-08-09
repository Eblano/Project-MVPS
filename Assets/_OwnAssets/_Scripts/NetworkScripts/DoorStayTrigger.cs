using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorStayTrigger : MonoBehaviour
{
    [SerializeField] private Door[] doors;
    [SerializeField] private int onTrigger = 0;

    private void OnTriggerEnter(Collider other)
    {
        onTrigger++;
    }

    private void OnTriggerExit(Collider other)
    {
        onTrigger--;
        if(onTrigger == 0)
        {
            foreach (Door door in doors)
            {
                door.CloseDoor();
            }
        }
    }
}
