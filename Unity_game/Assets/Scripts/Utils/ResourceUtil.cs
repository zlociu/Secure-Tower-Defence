using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Utils
{
    class ResourceUtil
    {
        public static Sprite LoadSprite(string texturePath)
        {
            Texture2D texture = new Texture2D(32, 32);
            texture.LoadImage(File.ReadAllBytes("data/Sprites/" + texturePath));
            texture.filterMode = FilterMode.Point;
            return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f), 1f);
        }

        public static AudioClip LoadSound(string soundPath)
        {
            UnityWebRequest audioRequest =
                UnityWebRequestMultimedia.GetAudioClip(Application.dataPath + "/../data/Sounds/" + soundPath,
                    AudioType.OGGVORBIS);
            audioRequest.SendWebRequest();
            while (!audioRequest.isDone)
            {
                new WaitForSeconds(0.5f);
            }

            if (audioRequest.isNetworkError)
            {
                Debug.Log(audioRequest.error);
                return null;
            }

            AudioClip clip = DownloadHandlerAudioClip.GetContent(audioRequest);
            return clip;
        }
    }
}