using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    public class NPCGun : MonoBehaviour
    {
        [SerializeField] private Transform firingPt;
        [SerializeField] private GameObject hitEffect_Prefab;
        [SerializeField] private MuzzleFlash muzzleFlashEffect;

        private float timeToNextShot = 0;
        
        private float minVerticalDispersion = 0.1f;
        private float minHorizontalDispersion = 0.3f;

        public void FireGun(GlobalEnums.GunAccuracy accuracy)
        {
            switch (accuracy)
            {
                case GlobalEnums.GunAccuracy.HIGH:
                    break;
                case GlobalEnums.GunAccuracy.MID:
                    minHorizontalDispersion *= 1.3f;
                    minHorizontalDispersion *= 1.3f;
                    break;
                case GlobalEnums.GunAccuracy.LOW:
                    minHorizontalDispersion *= 1.6f;
                    minHorizontalDispersion *= 1.6f;
                    break;
            }

            Vector3 offsetAmt = 
                new Vector3(
                    Random.Range(-minHorizontalDispersion, minHorizontalDispersion), 
                    Random.Range(-minVerticalDispersion, minVerticalDispersion), 
                    0);

            Ray ray = new Ray(firingPt.position, firingPt.forward + offsetAmt);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity))
            {
                IDamageable iDamagable = hitInfo.transform.root.GetComponent<IDamageable>();

                if (iDamagable != null)
                    iDamagable.OnHit(hitInfo.collider, GlobalEnums.WeaponType.PISTOL);

                //Debug.Log(hitInfo.transform.name + " | " + hitInfo.transform.root.name);
                //Debug.Log("Bullet Hit");
                //Instantiate(hitEffect_Prefab, hitInfo.point, Quaternion.identity);
            }

            muzzleFlashEffect.Activate();
        }
    }
}