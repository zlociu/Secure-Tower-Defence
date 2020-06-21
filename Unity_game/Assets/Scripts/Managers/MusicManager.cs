using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Assets.Scripts.Utils;
using UnityEngine;
using Random = System.Random;

public class MusicManager : MonoBehaviour
{
    private static AudioSource _source;
    private static List<AudioClip> _battleMusicClips = new List<AudioClip>();
    private static List<AudioClip> _menuMusicClips = new List<AudioClip>();
    private static int _isMusicPlaying = 0;

    private void Start()
    {
        if (GameObject.FindGameObjectWithTag("music"))
        {
            Destroy(this);
            return;
        }

        gameObject.tag = "music";
        DontDestroyOnLoad(transform.gameObject);
        _source = gameObject.AddComponent<AudioSource>();
        _source.loop = true;
    }

    public static void PlayBattleMusic()
    {
        if (_isMusicPlaying == 2 || _menuMusicClips.Count == 0)
        {
            return;
        }

        int randomBattleMusic = new Random().Next(0, _battleMusicClips.Count);
        _source.Stop();
        _source.clip = _battleMusicClips[randomBattleMusic];
        _source.Play();
        _isMusicPlaying = 2;
    }

    public static void PlayMenuMusic()
    {
        if (_isMusicPlaying == 1 || _battleMusicClips.Count == 0)
        {
            return;
        }

        int randomBattleMusic = new Random().Next(0, _menuMusicClips.Count);
        _source.Stop();
        _source.clip = _menuMusicClips[randomBattleMusic];
        _source.Play();
        _isMusicPlaying = 1;
    }

    public static void Stop()
    {
        if (_isMusicPlaying == 0)
        {
            return;
        }

        _source.Stop();
        _isMusicPlaying = 0;
    }

    public static void LoadAllMusicClips()
    {
        _battleMusicClips.Clear();
        _menuMusicClips.Clear();
        string[] filePaths = Directory.GetFiles(Application.dataPath + "/../data/Sounds/Music", "*.ogg",
            SearchOption.AllDirectories);
        foreach (string filePath in filePaths)
        {
            string filePathCropped = filePath.Replace("\\", "/");
            filePathCropped = Regex.Match(filePathCropped, "Music/.+").ToString();
            AudioClip clip = ResourceUtil.LoadSound(filePathCropped);
            if (filePathCropped.ToLower().Contains("battle"))
            {
                _battleMusicClips.Add(clip);
            }
            else
            {
                _menuMusicClips.Add(clip);
            }
        }
    }
}