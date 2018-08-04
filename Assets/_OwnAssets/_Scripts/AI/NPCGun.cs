using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace SealTeam4
{
    public class NPCGun : MonoBehaviour
    {
        [SerializeField] private Transform firingPt;
        [SerializeField] private GameObject hitEffect_Prefab;
        [SerializeField] private GameObject bloodFX_Prefab;
        [SerializeField] private MuzzleFlash muzzleFlashEffect;
        private NetworkAnimator gunNetworkAnim;
        private NetworkInstanceId gunNetID;
        private NetworkedAudioSource networkedAudioSource;

        private float timeToNextShot = 0;

        private float minVerticalDispersion = 0.03f;
        private float minHorizontalDispersion = 0.04f;

        private List<Vector3> hitPoints = new List<Vector3>();
        

        private void Start()
        {
            gunNetworkAnim = GetComponent<NetworkAnimator>();
            networkedAudioSource = GetComponent<NetworkedAudioSource>();
            gunNetID = GetComponent<NetworkIdentity>().netId;
        }

        private void Update()
        {
            foreach (Vector3 point in hitPoints)
            {
                Debug.DrawLine(firingPt.position, point, Color.green);
            }

            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            //    FireGun(GlobalEnums.GunAccuracy.HIGH);
            //}
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


            int layerToHit = ~(1 << LayerMask.NameToLayer("UI")) | ~(1 << LayerMask.NameToLayer("Player"));

            Ray ray = new Ray(firingPt.position, firingPt.forward + offsetAmt);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, layerToHit))
            {
                IDamageable iDamagable = hitInfo.transform.root.GetComponent<IDamageable>();

                hitPoints.Add(hitInfo.point);

                if (iDamagable != null)
                {
                    // Deal Damage
                    iDamagable.OnHit(hitInfo.collider, GlobalEnums.WeaponType.PISTOL);

                    //// Spawn blood particles
                    //GameObject bloodFX = Instantiate(bloodFX_Prefab, 
                    //                         hitInfo.point + (hitInfo.normal * 0.005F),
                    //                         Quaternion.FromToRotation(Vector3.forward, -hitInfo.normal)
                    //                         ) as GameObject;

                    GameManagerAssistant.instance.RpcSpawnBloodServer(gunNetID, hitInfo.point, hitInfo.normal, Quaternion.FromToRotation(Vector3.forward, -hitInfo.normal).eulerAngles);
                    SpawnBlood(hitInfo.point, hitInfo.normal, Quaternion.FromToRotation(Vector3.forward, -hitInfo.normal).eulerAngles);
                }
                else
                {
                    //// Spawn bullet hole
                    //GameObject bulletHole = Instantiate(
                    //                        hitEffect_Prefab, 
                    //                        hitInfo.point + (hitInfo.normal*0.005F), 
                    //                        Quaternion.FromToRotation(Vector3.forward, -hitInfo.normal)
                    //                        );
                    //Destroy(bulletHole, 360);

                    GameManagerAssistant.instance.RpcSpawnBulletHoleServer(gunNetID, hitInfo.point, hitInfo.normal, Quaternion.FromToRotation(Vector3.forward, -hitInfo.normal).eulerAngles);
                    SpawnBulletHole(hitInfo.point, hitInfo.normal, Quaternion.FromToRotation(Vector3.forward, -hitInfo.normal).eulerAngles);
                }
                //Debug.Log(hitInfo.transform.name + " | " + hitInfo.transform.root.name);
                //Debug.Log("Bullet Offset " + offsetAmt);
                //Debug.Log("Bullet Hit");
            }

            GameManagerAssistant.instance.RpcSyncGunEffects(gunNetID);
            SyncAIGunEffects();

            if (gunNetworkAnim)
                gunNetworkAnim.SetTrigger("AI_Fire");
        }

        public void SpawnBlood(Vector3 hitPos, Vector3 normal, Vector3 faceAngle)
        {
            GameObject bloodFX = Instantiate(
                                 bloodFX_Prefab,
                                 hitPos + (normal * 0.005F),
                                 Quaternion.Euler(faceAngle)
                                 );
        }

        public void SpawnBulletHole(Vector3 hitPos, Vector3 normal, Vector3 faceAngle)
        {
            GameObject bulletHole = Instantiate(
                                 hitEffect_Prefab,
                                 hitPos + (normal * 0.005F),
                                 Quaternion.Euler(faceAngle)
                                 );

            Destroy(bulletHole, 360);
        }

        public void SyncAIGunEffects()
        {
            if (muzzleFlashEffect)
                muzzleFlashEffect.Activate();

            if (networkedAudioSource)
                networkedAudioSource.DirectPlay();
        }
    }
}