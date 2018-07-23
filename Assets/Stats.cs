using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    private int hitPoints;

    [SerializeField] private Collider colHead;
    [SerializeField] private Collider colLHand;
    [SerializeField] private Collider colRHand;
    [SerializeField] private Collider colLLeg;
    [SerializeField] private Collider colRLeg;
    [SerializeField] private Collider colBody;

    [SerializeField] private int headDmg;
    [SerializeField] private int handDmg;
    [SerializeField] private int legDmg;
    [SerializeField] private int bodyDmg;

    public void TakeDamage(Collider c)
    {
        if(c == colHead)
        {
            //hitPoints
        }
        else if (c == colLHand)
        {

        }
        else if (c == colRHand)
        {

        }
        else if (c == colLLeg)
        {

        }
        else if (c == colRLeg)
        {

        }
        else if (c == colBody)
        {

        }
    }
}
