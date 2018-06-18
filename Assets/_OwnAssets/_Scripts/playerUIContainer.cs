using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerUIContainer : MonoBehaviour
{

    private string playerName;
    [SerializeField] private GameObject b1;
    [SerializeField] private GameObject b2;

    public void SetButtonStates(bool state)
    {
        b1.SetActive(state);
        b2.SetActive(state);
    }

}
