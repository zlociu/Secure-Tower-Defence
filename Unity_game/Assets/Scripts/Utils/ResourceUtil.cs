using System.IO;
using UnityEngine;

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
    }
}