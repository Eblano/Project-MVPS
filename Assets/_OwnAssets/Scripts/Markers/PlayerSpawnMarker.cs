using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    public class PlayerSpawnMarker : PointMarker
    {
        [SerializeField] private GameObject networkSpawnMarker;

        private new void Start()
        {
            RegisterMarkerOnGameManager(GameManager.MARKER_TYPE.PLAYER_SPAWN_MARKER);
            base.Start();
        }

        public void SpawnNetworkPlayerMarker()
        {
            if (networkSpawnMarker)
            {
                Instantiate(networkSpawnMarker, transform.position, transform.rotation);
            }
            else
            {
                Debug.Log("missing network spawn marker, failed to spawn");
            }

            Destroy(gameObject);
        }
    }
}