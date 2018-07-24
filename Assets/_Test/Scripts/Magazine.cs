using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    public class Magazine : SnappableObject
    {
        [SerializeField] private int bulletsInMag;
        [SerializeField] private int bulletCapacity;
        [SerializeField] private Collider magCollider;

        /// <summary>
        /// Reduces the amount of bullets in magazine by 1.
        /// </summary>
        public void LoadIntoChamber()
        {
            bulletsInMag--;
        }

        /// <summary>
        /// Increases the amount of bullets in magazine by 1.
        /// </summary>
        public void LoadIntoMag()
        {
            bulletsInMag++;
        }

        /// <summary>
        /// Returns the current amount of bullets in magazine
        /// </summary>
        /// <returns></returns>
        public int GetBulletsInMag()
        {
            return bulletsInMag;
        }

        /// <summary>
        /// Overrides the base unsnap method
        /// </summary>
        public override void Unsnap()
        {
            //snappedTo.GetComponent<Gun>().gun.SetMagazine(null);
            base.Unsnap();
            magCollider.enabled = true;
        }

        /// <summary>
        /// Snaps object to
        /// </summary>
        /// <param name="snapTransform"></param>
        public override void SnapObject(Transform snapTransform)
        {
            magCollider.enabled = false;
            base.SnapObject(snapTransform);
            snapTransform.parent.GetComponent<Gun>().gun.SetMagazine(this);
        }
    }
}