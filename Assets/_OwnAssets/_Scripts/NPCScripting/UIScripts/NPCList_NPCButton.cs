using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPCList_NPCButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI labelText;
    [SerializeField] private GameObject rightPanel;
    [SerializeField] private GameObject infoPanel_Prefab;
    private GameObject infoPanel;

    private void Start()
    {
        infoPanel = Instantiate(infoPanel_Prefab, Vector3.zero, Quaternion.identity);
        infoPanel.transform.SetParent(rightPanel.transform);
        infoPanel.SetActive(false);
    }

    public void SetText(string text)
    {
        labelText.text = text;
    }
}
