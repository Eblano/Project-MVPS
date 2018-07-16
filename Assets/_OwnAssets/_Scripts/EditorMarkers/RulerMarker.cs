using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    public class RulerMarker : MonoBehaviour
    {
        private LineRenderer lineR;

        private void Start()
        {
            if (GetComponent<LineRenderer>())
                lineR = GetComponent<LineRenderer>();
            else
                lineR = gameObject.AddComponent<LineRenderer>();
        }
    }
}
