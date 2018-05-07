using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    public class Magazine : SnappableObject
    {
        [SerializeField] private int bulletsInMag;

        public void LoadIntoChamber()
        {
            bulletsInMag--;
        }

        public void LoadIntoMag()
        {
            bulletsInMag++;
        }

        public int GetBulletsInMag()
        {
            return bulletsInMag;
        }

        public override void Unsnap()
        {
            //snappedTo.GetComponent<Gun>().gun.SetMagazine(null);
            base.Unsnap();
        }

        public override void SnapObject(Transform snapTransform)
        {
            base.SnapObject(snapTransform);
            snapTransform.parent.GetComponent<Gun>().gun.SetMagazine(this);
        }
    }
}