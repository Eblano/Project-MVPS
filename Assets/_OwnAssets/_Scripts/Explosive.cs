using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : MonoBehaviour, IActions
{
    List<string> action = new List<string>();
    [SerializeField] ParticleSystem explosionParticles;

    void Start()
    {
        action.Add("Explode");
    }
    public List<string> GetActions()
    {
        foreach(string a in action)
        {
            Debug.Log(a);
        }
        return action;
    }

    public void SetAction(string action)
    {
        Debug.Log("SetAction");
        Explode();
    }

    private void Explode()
    {
        explosionParticles.Play();
        Destroy(gameObject);
    }
}
