using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace PixelSprite
{
    // ScriptableObject class for storing information and communication
    [ExecuteInEditMode]
    [CreateAssetMenu(fileName = "PS_Profile", menuName = "Pixel Sprite Profile", order = 0)]
    public class PixelSpriteProfile : ScriptableObject
    {
        private struct DrawMeshItem
        {
            public Texture texture;
            public Matrix4x4 transform;
            public Material material;
            public Color outlineColor;
            public Texture palette;

            public DrawMeshItem(Texture tx, Matrix4x4 tr, Material mt, Color oc, Texture pl)
            {
                texture = tx;
                transform = tr;
                material = mt;
                outlineColor = oc;
                palette = pl;
            }
        }
        [Header("Objects")]
        [SerializeField] private int pixelsPerUnit = 32; // Uniform pixels per world unit for rendered objects
        [SerializeField] private int tempLayer = 31; // Layer used to render individual objects
        [SerializeField] private int sortingLayerContribution = 1000; // Contribution of sorting layer to render order (order is determined by <object sorting layer> * <sorting layer contribution> + <order in layer>)

        [Header("Camera")]
        [SerializeField] private Mesh drawMesh; // Mesh used to draw final versions of all objects (quad in 99% of cases)
        [SerializeField] private Material defaultMaterial; // Default material used to render objects if no override is given
        [SerializeField] private Texture defaultPalette; // Default palette texture if used by material and none specified by PixelRenderer
        [SerializeField] private CameraEvent commandBufferEvent = CameraEvent.BeforeForwardOpaque; // Event at which the command buffer is inserted
        private MainCamera mainCamera; // Main camera used to render final versions of all objects
        private Camera renderCamera = null; // Camera used to render downscaled objects
        private string renderCameraTag = "ObjectRenderCamera"; // Tag used to find appropriate render camera

        private SortedList<int, DrawMeshItem> drawQueue = new SortedList<int, DrawMeshItem>();
        private CommandBuffer commandBuffer; // Command buffer used to draw objects on screen
        private MaterialPropertyBlock propertyBlock; // Property block used to specify (non-shared) material texture

        public void ResetAll()
        {
            mainCamera = null;
            renderCamera = null;
            drawQueue = new SortedList<int, DrawMeshItem>();
            commandBuffer = new CommandBuffer();
            propertyBlock = new MaterialPropertyBlock();
        }

        public void SetMainCamera(MainCamera mc)
        {
            // Set camera reference
            mainCamera = mc;
            mainCamera.camera.RemoveAllCommandBuffers();
            
            // Add command buffer to camera at specified event
            commandBuffer = new CommandBuffer() { name = "PixelSprite Commands" };
            mainCamera.camera.AddCommandBuffer(commandBufferEvent, commandBuffer);

            // Initialize property block
            propertyBlock = new MaterialPropertyBlock();
        }
        public void SetRenderCamera()
        {
            renderCamera = GameObject.FindWithTag(renderCameraTag).GetComponent<Camera>();
        }

        public int UnitsToPixels(float units)
        {
            return (int)(units * pixelsPerUnit);
        }
        public float PixelsToUnits(int pixels)
        {
            return ((float)pixels) / pixelsPerUnit;
        }

        public void AddToQueue(PixelRenderer subject, bool doUpdate)
        {
            #if (UNITY_EDITOR)
            // Prevent triggering of render from scene view
            if (new List<Camera>(UnityEditor.SceneView.GetAllSceneCameras()).Contains(Camera.current)) return;
            #endif
            // Don't add to queue if the active camera is already the render camera
            if (Camera.current == renderCamera) return;

            // Get the subject bounding rect
            Rect bounds = subject.Bounds;

            if (doUpdate)
            {
                // Prep the subject and sub-sprites
                subject.Prep(tempLayer);
                subject.BroadcastMessage("PrepSubSprite", tempLayer, SendMessageOptions.DontRequireReceiver);

                // Position the render camera
                renderCamera.transform.position = (Vector3)((Vector2)subject.transform.position + bounds.position) + Vector3.forward * renderCamera.transform.position.z;
                renderCamera.orthographicSize = bounds.size.y / 2;
                renderCamera.targetTexture = subject.RenderTarget;

                // Take the picture
                renderCamera.Render();
                renderCamera.targetTexture = null;

                // Release the subject and sub-sprites
                subject.Release();
                subject.BroadcastMessage("ReleaseSubSprite", SendMessageOptions.DontRequireReceiver);
            }

            // Add item to draw queue
            Matrix4x4 transformMatrix = new Matrix4x4(
                        new Vector4(bounds.size.x, 0, 0, 0),
                        new Vector4(0, bounds.size.y, 0, 0),
                        new Vector4(0, 0, 1, 0),
                        new Vector4(subject.transform.position.x + bounds.position.x, subject.transform.position.y + bounds.position.y, 0, 1)
                    );
            //transformMatrix = mainCamera.transform.worldToLocalMatrix * transformMatrix;

            int order = subject.orderInLayer + SortingLayer.GetLayerValueFromName(subject.sortingLayer) * sortingLayerContribution;
            DrawMeshItem item = new DrawMeshItem(subject.RenderTarget, transformMatrix, subject.material ? subject.material : defaultMaterial, subject.outlineColor, subject.palette ? subject.palette : defaultPalette);
            while (drawQueue.ContainsKey(order)) order++;
            drawQueue.Add(order, item);
        }

        public void ConvertQueue()
        {
            while (drawQueue.Count > 0)
            {
                // Pop item off front of queue
                DrawMeshItem item = drawQueue.Values[0];
                drawQueue.RemoveAt(0);

                // Set property block texture and submit to command buffer
                propertyBlock.SetTexture("_MainTex", item.texture);
                propertyBlock.SetColor("_OutlineColor", item.outlineColor);
                propertyBlock.SetTexture("_PaletteTex", item.palette);

                commandBuffer.DrawMesh(drawMesh, item.transform, item.material, 0, 0, propertyBlock);
            }
        }

        public void ClearQueue()
        {
            commandBuffer.Clear();
        }
    }
}