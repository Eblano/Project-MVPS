using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject hostBtn;
    [SerializeField] private GameObject unhostBtn;
    [SerializeField] private GameObject joinBtn;
    [SerializeField] private GameObject unjoinBtn;
    [SerializeField] private GameObject inputField;

    public void EnableHostBtn(bool state)
    {
        hostBtn.SetActive(state);
    }

    public void EnableUnhostBtn(bool state)
    {
        unhostBtn.SetActive(state);
    }

    public void EnableJoinBtn(bool state)
    {
        joinBtn.SetActive(state);
    }

    public void EnableUnjoinBtn(bool state)
    {
        unjoinBtn.SetActive(state);
    }

    public void EnableInputField(bool state)
    {
        inputField.SetActive(state);
    }
}
