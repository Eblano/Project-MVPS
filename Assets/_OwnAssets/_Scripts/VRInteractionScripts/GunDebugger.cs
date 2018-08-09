using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunDebugger : MonoBehaviour
{
    [SerializeField] private Gun gunToDebug;
    [SerializeField] private Text gunStats;

    private void Update()
    {
        if (gunToDebug == null || gunToDebug.gun == null || gunToDebug.gun.GetMagazine() == null)
        {
            gunStats.text = "No Mag";
            return;
        }
        Gun.GunProfile profile = gunToDebug.gun;
        gunStats.text = "Chamber: " + profile.ChamberIsLoaded() + "/n Magazine: " + profile.GetMagazine().GetBulletsInMag();
    }
}
