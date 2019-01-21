using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace PixelSprite {
    [ExecuteInEditMode]
    [AddComponentMenu("PixelSprite/Main Camera", 1)]
    [RequireComponent(typeof(Camera))]
    public class MainCamera : MonoBehaviour
    {
        [ContextMenuItem("Reset", "ResetAll")]
        [SerializeField] private PixelSpriteProfile profile;
        [SerializeField] private int pixelSize = 128;
        new public Camera camera { get; private set; }

        private void Awake()
        {
            camera = GetComponent<Camera>();

            if (profile != null)
            {
                profile.SetMainCamera(this);
                profile.SetRenderCamera();
            }
            else
            {
                Debug.LogError("<Pixel Sprite> No profile set on MainCamera!");
            }

        }

        private void Update()
        {
            #if UNITY_EDITOR
            // Reset to avoid issues previewing in editor
            ResetAll();
            #endif
            // Set ortho size from pixel height
            SetOrthographicSize();
        }

        public void OnPreRender()
        {
            if (profile == null) return;
            // Clear command buffer
            profile.ClearQueue();
            // Move queue items into command buffer
            profile.ConvertQueue();
        }

        public void ResetAll()
        {
            profile.ResetAll();
            Awake();
        }

        public void SetOrthographicSize()
        {
            if (profile != null) camera.orthographicSize = profile.PixelsToUnits(pixelSize);
        }
    }
}