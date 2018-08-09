using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    public class KeySFXManager : MonoBehaviour
    {
        public static KeySFXManager instance;
        [SerializeField] AudioSource audioS;

        private void Start()
        {
            if (!instance)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && audioS)
                audioS.Play();
        }
    }
}
