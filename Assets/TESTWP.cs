using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TESTWP : MonoBehaviour
{
    [SerializeField] private Transform[] wps;
    [SerializeField] private float spd;
    [SerializeField] private int counter = 0;

    private void Update()
    {
        MoveTowards(wps[counter]);

        if (Vector3.Distance(wps[counter].position,transform.position) < 0.5f)
        {
            counter = ++counter % wps.Length;
        }
    }

    private void MoveTowards(Transform wp)
    {
        transform.Translate((wp.position - transform.position).normalized * spd * Time.deltaTime);
    }
}
