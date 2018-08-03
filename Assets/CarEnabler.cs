using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEnabler : MonoBehaviour
{
    [Battlehub.SerializeIgnore] [SerializeField] private GameObject[] enables;

    private void Start()
    {
        foreach(GameObject obj in enables)
        {
            obj.SetActive(true);
        }

        Destroy(this);
    }
}
