using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class NetworkedAudioSource : MonoBehaviour
{
    private AudioSource[] audioSources;
    private int counter = 0;

    private void Start()
    {
        audioSources = GetComponents<AudioSource>();

        if (NetworkASManager.instance)
        {
            // Registers the audio source attached to this object on start
            foreach(AudioSource audio in audioSources)
            {
                NetworkASManager.instance.RegisterAudioSource(audio);
            }
        }
        else
        {
            Debug.Log("NetworkAS not found");
        }
    }

    public void Play()
    {
        NetworkASManager.instance.SendAudioSource(audioSources[counter++ % audioSources.Length]);
    }

    public void DirectPlay()
    {
        audioSources[counter++ % audioSources.Length].Play();
    }

    public void DirectStop()
    {
        audioSources[counter++ % audioSources.Length].Stop();
    }
}
