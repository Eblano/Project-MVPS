using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectInfoSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleTxt;
    [SerializeField] private TextMeshProUGUI contentTxt;

    public void UpdateContent(string title, string content)
    {
        titleTxt.text = title;
        contentTxt.text = content;
    }
}
