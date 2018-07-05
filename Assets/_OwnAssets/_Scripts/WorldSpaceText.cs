using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SealTeam4
{
    public class WorldSpaceText : MonoBehaviour
    {
        private GameObject canvasCam;
        private DynamicBillboard parentDBillboard;

        [SerializeField] private Canvas canvas;
        [SerializeField] private Button btn;
        [SerializeField] private TextMeshProUGUI text;

        private void Start()
        {
            btn.onClick.AddListener(delegate { OnBtnClick(); });
        }

        public void SetParentDynamicBillboard(DynamicBillboard billbaord)
        {
            parentDBillboard = billbaord;
        }

        private void Update()
        {
            if(!canvasCam)
            {
                canvasCam = GameObject.Find("MarkerUICamera(Clone)");
                if(canvasCam)
                {
                    canvas.worldCamera = canvasCam.GetComponent<Camera>();
                }
            }
        }

        public void SetText(string newText)
        {
            text.text = newText;
        }

        private void OnBtnClick()
        {
            parentDBillboard.SelectThis();
        }

        public GameObject GetObject()
        {
            return parentDBillboard.gameObject;
        }
    }
}