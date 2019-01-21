using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelSprite
{
    [ExecuteInEditMode]
    public class SubSprite : MonoBehaviour
    {
        private int initialLayer;

        public void PrepSubSprite(int layer)
        {
            initialLayer = gameObject.layer;
            gameObject.layer = layer;
        }
        public void ReleaseSubSprite()
        {
            gameObject.layer = initialLayer;
        }
    }
}