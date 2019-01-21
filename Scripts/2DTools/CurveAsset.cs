using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CurveAsset", menuName = "Curve Asset", order = 0)]
public class CurveAsset : ScriptableObject
{
    public AnimationCurve curve;
    public float Evaluate(float t)
    {
        return curve.Evaluate(t);
    }

    public static implicit operator AnimationCurve (CurveAsset o)
    {
        return o.curve;
    }
}
