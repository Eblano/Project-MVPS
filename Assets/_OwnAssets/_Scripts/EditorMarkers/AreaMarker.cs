using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battlehub.RTSaveLoad;
using System;
using Battlehub;

namespace SealTeam4
{
    public class AreaMarker : BaseAreaMarker, IActions
    {
        private List<GameObject> registeredSeats = new List<GameObject>();
        [SerializeField] private int numRegisteredSeats = 0;
        [SerializeIgnore] private float refreshRate = 3.0f;
        private float currRefreshRate;

        private List<string> actionList = new List<string>();
        private List<AIController> npcsSeatedInArea = new List<AIController>();

        private new void Start()
        {
            base.Start();
            actionList.Add("Dismiss all seated NPCs");
        }

        private new void Update()
        {
            base.Update();
            UpdateRegisteredSeatNumber();
        }

        private void UpdateRegisteredSeatNumber()
        {
            numRegisteredSeats = registeredSeats.Count;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.GetComponent<SeatMarker>())
            {
                SeatMarker seat = other.GetComponent<SeatMarker>();
                if(!seat.OwnedByAnyArea())
                {
                    RegisterSeat(seat.gameObject);
                    seat.SetSeatAreaOwned(this);
                }
            }
        }

        public void RegisterNPCSitInArea(AIController npc)
        {
            npcsSeatedInArea.Add(npc);
        }

        public void UnregisterNPCSitInArea(AIController npc)
        {
            if (npcsSeatedInArea.Exists(x => x == npc))
                npcsSeatedInArea.Remove(npc);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<SeatMarker>())
            {
                GameObject seat = other.gameObject;
                if(seat.GetComponent<SeatMarker>().OwnedByArea(this))
                {
                    UnregisterSeat(seat);
                    seat.GetComponent<SeatMarker>().SetSeatAreaOwned(null);
                }
            }
        }

        private void UnregisterSeat(GameObject seat)
        {
            registeredSeats.Remove(seat);
        }

        public void RegisterSeat(GameObject seat)
        {
            registeredSeats.Add(seat);
        }

        public SeatMarker GetRandomEmptySeat()
        {
            foreach(GameObject seat in registeredSeats)
            {
                if(seat.GetComponent<SeatMarker>().SeatAvailable())
                {
                    return seat.GetComponent<SeatMarker>();
                }
            }
            Debug.Log("No Seats found in " + gameObject.name);
            return null;
        }

        public List<string> GetActions()
        {
            return actionList;
        }

        public void SetAction(string action)
        {
            switch (action)
            {
                case "Dismiss all seated NPCs":
                    foreach(AIController npc in npcsSeatedInArea)
                    {
                        npc.SetAction("Dismiss from Seat");
                    }
                    break;
            }
        }

        public string GetName()
        {
            return gameObject.name;
        }

        public Vector3 GetHighestPointPos()
        {
            return highestPoint.position;
        }

        public Transform GetHighestPointTransform()
        {
            return highestPoint;
        }

        public Collider GetCollider()
        {
            return mCollider;
        }
    }
}
