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
    private InteractableObject interactableObject;
    private bool isTwoHandedGrab = false;
    private bool isBeingGrabbed = false;
    private bool grabStateChanged = false;
    private Vector3 initRot;
    private NetworkAnimator gunNetworkAnim;
    private NetworkedAudioSource networkedAudioSource;

    private void Start()
    {
        interactableObject = GetComponent<InteractableObject>();
        gunNetworkAnim = GetComponent<NetworkAnimator>();
        networkedAudioSource = GetComponent<NetworkedAudioSource>();
    }

    private void Update()
    {
        if (isTwoHandedGrab)
        {
            CmdCalculateGunRotation();
        }

        if (secondaryGrabTransforms.Count > 0)
        {
            CheckGrabState();
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

    private void BulletShellSpawn()
    {
        GameObject GO = Instantiate(bulletPref, bulletExitPoint.position, bulletExitPoint.rotation);
        if (Random.Range(0, 1) == 0)
        {
            GO.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(20, 50), 25, 0));
        }
        else
        {
            GO.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-20, -50), 25, 0));
        }
        NetworkServer.Spawn(GO);
        StartCoroutine(DestroyServerObjAfter(GO, 5.0f));
    }

    private IEnumerator DestroyServerObjAfter(GameObject destroyObj, float destroyAfter)
    {
        yield return new WaitForSeconds(destroyAfter);
        NetworkServer.Destroy(destroyObj);
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
    private void FireBullet()
    {
        RaycastHit hit;
        if (Physics.Raycast(firingPoint.position, firingPoint.forward, out hit, Mathf.Infinity))
        {
            IDamageable damageableObj = hit.collider.transform.root.GetComponent<IDamageable>();

            if (damageableObj != null)
            {
                damageableObj.OnHit(hit.collider, GlobalEnums.WeaponType.PISTOL);
            }

            Instantiate(spawnPref, hit.point, firingPoint.rotation);
        }
    }
    #endregion HelperMethods

    #region InterfaceMethods
    /// <summary>
    /// Called if use button is pressed.
    /// </summary>
    public void UseObject(NetworkInstanceId networkInstanceId)
    {
        CmdFireGun(networkInstanceId);
    }

    /// <summary>
    /// Called if up button is pressed.
    /// </summary>
    public void UpButtonPressed()
    {
        //CmdLoadChamber();
        CmdSafety(!gun.IsSafety());
    }

    /// <summary>
    /// Called if down button is pressed.
    /// </summary>
    public void DownButtonPressed()
    {
        CmdUnloadMagazine();
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
        CmdResetGunRotation();
    }
    #endregion InterfaceMethods

    #region ServerMethods
    /// <summary>
    /// Tries to fire the gun.
    /// </summary>
    //[Command]
    private void CmdFireGun(NetworkInstanceId networkInstanceId)
    {
        RpcFireGun(networkInstanceId);
    }

    /// <summary>
    /// Calculates gun angle base on second hand position.
    /// </summary>
    //[Command]
    private void CmdCalculateGunRotation()
    {
        RpcCalculateGunRotation();
    }

    /// <summary>
    /// Reset the gun's rotation to identity.
    /// </summary>
    //[Command]
    private void CmdResetGunRotation()
    {
        RpcResetGunRotation();
    }

    /// <summary>
    /// Unload the magazine.
    /// </summary>
    //[Command]
    private void CmdUnloadMagazine()
    {
        RpcUnloadMagazine();
    }

    /// <summary>
    /// Load the chamber into the gun.
    /// </summary>
    //[Command]
    public void CmdLoadChamber()
    {
        RpcLoadChamber();
    }

    //[Command]
    public void CmdSafety(bool state)
    {
        RpcSafety(state);
    }

    //[ClientRpc]
    private void RpcSafety(bool state)
    {
        gun.SetSafeState(state);
    }

    //[ClientRpc]
    private void RpcFireGun(NetworkInstanceId networkInstanceId)
    {
        VRTK.VRTK_DeviceFinder.Devices devices = VRTK.VRTK_DeviceFinder.Devices.Headset;

        if (transform.parent.name == "LHand")
        {
            devices = VRTK.VRTK_DeviceFinder.Devices.LeftController;
        }
        else if (transform.parent.name == "RHand")
        {
            devices = VRTK.VRTK_DeviceFinder.Devices.RightController;
        }

        // If chamber is loaded and not on safety mode
        if (gun.ChamberIsLoaded() && !gun.IsSafety())
        {
            // Fire bullet
            //GameManagerAssistant.instance.CmdSyncHaps(networkInstanceId, ControllerHapticsManager.HapticType.GUNFIRE, devices);
            ActivateGunEffects();
            
            if (bulletPref)
            {
                BulletShellSpawn();
            }

            FireBullet();

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

    //[ClientRpc]
    private void RpcCalculateGunRotation()
    {
        transform.rotation = Quaternion.FromToRotation(Vector3.forward, secondaryHoldingTransform.position - transform.position);
    }

    //[ClientRpc]
    private void RpcResetGunRotation()
    {
        transform.localRotation = Quaternion.Euler(initRot);
    }

    //[ClientRpc]
    private void RpcUnloadMagazine()
    {
        if (gun.GetMagazine() != null)
        {
            gun.GetMagazine().CmdUnsnap();
            gun.SetMagazine(null);
        }
    }

    //[ClientRpc]
    private void RpcLoadChamber()
    {
        // If there is a magazine attached and it has bullets
        if (gun.GetMagazine() != null && gun.GetMagazine().GetBulletsInMag() > 0)
        {
            // If chamber is already loaded
            if (gun.ChamberIsLoaded())
            {
                if (bulletPref)
                {
                    BulletShellSpawn();
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
        if (gunNetworkAnim)
        {
            gunNetworkAnim.SetTrigger("Fire");
        }

        if (muzzleFlashEffects)
        {
            muzzleFlashEffects.Activate();
        }

        if (networkedAudioSource)
        {
            networkedAudioSource.DirectPlay();
        }
    }

    #endregion ServerMethods

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