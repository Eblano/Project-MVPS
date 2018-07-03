using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    /// <summary>
    /// Marker for identifying a seat
    /// </summary>
    public class SeatMarker : BaseMarker
    {
        private AreaMarker ownedByArea;
        [SerializeField] private bool seatAvailable = true;

        private void Start()
        {
            RegisterMarkerOnGameManager(this);
            Destroy(GetComponent<LineRenderer>());
        }

        public void SetSeatAreaOwned(AreaMarker area)
        {
            ownedByArea = area;
        }

        public bool OwnedByAnyArea()
        {
            if (ownedByArea)
                return true;
            else
                return false;
        }

        public bool OwnedByArea(AreaMarker area)
        {
            return area == ownedByArea;
        }

        public bool SeatAvailable()
        {
            return seatAvailable;
        }

        /// <summary>
        /// Set seat availablity
        /// </summary>
        /// <param name="occupied">True if seat is occupied</param>
        public void SetSeatAvailability(bool seatAvailable)
        {
            this.seatAvailable = seatAvailable;
        }
    }
}
