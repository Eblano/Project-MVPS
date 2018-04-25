using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    public float moveSpeed;
    public GameObject bullet;
    public float bulletSpeed;
    public Transform firingPoint;

    private void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        CmdMovePlayer(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CmdFireBullet();
        }
    }

    [Command]
    private void CmdFireBullet()
    {
        GameObject go = Instantiate(bullet, firingPoint.position, firingPoint.rotation);
        go.GetComponent<Rigidbody>().velocity = go.transform.up * bulletSpeed * Time.deltaTime;

        NetworkServer.Spawn(go);
    }

    [Command]
    private void CmdMovePlayer(float vertical, float horizontal)
    {
        RpcUpdatePlayerPos(vertical, horizontal);
    }

    [ClientRpc]
    private void RpcUpdatePlayerPos(float vertical, float horizontal)
    {
        transform.Translate(horizontal * moveSpeed * Time.deltaTime, vertical * moveSpeed * Time.deltaTime, 0);
    }
}
