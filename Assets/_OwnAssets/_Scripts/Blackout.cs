using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackout : MonoBehaviour, IActions
{
    [SerializeField] private List<Light> lamps;
    List<string> action = new List<string>();

    [Header("Exposed for InterfaceManager")]
    [SerializeField] Transform highestPoint;
    [SerializeField] Collider col;

    private void Start()
    {
        action.Add("Activate Blackout");
        action.Add("Deactivate Blackout");
    }

    public List<string> GetActions()
    {
        return action;
    }

    public void SetAction(string action)
    {
        switch (action)
        {
            case "Activate Blackout":
                foreach (Light lamp in lamps)
                {
                    lamp.enabled = false;
                }
                break;

            case "Deactivate Blackout":
                foreach (Light lamp in lamps)
                {
                    lamp.enabled = true;
                }
                break;
        }
    }

    public string GetName()
    {
        return gameObject.name;
    }

    public Vector3 GetHighestPointPos()
    {
        return highestPoint.position;
    }

    public Transform GetHighestPointTransform()
    {
        return highestPoint;
    }

    public Collider GetCollider()
    {
        return col;
    }
}
