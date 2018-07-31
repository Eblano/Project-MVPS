using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SealTeam4
{
    public class DisconnectedPopup : MonoBehaviour
    {
        [SerializeField] private Button backToEditorBtn;

        private void Start()
        {
            backToEditorBtn.onClick.AddListener(delegate { OnBackToEditorBtnClick(); });
        }

        private void OnBackToEditorBtnClick()
        {
            GameManager.instance.RestartScene();
        }
    }
}
