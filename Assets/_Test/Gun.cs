using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Gun : NetworkBehaviour, IUsableObject
{
    [SerializeField] private GameObject spawnPref;

    public void UseObject()
    {
        CmdFireGun();
    }
    [Command]
    private void CmdFireGun()
    {
        RpcFireGun();
    }

    //[Command]
    //private void CmdCalculateGunRotation()
    //{
    //    RpcCalculateGunRotation();
    //}

    //[Command]
    //private void CmdResetGunRotation()
    //{
    //    RpcResetGunRotation();
    //}

    [ClientRpc]
    private void RpcFireGun()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, ~LayerMask.NameToLayer("Shootable")))
        {
            Instantiate(spawnPref, hit.point, transform.rotation);
        }
    }
    
    //[ClientRpc]
    //private void RpcCalculateGunRotation()
    //{
    //    transform.rotation = Quaternion.FromToRotation(Vector3.forward, secondHandTransform.position - transform.position);
    //}

    //[ClientRpc]
    //private void RpcResetGunRotation()
    //{
    //    transform.rotation = Quaternion.identity;
    //}
}