using UnityEngine;

namespace Assets.Scripts
{
    public class CameraMovement : MonoBehaviour
    {
        private const float Speed = 200f;
        public Bounds Bounds;

        void Update()
        {
            Vector3 pos = transform.position;

            if (Input.GetKey("w"))
            {
                pos.y += Speed * Time.deltaTime;
            }

            if (Input.GetKey("s"))
            {
                pos.y -= Speed * Time.deltaTime;
            }

            if (Input.GetKey("d"))
            {
                pos.x += Speed * Time.deltaTime;
            }

            if (Input.GetKey("a"))
            {
                pos.x -= Speed * Time.deltaTime;
            }

            if (Bounds.Contains(pos))
            {
                transform.position = pos;
            }
        }
    }
}