using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static Dictionary<int, AudioSource> _audioSources;
    private static List<int> _audioSourcesToDelete;

    // Start is called before the first frame update
    private void Start()
    {
        _audioSources = new Dictionary<int, AudioSource>();
        _audioSourcesToDelete = new List<int>();
    }

    // Update is called once per frame
    private void Update()
    {
        for (int i = 0; i < _audioSourcesToDelete.Count; i++)
        {
            int instanceId = _audioSourcesToDelete[i];
            if (!_audioSources[instanceId].isPlaying)
            {
                Destroy(_audioSources[instanceId]);
                _audioSources.Remove(instanceId);
                _audioSourcesToDelete[i] = 0;
            }
        }

        _audioSourcesToDelete = _audioSourcesToDelete.FindAll(e => e != 0);
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
        _audioSourcesToDelete.Add(instanceId);
    }
}