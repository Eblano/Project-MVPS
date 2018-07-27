using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SealTeam4
{
    public class SpawnFire : MonoBehaviour, IActions
    {
        [SerializeField] private ParticleSystem[] particleList;
        [SerializeField] private Light glow;
        [SerializeField] private AudioSource audioSource;

        [Header("Exposed for InterfaceManager")]
        [SerializeField] Transform highestPoint;
        [SerializeField] private Collider col;
        List<string> action = new List<string>();

        private void Start()
        {
            action.Add("Start Fire");
            action.Add("Extinguish Fire");
        }

        public List<string> GetActions()
        {
            return action;
        }

        public void SetAction(string action)
        {
            switch (action)
            {
                case "Start Fire":
                    col.isTrigger = false;
                    glow.enabled = true;
                    audioSource.Play();
                    foreach (ParticleSystem p in particleList)
                    {
                        p.Play();
                    }
                    break;

                case "Extinguish Fire":
                    col.isTrigger = true;
                    glow.enabled = false;
                    audioSource.Stop();
                    foreach (ParticleSystem p in particleList)
                    {
                        p.Stop();
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
}

