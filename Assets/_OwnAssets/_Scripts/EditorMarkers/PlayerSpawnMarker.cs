﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    public class PlayerSpawnMarker : PointMarker, IMarkerBehaviours
    {
        private GameObject networkStartPosGO;
        public static PlayerSpawnMarker instance;

        private new void Start()
        {
            if (instance == null)
                instance = this;
            else
            {
                Debug.Log("Duplicate PlayerSpawnMarker detected, destroying duplicate..");
                Destroy(gameObject);
            }

            base.Start();
            RegisterMarkerOnGameManager(this);
            CreateNetworkStartPosGO();
        }

        private void CreateNetworkStartPosGO()
        {
            if (!networkStartPosGO)
            {
                networkStartPosGO = new GameObject();
                networkStartPosGO.AddComponent<UnityEngine.Networking.NetworkStartPosition>();
                networkStartPosGO.AddComponent<Battlehub.RTSaveLoad.PersistentIgnore>();
            }
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
                Destroy(networkStartPosGO.gameObject);
        }

        private void OnEnable()
        {
            CreateNetworkStartPosGO();
        }
    }
}