using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class CameraZoom : MonoBehaviour
    {
        private const int ZoomSpeed = 250;
        public float CurrentOrtho;
        private const float SmoothSpeed = 1f;
        private const float MinOrtho = 60.0f;
        public float MaxOrtho;

        void Start()
        {
            CurrentOrtho = Camera.main.orthographicSize;
        }

        void Update()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Math.Abs(scroll) > 0.01f)
            {
                CurrentOrtho -= scroll * ZoomSpeed;
                ApplyOrthoLimits();
            }

            Camera.main.orthographicSize =
                Mathf.MoveTowards(Camera.main.orthographicSize, CurrentOrtho, SmoothSpeed);
        }

        public void ApplyOrthoLimits()
        {
            CurrentOrtho = Mathf.Clamp(CurrentOrtho, MinOrtho, MaxOrtho);
        }
    }
}