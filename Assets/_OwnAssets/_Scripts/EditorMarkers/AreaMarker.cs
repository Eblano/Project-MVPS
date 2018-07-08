using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battlehub.RTSaveLoad;
using System;
using Battlehub;

namespace SealTeam4
{
    public class AreaMarker : BaseAreaMarker//, IActions
    {
        private List<GameObject> registeredSeats = new List<GameObject>();
        [SerializeField] private int numRegisteredSeats = 0;
        [SerializeIgnore] private float refreshRate = 3.0f;
        private float currRefreshRate;

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

        public GameObject GetRandomEmptySeat()
        {
            foreach(GameObject seat in registeredSeats)
            {
                if(seat.GetComponent<SeatMarker>().SeatAvailable())
                {
                    return seat;
                }
            }
            Debug.Log("No Seats found in " + gameObject.name);
            return null;
        }

        public List<string> GetActions()
        {
            throw new NotImplementedException();
        }

        public void SetAction(string action)
        {
            throw new NotImplementedException();
        }

        public string GetName()
        {
            return gameObject.name;
        }

        public Vector3 GetHighestPointPos()
        {
            throw new NotImplementedException();
        }

        public Transform GetHighestPointTransform()
        {
            throw new NotImplementedException();
        }

        public Collider GetCollider()
        {
            throw new NotImplementedException();
        }
    }
}
