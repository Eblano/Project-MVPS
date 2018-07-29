using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    public class TileSelection : MonoBehaviour
    {
        [SerializeField] private int x_Axis = 1;
        [SerializeField] private int z_Axis = 1;

        private int prevX_Axis;
        private int prevZ_Axis;

        private GameObject currObject;
        private void Start()
        {
            prevX_Axis = x_Axis;
            prevZ_Axis = z_Axis;
        }

        private void Update()
        {
            //if a change is detected
            if (x_Axis != prevX_Axis || z_Axis != prevZ_Axis)
            {
                //set new axis values for the tile
                SetTile(x_Axis, z_Axis);
                prevX_Axis = x_Axis;
                prevZ_Axis = z_Axis;
            }
        }

        private void SetTile(int x, int z)
        {
            Debug.Log("x:" + x + ", z:" + z);
            gameObject.transform.localScale = new Vector3(x, 0.005f, z);
        }
    }
}

