using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static Dictionary<int, AudioSource> _audioSources;

    // Start is called before the first frame update
    void Start()
    {
        _audioSources = new Dictionary<int, AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void AddAudioSource(int instanceId, AudioClip clip)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        _audioSources.Add(instanceId, audioSource);
    }

    public void PlaySound(int instanceId)
    {
        _audioSources[instanceId].Play();
    }

    public void RemoveAudioSource(int instanceId)
    {
        _audioSources.Remove(instanceId);
    }
}