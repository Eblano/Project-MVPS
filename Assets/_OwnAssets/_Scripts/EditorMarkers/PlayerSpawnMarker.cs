using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    public class PlayerSpawnMarker : PointMarker, IMarkerBehaviours
    {
        private GameObject networkStartPosGO;

        private new void Start()
        {
            base.Start();
            //gameObject.name = RegisterMarkerOnGameManager(GameManager.MARKER_TYPE.PLAYER_SPAWN_MARKER);
            RegisterMarkerOnGameManager(GameManager.MARKER_TYPE.PLAYER_SPAWN_MARKER);

            if (!networkStartPosGO)
                networkStartPosGO = new GameObject();

            networkStartPosGO.AddComponent<UnityEngine.Networking.NetworkStartPosition>();
        }

        private new void Update()
        {
            base.Update();
            UpdateNetworkStartPosGOPos();
        }

        private void UpdateNetworkStartPosGOPos()
        {
            networkStartPosGO.transform.position = pointPosition;
        }

        private void OnDestroy()
        {
            Destroy(networkStartPosGO.gameObject);
        }

        private void OnDisable()
        {
            if (networkStartPosGO)
                networkStartPosGO.SetActive(false);
        }

        private void OnEnable()
        {
            if (networkStartPosGO)
                networkStartPosGO.SetActive(true);
        }
    }
}