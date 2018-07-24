using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class NetworkedAudioSource : MonoBehaviour
{
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (NetworkASManager.instance)
        {
            // Registers the audio source attached to this object on start
            NetworkASManager.instance.RegisterAudioSource(audioSource);
        }
        else
        {
            Debug.Log("NetworkAS not found");
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            NetworkASManager.instance.SendAudioSource(audioSource);
        }
    }
}
