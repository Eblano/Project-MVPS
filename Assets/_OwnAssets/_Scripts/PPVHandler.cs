using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PPVHandler : MonoBehaviour
{
    public static PPVHandler instance;

    [SerializeField] private PostProcessVolume ppv;
    [SerializeField] private int smoothness = 20;
    [SerializeField] private float increment = 0.1f;
    [SerializeField] private float vignetteDura = 1f;
    private bool checkPPV = true;

    private bool isPlaying;

    private void Start()
    {
        if (!instance)
            instance = this;
    }

    public void CheckHpEffect(int hp)
    {
        if (hp <= 40)
        {
            checkPPV = false;
            StartCoroutine(OnLowHp());
        }

        StartCoroutine(OnHit(hp -= 10));
    }

    private IEnumerator OnLowHp()
    {
        ppv.enabled = true;
        
        for (; ; )
        {
            for (int i = 0; i < smoothness; i++)
            {
                ppv.weight += increment / smoothness;
                Debug.Log(ppv.weight);
                yield return new WaitForSeconds((vignetteDura / smoothness) / 2.0f);
            }

            for (int i = 0; i < smoothness; i++)
            {
                ppv.weight -= increment / smoothness;
                Debug.Log(ppv.weight);
                yield return new WaitForSeconds((vignetteDura / smoothness) / 2.0f);
            }
        }
    }

    private IEnumerator OnHit(int hp)
    {
        if (isPlaying)
            yield return null;

        isPlaying = true;
        if (checkPPV)
            ppv.enabled = true;

        ppv.weight = 1 - (hp / 100.0f);
        Debug.Log(ppv.weight);

        for (int i = 0; i < smoothness; i++)
        {
            ppv.weight += increment / smoothness;
            Debug.Log(ppv.weight);
            yield return new WaitForSeconds((vignetteDura / smoothness) / 2.0f);
        }

        for (int i = 0; i < smoothness; i++)
        {
            ppv.weight -= increment / smoothness;
            Debug.Log(ppv.weight);
            yield return new WaitForSeconds((vignetteDura / smoothness) / 2.0f);
        }

        if(checkPPV)
            ppv.enabled = false;
        isPlaying = false;
    }
}
