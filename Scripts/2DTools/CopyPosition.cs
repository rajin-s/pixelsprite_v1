using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("PixelSprite/Utilities/Copy Position", 3)]
[ExecuteInEditMode]
public class CopyPosition : MonoBehaviour
{
    public Transform reference;
    private void LateUpdate()
    {
        if(reference != null)
            transform.position = reference.position;
    }
}
