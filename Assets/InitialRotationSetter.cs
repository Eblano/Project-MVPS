using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    public class InitialRotationSetter : MonoBehaviour
    {
        [SerializeField] private bool setted = false;
        [SerializeField] private Vector3 rotation;

        private void Start()
        {
            if(!setted)
            {
                transform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
            }
            Destroy(this);
        }
    }
}

