using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelSprite
{
    [ExecuteInEditMode]
    [AddComponentMenu("PixelSprite/Activation Camera", 2)]
    [RequireComponent(typeof(Camera))]
    public class ActivationCamera : MonoBehaviour
    {
        private Camera cam;
        private void Awake()
        {
            float ratio = (float)Screen.width / Screen.height;
            cam = GetComponent<Camera>();
            cam.targetTexture = new RenderTexture(Mathf.CeilToInt(ratio), 1, 0)
            {
                name = "Activation Camera Render Texture"
            };
        }

        private void Update()
        {
            cam.orthographicSize = Camera.main.orthographicSize;
        }

#if (UNITY_EDITOR)
        private void OnEnable()
        {
            Awake();
        }
#endif
    }

}