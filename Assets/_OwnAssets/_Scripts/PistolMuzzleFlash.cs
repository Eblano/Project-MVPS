using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SealTeam4
{
    public class PistolMuzzleFlash : MonoBehaviour
    {
        [SerializeField] private ParticleSystem[] particleList;
        [SerializeField] private Light flash;
        // Use this for initialization

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
