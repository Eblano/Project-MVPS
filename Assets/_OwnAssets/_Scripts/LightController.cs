using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    private List<Light> outList;
    private Ray ray;
    private RaycastHit hit;

    private void Start()
    {
        outList = new List<Light>();
    }

    public bool on = true;
}
