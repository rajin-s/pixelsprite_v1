using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelSprite
{
    [ExecuteInEditMode]
    [AddComponentMenu("PixelSprite/Pixel Renderer", 0)]
    public class PixelRenderer : MonoBehaviour
    {
        public PixelSpriteProfile profile; // Profile for shared values, communication

        [Header("Object")]
        [SerializeField]
        private Rect bounds = new Rect(0, 0, 2, 2); // World-space window used to render object
        public Rect Bounds
        {
            get { return bounds; }
        }
        public Color outlineColor = Color.black;
        public Texture palette;

        [Header("Rendering")]
        public Material material = null; // Material override (uses profile default if null)
        [SortingLayer] public string sortingLayer = "Default";
        public int orderInLayer = 0;
        [SerializeField] private int updateFPS = 24;

        public bool doRender = true;

        public RenderTexture RenderTarget { get; private set; } // Texture that the object will be rendered into
        private int initialLayer;

        private float t = 0;
        private bool firstFrame = true;

        private void Awake()
        {
            if (profile) InitializeRenderTexture();
        }

        // Needs to be triggered by some camera which has the object visible (generally set to render to a throwaway rendertexture)
        public void OnWillRenderObject()
        {
            if (profile != null) profile.AddToQueue(this, updateFPS == -1 || t == 0);
        }

        private void Update()
        {
            if (!firstFrame)
            {
                t += Time.deltaTime * updateFPS;
                if (t >= 1) t = 0;
            }
            firstFrame = false;
        }

        // Initialization
        private void InitializeRenderTexture()
        {
            // Create a new render texture of the appropriate size
            RenderTarget = new RenderTexture(profile.UnitsToPixels(bounds.size.x), profile.UnitsToPixels(bounds.size.y), 16)
            {
                filterMode = FilterMode.Point,
                name = name + " Render Texture"
            };
        }

        // Rendering Steps
        public void Prep(int layer)
        {
            // Set layer to be visible to the render camera, save initial layer to reset later
            initialLayer = gameObject.layer;
            gameObject.layer = layer;
        }
        public void Release()
        {
            gameObject.layer = initialLayer;
            //material.SetTexture("_MainTex", RenderTarget);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(transform.position + (Vector3)Bounds.position, (Vector2)Bounds.size);
        }

        #if UNITY_EDITOR
        private void OnValidate()
        {
            if (profile && doRender) InitializeRenderTexture();
        }
        #endif
    }
}