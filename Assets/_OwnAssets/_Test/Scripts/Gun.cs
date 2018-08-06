using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SealTeam4;

public class Gun : NetworkBehaviour, IUsableObject, ITwoHandedObject, IButtonActivatable
{
    public GunProfile gun;
    [SerializeField] private GameObject spawnPref;
    [SerializeField] private Transform firingPoint;
    [SerializeField] private List<Transform> secondaryGrabTransforms;
    public Transform secondaryHoldingTransform;
    [SerializeField] private MuzzleFlash muzzleFlashEffects;
    [SerializeField] private GameObject bulletPref;
    [SerializeField] private Transform bulletExitPoint;
    [SerializeField] private GameObject bloodFX_Prefab;
    [SerializeField] private GameObject hitEffect_Prefab;
    private InteractableObject interactableObject;
    private bool isTwoHandedGrab = false;
    private bool isBeingGrabbed = false;
    private bool grabStateChanged = false;
    private Vector3 initRot;
    private NetworkAnimator gunNetworkAnim;
    private NetworkedAudioSource networkedAudioSource;
    private NetworkInstanceId gunNetID;

    private class Rays
    {
        public Vector3 start;
        public Vector3 end;
    }
    private List<Rays> hitrays = new List<Rays>();

    private void Start()
    {
        interactableObject = GetComponent<InteractableObject>();
        gunNetworkAnim = GetComponent<NetworkAnimator>();
        networkedAudioSource = GetComponent<NetworkedAudioSource>();
        gunNetID = GetComponent<NetworkIdentity>().netId;
    }

    private void Update()
    {
        if (isTwoHandedGrab)
        {
            CalculateGunRotation();
        }

        if (secondaryGrabTransforms.Count > 0)
        {
            CheckGrabState();
        }

        foreach(Rays rayy in hitrays)
        {
            Debug.DrawLine(rayy.start, rayy.end);
        }

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    if (muzzleFlashEffects)
        //        muzzleFlashEffects.Activate();

        //    if (gunNetworkAnim)
        //        gunNetworkAnim.SetTrigger("Fire");

        //    if (networkedAudioSource)
        //        networkedAudioSource.DirectPlay();

        //    BulletShellSpawn();
        //}
    }

    #region HelperMethods
    /// <summary>
    /// Checks for the object grabbed state.
    /// </summary>
    private void CheckGrabState()
    {
        if (interactableObject.GetOwner() != null)
        {
            isBeingGrabbed = true;
        }
        else
        {
            isBeingGrabbed = false;
        }

        if (grabStateChanged != isBeingGrabbed)
        {
            if (isBeingGrabbed)
            {
                SetColliderState(true);
            }
            else
            {
                SetColliderState(false);
            }

            grabStateChanged = isBeingGrabbed;
        }
    }

    public void BulletShellSpawn(Vector3 force)
    {
        GameObject GO = Instantiate(bulletPref, bulletExitPoint.position, bulletExitPoint.rotation);

        if (force != Vector3.zero)
        {
            GO.GetComponent<Rigidbody>().AddForce(force);
        }
        else
        {
            Vector3 forceStrength = new Vector3(Random.Range(20, 50), 25, 0);
            if (Random.Range(0, 1) == 0)
            {
                GO.GetComponent<Rigidbody>().AddForce(forceStrength);
            }
            else
            {
                forceStrength.x *= -1;
                GO.GetComponent<Rigidbody>().AddForce(forceStrength);
            }

            GameManagerAssistant.instance.RelaySenderCmdGunShellSync(gunNetID, forceStrength);
        }

        Destroy(GO, 5.0f);
    }

    private void SetColliderState(bool state)
    {
        foreach (Transform t in secondaryGrabTransforms)
        {
            t.GetComponent<Collider>().enabled = state;
        }
    }

    /// <summary>
    /// Uses raycast to check for a hit.
    /// </summary>
    public void FireBullet()
    {
        RaycastHit hit;

        if (Physics.Raycast(firingPoint.position, firingPoint.forward, out hit, Mathf.Infinity))
        {
            Debug.Log("firingPoint.position" + firingPoint.position);
            IDamageable damageableObj = hit.collider.transform.root.GetComponent<IDamageable>();
            Rays rayy = new Rays
            {
                start = firingPoint.position,
                end = hit.point
            };

            hitrays.Add(rayy);

            if (damageableObj != null)
            {
                GameManagerAssistant.instance.RelaySenderCmdSpawnBloodPlayer(gunNetID, hit.point, hit.normal, Quaternion.FromToRotation(Vector3.forward, -hit.normal).eulerAngles);
                SpawnBlood(hit.point, hit.normal, Quaternion.FromToRotation(Vector3.forward, -hit.normal).eulerAngles);
                damageableObj.OnHit(hit.collider, GlobalEnums.WeaponType.PISTOL);
            }
            else
            {
                GameManagerAssistant.instance.RelaySenderCmdSpawnBulletHolePlayer(gunNetID, hit.point, hit.normal, Quaternion.FromToRotation(Vector3.forward, -hit.normal).eulerAngles);
                SpawnBulletHole(hit.point, hit.normal, Quaternion.FromToRotation(Vector3.forward, -hit.normal).eulerAngles);
            }
            //Instantiate(spawnPref, hit.point, firingPoint.rotation);
        }
    }
    #endregion HelperMethods

    #region InterfaceMethods
    /// <summary>
    /// Called if use button is pressed.
    /// </summary>
    public void UseObject(NetworkInstanceId networkInstanceId)
    {
        FireGun(networkInstanceId);
    }

    /// <summary>
    /// Called if up button is pressed.
    /// </summary>
    public void UpButtonPressed()
    {
        SetSafety(!gun.IsSafety());
    }

    /// <summary>
    /// Called if down button is pressed.
    /// </summary>
    public void DownButtonPressed()
    {
        UnloadMagazine();

    }

    /// <summary>
    /// Called if secondary hand becomes active.
    /// </summary>
    public void SecondHandActive()
    {
        isTwoHandedGrab = true;
        initRot = transform.localEulerAngles;
    }

    /// <summary>
    /// Called if secondary hand becomes inactive.
    /// </summary>
    public void SecondHandInactive()
    {
        isTwoHandedGrab = false;
        ResetGunRotation();
    }
    #endregion InterfaceMethods

    #region GunHandlingMethods
    private void FireGun(NetworkInstanceId networkInstanceId)
    {
        VRTK.VRTK_DeviceFinder.Devices devices = VRTK.VRTK_DeviceFinder.Devices.Headset;

        if (transform.parent.name == "mixamorig:LeftHand")
        {
            devices = VRTK.VRTK_DeviceFinder.Devices.LeftController;
        }
        else if (transform.parent.name == "mixamorig:LeftHand")
        {
            devices = VRTK.VRTK_DeviceFinder.Devices.RightController;
        }

        // If chamber is loaded and not on safety mode
        if (gun.ChamberIsLoaded() && !gun.IsSafety())
        {
            // Fire bullet
            //GameManagerAssistant.instance.CmdSyncHaps(networkInstanceId, ControllerHapticsManager.HapticType.GUNFIRE, devices);

            GameManagerAssistant.instance.CmdGunFire(gunNetID);
            GameManagerAssistant.instance.RelaySenderCmdGunEffectSync(gunNetID);
            ControllerHapticsManager.PlayHaptic(ControllerHapticsManager.HapticType.GUNFIRE, devices);
            if (gunNetworkAnim)
            {
                gunNetworkAnim.SetTrigger("Fire");
            }
            ActivateGunEffects();
            BulletShellSpawn(Vector3.zero);

            // If there is a magazine attached
            if (gun.GetMagazine() != null)
            {
                // If there is no more bullets left in the magazine
                if (gun.GetMagazine().GetBulletsInMag() == 0)
                {
                    // Unload chamber
                    gun.SetChamberState(false);
                }
                else
                {
                    // Load into chamber
                    gun.GetMagazine().LoadIntoChamber();
                }
            }
            else
            {
                gun.SetChamberState(false);
            }
        }
        else
        {
            // Play no bullet sound
            //GameManagerAssistant.instance.CmdSyncHaps(networkInstanceId, ControllerHapticsManager.HapticType.GUNRELOAD, devices);
        }
    }

    private void SetSafety(bool state)
    {
        gun.SetSafeState(state);
    }

    private void CalculateGunRotation()
    {
        transform.rotation = Quaternion.FromToRotation(Vector3.forward, secondaryHoldingTransform.position - transform.position);
    }

    private void ResetGunRotation()
    {
        transform.localRotation = Quaternion.Euler(initRot);
    }

    public void UnloadMagazine()
    {
        if (gun.GetMagazine() != null)
        {
            gun.GetMagazine().UnsnapObject();
            gun.SetMagazine(null);
        }
    }

    public void LoadChamber()
    {
        // If there is a magazine attached and it has bullets
        if (gun.GetMagazine() != null && gun.GetMagazine().GetBulletsInMag() > 0)
        {
            // If chamber is already loaded
            if (gun.ChamberIsLoaded())
            {
                if (bulletPref)
                {
                    BulletShellSpawn(Vector3.zero);
                }
            }
            else
            {
                // Change chamber load state
                gun.SetChamberState(true);
            }
            // Remove bullet from magazine
            gun.GetMagazine().LoadIntoChamber();
        }
        else
        {
            // play load into gun no bullet sound?
        }
    }

    public void ActivateGunEffects()
    {
        if (muzzleFlashEffects)
        {
            muzzleFlashEffects.Activate();
        }

        if (networkedAudioSource)
        {
            networkedAudioSource.DirectPlay();
        }
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
    #endregion GunHandlingMethods

    /// <summary>
    /// Stores the information of a gun. Contains methods for chamber, spread and maagzine.
    /// </summary>
    [System.Serializable]
    public class GunProfile
    {
        [SerializeField] private float spread;
        private Magazine currMag;
        private bool bulletInChamber = false;
        private bool isSafe = true;

        public void SetChamberState(bool state)
        {
            bulletInChamber = state;
        }

        public bool ChamberIsLoaded()
        {
            return bulletInChamber;
        }

        public void SetSafeState(bool state)
        {
            isSafe = state;
        }

        public bool IsSafety()
        {
            return isSafe;
        }

        public float GetBulletSpread()
        {
            return Random.Range(-spread, spread);
        }

        public Magazine GetMagazine()
        {
            return currMag;
        }

        public void SetMagazine(Magazine magazine)
        {
            currMag = magazine;
        }
    }
}