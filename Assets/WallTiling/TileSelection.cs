using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4 { 
    public class WallTilingSelection : MonoBehaviour {
        public enum Tilesets {_1x1, _1x2, _2x2, _3x3, _1x3};
        public Tilesets size = Tilesets._1x1;
        public Tilesets prevSize;

        public GameObject _1x1_Prefab;
        public GameObject _1x2_Prefab;
        public GameObject _2x2_Prefab;
        public GameObject _3x3_Prefab;
        public GameObject _1x3_Prefab;

        private GameObject currObject;
        private void Start()
        {
            currObject = Instantiate(_1x1_Prefab, new Vector3(0, 0, 0), Quaternion.identity);
            currObject.transform.SetParent(transform);
            currObject.transform.position = transform.position;
            prevSize = size;
        }

        private void Update()
        {
            if (size != prevSize)
            {
                switch (size)
                {
                    case Tilesets._1x1:
                        Destroy(currObject);
                        break;
                    case Tilesets._1x2:
                        break;
                    case Tilesets._2x2:
                        break;
                    case Tilesets._3x3:
                        break;
                    case Tilesets._1x3:
                        break;
                    default:
                        break;
                }
            }
        }
    }
}