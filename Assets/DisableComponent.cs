using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SealTeam4;

public class DisableComponent : MonoBehaviour
{
    [SerializeField] private Behaviour[] componentsToDelete;
    private List<Behaviour> toBeDeleted;

    private void Start()
    {
        if (GameManager.instance.isServerObj)
        {
            return;
        }

        toBeDeleted = new List<Behaviour>();
        foreach (Behaviour b in componentsToDelete)
        {
            toBeDeleted.Add(b);
        }

        foreach (Behaviour b in toBeDeleted)
        {
            Destroy(b);
        }
    }
}
