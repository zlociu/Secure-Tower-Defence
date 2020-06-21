using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Utils
{
    class ResourceUtil
    {
        private static Dictionary<string, Sprite> _loadedTextures = new Dictionary<string, Sprite>();
        private static Dictionary<string, AudioClip> _loadedSounds = new Dictionary<string, AudioClip>();
        public static Sprite LoadSprite(string texturePath)
        {
            if (_loadedTextures.ContainsKey(texturePath))
            {
                return _loadedTextures[texturePath];
            }
            Texture2D texture = new Texture2D(32, 32);
            texture.LoadImage(File.ReadAllBytes("data/Sprites/" + texturePath));
            texture.filterMode = FilterMode.Point;
            Sprite result = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f), 1f);
            _loadedTextures.Add(texturePath, result);
            return result;
        }

        public static AudioClip LoadSound(string soundPath)
        {
            Debug.Log(soundPath);
            if (_loadedSounds.ContainsKey(soundPath))
            {
                return _loadedSounds[soundPath];
            }
            UnityWebRequest audioRequest =
                UnityWebRequestMultimedia.GetAudioClip(Application.dataPath + "/../data/Sounds/" + soundPath,
                    AudioType.OGGVORBIS);
            audioRequest.SendWebRequest();
            while (!audioRequest.isDone)
            {
                new WaitForSeconds(0.1f);
            }

            if (audioRequest.isNetworkError)
            {
                Debug.Log(audioRequest.error);
                return null;
            }

            AudioClip clip = DownloadHandlerAudioClip.GetContent(audioRequest);
            _loadedSounds.Add(soundPath, clip);
            return clip;
        }

        public static void Reset()
        {
            _loadedTextures.Clear();
            _loadedSounds.Clear();
        }
    }
}