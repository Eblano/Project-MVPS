using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackout : MonoBehaviour, IActions
{
    [SerializeField] private List<LightController> lamps;
    List<string> action = new List<string>();

    private void Start()
    {
        action.Add("Activate Blackout");
        action.Add("Deactivate Blackout");
    }

    public List<string> GetActions()
    {
        Debug.Log("GA");
        return action;
    }

    public void SetAction(string action)
    {
        switch (action)
        {
            case "Activate Blackout":
                foreach (LightController lamp in lamps)
                {
                    lamp.on = false;
                }
                break;

            case "Deactivate Blackout":
                foreach (LightController lamp in lamps)
                {
                    lamp.on = true;
                }
                break;
        }
    }
}
