

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    public class TileSelection : MonoBehaviour
    {
        [SerializeField] private int x_Axis = 1;
        [SerializeField] private int y_Axis = 1;
        [SerializeField] private GameObject _1x1_Prefab;

        private int prevX_Axis;
        private int prevY_Axis;

        private GameObject currObject;
        private void Start()
        {
            prevX_Axis = x_Axis;
            prevY_Axis = y_Axis;
            currObject = Instantiate(_1x1_Prefab, new Vector3(0, 0, 0), Quaternion.identity);
            currObject.transform.SetParent(transform);
            currObject.transform.position = transform.position;
        }

        private void Update()
        {
            //if a change is detected
            if (x_Axis != prevX_Axis || y_Axis != prevY_Axis)
            {
                //set new axis values for the tile
                SetTile(x_Axis, y_Axis);
            }
        }

        private void SetTile(int x, int y)
        {
            Debug.Log("x:" + x + ", y:" + y);
            gameObject.transform.localScale = new Vector3(x, y, 0);
        }
    }
}

