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
    [SerializeField] private Transform secondHandTransform;
    private Collider secondHandGrabCollider;
    private InteractableObject interactableObject;
    private bool isTwoHandedGrab = false;
    private bool isBeingGrabbed = false;
    private bool grabStateChanged = false;

    private void Start()
    {
        interactableObject = GetComponent<InteractableObject>();
        //gun = GetComponent<GunProfile>();

        if (secondHandTransform)
        {
            secondHandGrabCollider = secondHandTransform.GetComponent<Collider>();
        }
    }

    private void Update()
    {
        if (isTwoHandedGrab)
        {
            CmdCalculateGunRotation();
        }

        if (secondHandTransform)
        {
            CheckGrabState();
        }
    }

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
                secondHandGrabCollider.enabled = true;
            }
            else
            {
                secondHandGrabCollider.enabled = false;
            }

            grabStateChanged = isBeingGrabbed;
        }
    }

    public void UseObject()
    {
        CmdFireGun();
    }

    public void UpButtonPressed()
    {
        CmdLoadChamber();
    }

    public void DownButtonPressed()
    {
        CmdUnloadMagazine();
    }

    public void SecondHandActive()
    {
        isTwoHandedGrab = true;
    }

    public void SecondHandInactive()
    {
        isTwoHandedGrab = false;
        CmdResetGunRotation();
    }

    [Command]
    private void CmdFireGun()
    {
        RpcFireGun();
    }

    [Command]
    private void CmdCalculateGunRotation()
    {
        RpcCalculateGunRotation();
    }

    [Command]
    private void CmdResetGunRotation()
    {
        RpcResetGunRotation();
    }

    [Command]
    private void CmdUnloadMagazine()
    {
        RpcUnloadMagazine();
    }

    [Command]
    private void CmdLoadChamber()
    {
        RpcLoadChamber();
    }

    [ClientRpc]
    private void RpcFireGun()
    {
        // If chamber is loaded and not on safety mode
        if (gun.ChamberIsLoaded() && !gun.IsSafety())
        {
            // Fire bullet
            FireBullet();

            // If there is a magazine attached
            if(gun.GetMagazine() != null)
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
        }
        else
        {
            // Play no bullet sound
        }
    }

    [ClientRpc]
    private void RpcCalculateGunRotation()
    {
        transform.rotation = Quaternion.FromToRotation(Vector3.forward, secondHandTransform.position - transform.position);
    }

    [ClientRpc]
    private void RpcResetGunRotation()
    {
        transform.rotation = Quaternion.identity;
    }

    [ClientRpc]
    private void RpcUnloadMagazine()
    {
        if (gun.GetMagazine() != null)
        {
            gun.GetMagazine().CmdUnsnap();
            gun.SetMagazine(null);
        }
    }

    [ClientRpc]
    private void RpcLoadChamber()
    {
        // If there is a magazine attached and it has bullets
        if (gun.GetMagazine() != null && gun.GetMagazine().GetBulletsInMag() > 0)
        {
            // If chamber is already loaded
            if (gun.ChamberIsLoaded())
            {
                // Animate losing 1 bullet
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

    private void FireBullet()
    {
        RaycastHit hit;
        if (Physics.Raycast(firingPoint.position, firingPoint.forward, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Shootable")))
        {
            Instantiate(spawnPref, hit.point, firingPoint.rotation);
        }
    }

    [System.Serializable]
    public class GunProfile
    {
        [SerializeField] private float spread;
        private Magazine currMag;
        private bool bulletInChamber;
        private bool isSafe = false;

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