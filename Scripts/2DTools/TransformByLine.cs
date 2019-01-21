using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using sf = UnityEngine.SerializeField;

[ExecuteInEditMode]
public class TransformByLine : MonoBehaviour
{
    [Header("Reference")]
    public LineRenderer line;
    public int startIndex = 0;
    public int endIndex = 1;

    [Header("Actions")]
    public bool doPosition = true;
    public bool doRotation = true;

    [Header("Settings")]
    [Range(0, 1)] public float positionInterpolation;
    public Vector3 positionOffset;
    public float angleOffset;
    public float angleScale = 1;

    private void Update()
    {
        if (line == null || line.positionCount <= startIndex || line.positionCount <= endIndex) return;

        Vector2 pA = line.GetPosition(startIndex);
        Vector2 pB = line.GetPosition(endIndex);
        if (doPosition) transform.localPosition = Vector3.Lerp(pA, pB, positionInterpolation) + positionOffset;
        if (doRotation) transform.localRotation = Quaternion.FromToRotation(Vector3.right, pB - pA); //Quaternion.Euler(Vector3.forward * (angleOffset + Vector2.Angle(Vector3.right, pB - pA) * angleScale * (pB.y < pA.y ? -1 : 1)));
    }
}
