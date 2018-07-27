using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    public class MuzzleFlash : MonoBehaviour
    {
        [SerializeField] private ParticleSystem[] particleList;
        [SerializeField] private Light flash;

        public void Activate()
        {
            StartCoroutine(FlashLight());

            foreach (ParticleSystem p in particleList)
            {
                p.Play();
            }
        }

        IEnumerator FlashLight()
        {
            flash.enabled = true;
            yield return new WaitForSeconds(0.2f);
            flash.enabled = false;
        }
    }
}
