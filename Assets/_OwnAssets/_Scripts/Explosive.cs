using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SealTeam4;

public class Explosive : MonoBehaviour, IActions, INetworkCommandable
{
    private List<string> action = new List<string>();
    [SerializeField] private ParticleSystem explosionParticles;
    private Collider col;

    void Start()
    {
        action.Add("Explode");
        GameManager.instance.RegisterNetCmdObj(gameObject, false);
        col = GetComponent<BoxCollider>();
    }
    public List<string> GetActions()
    {
        foreach (string a in action)
        {
            Debug.Log(a);
        }
        return action;
    }

    public void SetAction(string action)
    {
        switch (action)
        {
            case "Explode":
                Explode();
                GameManager.instance.SendNetCmdObjMsg(gameObject, "Explode");
                break;
        }
    }

    private void Explode()
    {
        ParticleSystem explosion;
        explosion = Instantiate(explosionParticles, gameObject.transform.position, Quaternion.identity) as ParticleSystem;
        explosion.Play();
        //GameManagerAssistant.instance.CmdSyncHaps(, ControllerHapticsManager.HapticType.GUNFIRE, VRTK.VRTK_DeviceFinder.Devices.LeftController);
        //GameManagerAssistant.instance.CmdSyncHaps(, ControllerHapticsManager.HapticType.GUNFIRE, VRTK.VRTK_DeviceFinder.Devices.RightController);
        action.Clear();
        Destroy(gameObject);
        GameManager.instance.TriggerThreatInLevel(true);
    }

    public string GetName()
    {
        return gameObject.name;
    }

    public Vector3 GetHighestPointPos()
    {
        return transform.position;
    }

    public Transform GetHighestPointTransform()
    {
        return transform;
    }

    public Collider GetCollider()
    {
        return col;
    }

    public void RecieveCommand(string command)
    {
        switch(command)
        {
            case "Explode":
                Explode();
                break;
        }
    }
}
