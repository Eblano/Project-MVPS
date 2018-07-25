using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SealTeam4;

public class NetworkASManager : MonoBehaviour
{
    [HideInInspector] public static NetworkASManager instance;
    [HideInInspector] public List<AudioSource> audioSources;

    private void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void RegisterAudioSource(AudioSource audioSource)
    {
        audioSources.Add(audioSource);
    }

    public void SendAudioSource(AudioSource audioSource)
    {
        GameManagerAssistant.instance.CmdSendAudioSourceInt(audioSources.IndexOf(audioSource));
    }

    public void PlayAudioSource(int i)
    {
        audioSources[i].Play();
    }
}
