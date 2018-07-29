using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    public class DefaultMaterialLoader : MonoBehaviour
    {
        [SerializeField] private Material defMaterial;

        private void Start()
        {
            MeshRenderer mr = GetComponent<MeshRenderer>();

            if (defMaterial && mr && mr.material)
            {
                mr.material = defMaterial;
            }

            Destroy(this);
        }
    }
}
