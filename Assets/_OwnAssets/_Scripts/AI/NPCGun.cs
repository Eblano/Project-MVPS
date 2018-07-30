using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    public class NPCGun : MonoBehaviour
    {
        [SerializeField] private Transform firingPt;
        [SerializeField] private GameObject hitEffect_Prefab;
        private float timeToNextShot = 0;
        
        private float minVerticalDispersion = 0.1f;
        private float minHorizontalDispersion = 0.5f;

        private List<Vector3> hitPoints = new List<Vector3>();

        private void Update()
        {
            foreach (Vector3 point in hitPoints)
            {
                Debug.DrawLine(firingPt.position, point, Color.green);
            }
        }

        public void FireGun(GlobalEnums.GunAccuracy accuracy)
        {
            float horizontalOffset = 0;
            float verticalOffset = 0;

            switch (accuracy)
            {
                case GlobalEnums.GunAccuracy.HIGH:
                    horizontalOffset = minHorizontalDispersion * 1f;
                    verticalOffset = minVerticalDispersion * 1f;
                    break;
                case GlobalEnums.GunAccuracy.MID:
                    horizontalOffset = minHorizontalDispersion * 1.3f;
                    verticalOffset = minVerticalDispersion * 1.3f;
                    break;
                case GlobalEnums.GunAccuracy.LOW:
                    horizontalOffset = minHorizontalDispersion * 1.6f;
                    verticalOffset = minVerticalDispersion * 1.6f;
                    break;
            }

            Vector3 offsetAmt = 
                new Vector3(
                    Random.Range(-horizontalOffset, horizontalOffset), 
                    Random.Range(-verticalOffset, verticalOffset), 
                    0);


            int layerToHit = ~(1 << LayerMask.NameToLayer("UI"));

            Ray ray = new Ray(firingPt.position, firingPt.forward + offsetAmt);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, layerToHit))
            {
                IDamageable iDamagable = hitInfo.transform.root.GetComponent<IDamageable>();

                hitPoints.Add(hitInfo.point);

                if (iDamagable != null)
                    iDamagable.OnHit(hitInfo.collider, GlobalEnums.WeaponType.PISTOL);

                //Debug.Log(hitInfo.transform.name + " | " + hitInfo.transform.root.name);
                //Debug.Log("Bullet Offset " + offsetAmt);
                //Debug.Log("Bullet Hit");
                Transform bulletHole = Instantiate(hitEffect_Prefab, hitInfo.point, Quaternion.identity).GetComponent<Transform>();
                bulletHole.LookAt(-hitInfo.normal);
                Destroy(bulletHole.gameObject, 120);
            }
        }
    }
}