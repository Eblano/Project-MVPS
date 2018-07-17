using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    public class NPCGun : MonoBehaviour
    {
        [SerializeField] private Transform firingPt;
        [SerializeField] private float firingRate = 0.5f;
        [SerializeField] private GameObject hitEffect_Prefab;
        private float timeToNextShot = 0;

        public void FireGun()
        {
            if (timeToNextShot <= 0)
            {
                FireRaycastBullet();
                timeToNextShot = firingRate;
            }
            else
                timeToNextShot -= Time.deltaTime;
        }

        private void FireRaycastBullet()
        {
            Ray ray = new Ray(firingPt.position, firingPt.forward);
            RaycastHit hitInfo;

            if(Physics.Raycast(ray, out hitInfo, Mathf.Infinity))
            {
                if(hitEffect_Prefab)
                {
                    GameObject hitEffect = Instantiate(hitEffect_Prefab, hitInfo.point, Quaternion.identity);
                }
                    
            }
        }
    }
}